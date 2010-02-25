using System;
#if SERVER
using System.Reflection;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Web;
using System.IO;
using System.Linq;
using dotless.Core;
using dotless.Core.configuration;
using Saltarelle.Mvc;
using System.Web.Configuration;
using Saltarelle.Configuration;
using System.Configuration;
#endif
#if CLIENT
using System.DHTML;
#endif

namespace Saltarelle {
#if SERVER
	[GlobalService(typeof(IScriptManagerService))]
	public class DefaultScriptManagerProvider : IScriptManagerService, IGlobalService {
		private int nextUniqueId = 1;

		private static bool debugScripts;
		private static List<string> addScriptsBeforeCoreScripts;
		private static List<string> addScriptsBeforeAssemblyScripts;
		private static List<string> addScriptsAfterAssemblyScripts;

		static DefaultScriptManagerProvider() {
			var cfg = (SaltarelleConfigSection)WebConfigurationManager.GetSection("saltarelle");
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
					if (res == null)
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

			addScriptsBeforeCoreScripts     = (from x in allScripts where x.Position == ScriptPosition.BeforeCoreScripts select x.Url).ToList();
			addScriptsBeforeAssemblyScripts = (from x in allScripts where x.Position == ScriptPosition.BeforeAssemblyScripts select x.Url).ToList();
			addScriptsAfterAssemblyScripts  = (from x in allScripts where x.Position == ScriptPosition.AfterAssemblyScripts select x.Url).ToList();
		}
	
		private HashSet<Assembly> registeredAssemblies = new HashSet<Assembly>();
		private List<string> earlyAdditionalIncludes = new List<string>();
		private List<string> lateAdditionalIncludes = new List<string>();
		private List<Func<string>> startupScripts = new List<Func<string>>();

		public void RegisterType(Type type) {
			registeredAssemblies.Add(type.Assembly);
		}

		public IEnumerable<string> GetAllRequiredIncludes() {
			var asms = registeredAssemblies.Concat(GlobalServices.AllLoadedServices.Select(kvp => kvp.Value.GetType().Assembly));
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
		
		public void RegisterTopLevelControl(IControl control) {
			startupScripts.Add(() => "new " + control.GetType().FullName + "('" + control.Id + "');");
		}
		
		public void AddStartupScript(string script) {
			startupScripts.Add(() => script);
		}
		
		public IEnumerable<string> GetStartupScripts() {
			return startupScripts.Select(f => f()).Where(s => !string.IsNullOrEmpty(s));
		}
		
		public string GetUniqueId() {
			return "id" + Utils.ToStringInvariantInt(nextUniqueId++);
		}
		
		public void Setup() {
			earlyAdditionalIncludes.AddRange(addScriptsBeforeCoreScripts);
			earlyAdditionalIncludes.AddRange((debugScripts ? Resources.CoreScriptsDebug : Resources.CoreScriptsRelease).Select(s => Routes.GetAssemblyResourceUrl(typeof(Resources).Assembly, s)));
			earlyAdditionalIncludes.AddRange(addScriptsBeforeAssemblyScripts);
			lateAdditionalIncludes.AddRange(addScriptsAfterAssemblyScripts);
		
			AddStartupScript(() => "if (typeof(Saltarelle) != 'undefined' && !Saltarelle.GlobalServices.hasService(" + typeof(IScriptManagerService) + ")) Saltarelle.GlobalServices.setService(" + typeof(IScriptManagerService).FullName + ", new " + typeof(DefaultScriptManagerProvider).FullName + "(" + Utils.ToStringInvariantInt(nextUniqueId) + "));");
		}
		
		public void Dispose() {
		}
	}
#endif
#if CLIENT
	public class DefaultScriptManagerProvider : IScriptManagerService {
		private int nextUniqueId;
		private ArrayList includedScripts;
		
		public DefaultScriptManagerProvider(int nextUniqueId) {
			this.nextUniqueId = nextUniqueId;

			includedScripts = new ArrayList();
			JQueryProxy.jQuery("script").each(delegate(int _, DOMElement el) {
				string s = ((ScriptElement)el).Src;
				if (s != null) {
					int ix = s.IndexOf("://");
					includedScripts.Add(ix != -1 ? s.Substr(s.IndexOf("/", ix + 3)) : s); // IE6 seems to return script paths relative, others return it as absolute
				}
				return true;
			});
		}
	
		public void EnsureScriptIncluded(string relativeUrl) {
			if (!includedScripts.Contains(relativeUrl)) {
				includedScripts.Add(relativeUrl);
				jQuery.ajax(new Dictionary("url", relativeUrl, "async", false, "cache", true, "dataType", "script"));
			}
		}
		
		public string GetUniqueId() {
			return "id" + Utils.ToStringInvariantInt(nextUniqueId++);
		}
	}
#endif
}
