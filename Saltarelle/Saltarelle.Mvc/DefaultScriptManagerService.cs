using System;
using System.Reflection;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Web;
using System.IO;
using System.Linq;
using Saltarelle.Mvc;
using dotless.Core;
using dotless.Core.configuration;
using System.Web.Configuration;
using Saltarelle.Configuration;
using System.Configuration;

// ReSharper disable CheckNamespace
namespace Saltarelle {
// ReSharper restore CheckNamespace
	public class DefaultScriptManagerService : IScriptManagerService {
		private int nextUniqueId = 1;

		private static bool debugScripts;
		private static ReadOnlyCollection<string> addScriptsBeforeCoreScripts;
		private static ReadOnlyCollection<string> addScriptsBeforeAssemblyScripts;
		private static ReadOnlyCollection<string> addScriptsAfterAssemblyScripts;

		static DefaultScriptManagerService() {
			var cfg = SaltarelleConfig.GetFromWebConfig();
			debugScripts = cfg.Scripts.Debug;

			var allScripts = cfg.Scripts.Cast<ScriptElement>().Select(elem => {
				string url;

				if (!string.IsNullOrEmpty(elem.Assembly)) {
					if (string.IsNullOrEmpty(elem.Resource))
						throw new ConfigurationErrorsException("Saltarelle configuration: if an assembly is specified for a script, the resource name must also be specified.");
					if (!string.IsNullOrEmpty(elem.Url))
						throw new ConfigurationErrorsException("Saltarelle configuration: if an assembly is specified for a script, the URL may not also be specified.");
					Assembly asm;
					try {
						asm = Assembly.Load(elem.Assembly);
					}
					catch (Exception ex) {
						throw new ConfigurationErrorsException("Saltarelle configuration: The assembly '" + elem.Assembly + "' could not be loaded.", ex);
					}
					var res = asm.GetCustomAttributes(typeof(WebResourceAttribute), false).Cast<WebResourceAttribute>().SingleOrDefault(x => x.PublicResourceName == elem.Resource);
					if (Utils.IsNull(res))
						throw new ConfigurationErrorsException("Saltarelle configuration: The assembly '" + elem.Assembly + "' does not contain a resource named '" + elem.Resource + "'.");
					url = Routes.GetAssemblyResourceUrl(asm, res.PublicResourceName);
				}
				else if (!string.IsNullOrEmpty(elem.Url)) {
					if (VirtualPathUtility.IsAppRelative(elem.Url))
						url = VirtualPathUtility.ToAbsolute(elem.Url);
					else
						url = elem.Url;
				}
				else
					throw new ConfigurationErrorsException("Saltarelle configuration: script elements must have assembly/resource or url specified.");
				return new { elem.Position, Url = url };
			}).ToList();

			addScriptsBeforeCoreScripts     = (from x in allScripts where x.Position == ScriptPosition.BeforeCoreScripts select x.Url).ToList().AsReadOnly();
			addScriptsBeforeAssemblyScripts = (from x in allScripts where x.Position == ScriptPosition.BeforeAssemblyScripts select x.Url).ToList().AsReadOnly();
			addScriptsAfterAssemblyScripts  = (from x in allScripts where x.Position == ScriptPosition.AfterAssemblyScripts select x.Url).ToList().AsReadOnly();
		}
	
		private HashSet<Assembly>  registeredAssemblies    = new HashSet<Assembly>();
		private HashSet<Type>      registeredServices      = new HashSet<Type>();
		private List<string>       earlyAdditionalIncludes = new List<string>();
		private List<string>       lateAdditionalIncludes  = new List<string>();
		private List<Func<string>> startupScripts          = new List<Func<string>>();
		private List<Action>       beforeRenderCallbacks   = new List<Action>();
		private Dictionary<string, IControl> topLevelControls = new Dictionary<string, IControl>();

		public void RegisterClientAssembly(Assembly asm) {
			registeredAssemblies.Add(asm);
		}
		
