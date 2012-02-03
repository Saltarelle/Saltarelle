using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.IO;
using System.Linq;
using Saltarelle.Ioc;
using Saltarelle.Mvc;
using Saltarelle.Mvc.CoreServiceImplementations;
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
	
        private static readonly ConcurrentDictionary<Type, List<Tuple<string, Type>>> _propertiesToInjectCache = new ConcurrentDictionary<Type, List<Tuple<string, Type>>>();

		private HashSet<Assembly>          registeredAssemblies    = new HashSet<Assembly>();
		private Dictionary<Type, IService> registeredServices      = new Dictionary<Type, IService>();
		private List<string>               earlyAdditionalIncludes = new List<string>();
		private List<string>               lateAdditionalIncludes  = new List<string>();
		private List<IControl>             topLevelControls        = new List<IControl>();

		public string GetUniqueId() {
			return "id" + Utils.ToStringInvariantInt(nextUniqueId++);
		}
		
		public IControl GetTopLevelControl(string id) {
			return topLevelControls.SingleOrDefault(c => c.Id == id);
		}

		public void RegisterTopLevelControl(IControl control) {
            if (!string.IsNullOrEmpty(control.Id) && GetTopLevelControl(control.Id) != null)
                throw new ArgumentException("A control with the id " + control.Id + " already exists.", "control");
            topLevelControls.Add(control);
		}

        public void RegisterClientService(Type serviceType, IService implementer) {
            if (!serviceType.IsInterface || serviceType == typeof(IService))
                throw new InvalidOperationException("Transferred services must be interfaces, and must not be the IService interface (tried to register type " + serviceType.FullName + ").");
            if (registeredServices.ContainsKey(serviceType))
                throw new InvalidOperationException("An instance has already been registered for the service " + serviceType.FullName);
            registeredServices.Add(serviceType, implementer);
        }

		public void RegisterClientAssembly(Assembly asm) {
			registeredAssemblies.Add(asm);
		}

		public void AddScriptInclude(string url, bool includeBeforeAssemblyScripts) {
			List<string> l = (includeBeforeAssemblyScripts ? earlyAdditionalIncludes : lateAdditionalIncludes);
			if (!l.Contains(url))
				l.Add(url);
		}

        public IEnumerable<string> GetAllRequiredIncludes() {
            return earlyAdditionalIncludes.Concat(ModuleUtils.TopologicalSortAssembliesWithDependencies(registeredAssemblies).Select(a => Routes.GetAssemblyScriptUrl(a))).Concat(lateAdditionalIncludes);
        }

        public IHtmlString GetMarkup() {
            var sb = new StringBuilder();

			foreach (var script in GetAllRequiredIncludes()) {
				sb.AppendLine("<script language=\"javascript\" type=\"text/javascript\" src=\"" + script + "\"></script>");
			}

			sb.AppendLine("<script language=\"javascript\" type=\"text/javascript\">");
			sb.AppendLine("$(function() {");
				sb.AppendLine("\tSaltarelle.GlobalServices.initialize(" + Utils.InitScript(ConfigObject) + ");");

			sb.AppendLine("});");
			sb.AppendLine("</script>");

            return new HtmlString(sb.ToString());
        }

        public ControlDocumentFragment CreateControlDocumentFragment(IContainer container, IControl control) {
			control.Id = Guid.NewGuid().ToString().Replace("-", "");
            container.ApplyToScriptManager(this);
            return new ControlDocumentFragment(GetAllRequiredIncludes().ToArray(), ConfigObject, control.GetType().FullName, control.Html, control.ConfigObject);
        }

		object IService.ConfigObject {
            get { return this.ConfigObject; }
        }

		public ScriptManagerConfig ConfigObject {
			get {
                return new ScriptManagerConfig {
                    nextUniqueId = nextUniqueId,
                    injections = (  from a in registeredAssemblies
                                    from t in a.GetTypes()
                                     let l = Helpers.FindPropertiesToInject(t)
                                   where l.Count > 0
                                  select new { t.FullName, rows = (IList<ScriptManagerConfigInjectedPropertyRow>)l.Select(x => new ScriptManagerConfigInjectedPropertyRow { propertyName = x.Item1, typeName = x.Item2.FullName }).ToList() }
                                 ).ToDictionary(x => x.FullName, x => x.rows),
                    services = registeredServices.ToDictionary(s => s.Key.FullName, s => new ScriptManagerConfigServiceEntry { type = s.Value.GetType().FullName, config = s.Value.ConfigObject }),
                    controls = topLevelControls.Select(c => new ScriptManagerConfigControlRow { type = c.GetType().FullName, config = c.ConfigObject }).ToArray()
                };
            }
		}

	    public DefaultScriptManagerService() {
			earlyAdditionalIncludes.AddRange(addScriptsBeforeCoreScripts);
			earlyAdditionalIncludes.AddRange((debugScripts ? Resources.CoreScriptsDebug : Resources.CoreScriptsRelease).Select(s => Routes.GetAssemblyResourceUrl(typeof(Resources).Assembly, s)));
			earlyAdditionalIncludes.AddRange(addScriptsBeforeAssemblyScripts);
			lateAdditionalIncludes.AddRange(addScriptsAfterAssemblyScripts);
	    }
	}
}