		public void RegisterClientService(Type serviceType) {
			if (serviceType == null)
				throw new ArgumentNullException("serviceType");
			this.RegisterClientType(serviceType);
			registeredServices.Add(serviceType);
		}
		
		public bool IsClientServiceRegistered(Type serviceType) {
			return registeredServices.Contains(serviceType);
		}

		public IEnumerable<string> GetAllRequiredIncludes() {
#warning TODO: Fix
//			var asms = registeredAssemblies.Concat(GlobalServices.AllLoadedServices.Select(kvp => kvp.Value.GetType().Assembly));
            var asms = registeredAssemblies;
			return earlyAdditionalIncludes.Concat(ModuleUtils.TopologicalSortAssembliesWithDependencies(asms).Select(a => Routes.GetAssemblyScriptUrl(a))).Concat(lateAdditionalIncludes);
		}
		
		public void AddScriptInclude(string url, bool includeBeforeAssemblyScripts) {
			List<string> l = (includeBeforeAssemblyScripts ? earlyAdditionalIncludes : lateAdditionalIncludes);
			if (!l.Contains(url))
				l.Add(url);
		}
		
		public void AddStartupScript(Func<string> scriptRetriever) {
			startupScripts.Add(scriptRetriever);
		}

		public void RegisterTopLevelControl(string id, IControl control) {
			if (!string.IsNullOrEmpty(id))
				topLevelControls[id] = control;
			string fmt = (!string.IsNullOrEmpty(id) ? "Saltarelle.GlobalServices.getService(" + typeof(IScriptManagerService).FullName + ").registerTopLevelControl(" + Utils.ScriptStr(id) + ", {0});" : "{0}");
			startupScripts.Add(() => string.Format(fmt, "new " + control.GetType().FullName + "(" + Utils.InitScript(control.ConfigObject) + ")"));
		}
		
		public void RegisterTopLevelControl(IControl control) {
			RegisterTopLevelControl(null, control);
		}
		
		public IControl GetTopLevelControl(string id) {
			IControl c;
			topLevelControls.TryGetValue(id, out c);
			return c;
		}
		
		public IEnumerable<string> GetStartupScripts() {
			return          registeredServices
#warning TODO: Fix
//			               .Select(svc => new { svc, impl = GlobalServices.GetService(svc) })
			               .Select(svc => new { svc, impl = (object)null })
			               .Select(x => "if (typeof(Saltarelle) != 'undefined' && !Saltarelle.GlobalServices.hasService(" + x.svc.FullName + ")) Saltarelle.GlobalServices.setService(" + x.svc.FullName + ", new " + x.impl.GetType().FullName + "(" + (x.impl is ITransferrableService ? Utils.InitScript((x.impl as ITransferrableService).ConfigObject) : "") + "));")
			       .Concat(startupScripts.Select(f => f()).Where(s => !string.IsNullOrEmpty(s)));
		}
		
		public string GetUniqueId() {
			return "id" + Utils.ToStringInvariantInt(nextUniqueId++);
		}
		
		public void RegisterBeforeRenderCallback(Action action) {
			beforeRenderCallbacks.Add(action);
		}
		
		public void ExecuteBeforeRenderCallbacks() {
			// The callback might register new callbacks, so make sure we handle that.
			while (beforeRenderCallbacks.Count > 0) {
				var oldCallbacks = beforeRenderCallbacks;
				beforeRenderCallbacks = new List<Action>();
				foreach (var a in oldCallbacks)
					a();
			}
		}
		
		public object ConfigObject {
			get { return new { nextUniqueId }; }
		}
		
		public void Setup() {
			earlyAdditionalIncludes.AddRange(addScriptsBeforeCoreScripts);
			earlyAdditionalIncludes.AddRange((debugScripts ? Resources.CoreScriptsDebug : Resources.CoreScriptsRelease).Select(s => Routes.GetAssemblyResourceUrl(typeof(Resources).Assembly, s)));
			earlyAdditionalIncludes.AddRange(addScriptsBeforeAssemblyScripts);
			lateAdditionalIncludes.AddRange(addScriptsAfterAssemblyScripts);
		}
	}
}
