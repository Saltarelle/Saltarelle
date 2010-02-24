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
#endif
#if CLIENT
using System.DHTML;
#endif

namespace Saltarelle {
#if SERVER
	[GlobalService(typeof(IScriptManagerService))]
	public class DefaultScriptManagerProvider : IScriptManagerService, IGlobalService {
		private int nextUniqueId = 1;
		private IUrlService urlService;
	
		private HashSet<Assembly> registeredAssemblies = new HashSet<Assembly>();
		private List<string> earlyAdditionalIncludes = new List<string>();
		private List<string> lateAdditionalIncludes = new List<string>();
		private List<Func<string>> startupScripts = new List<Func<string>>();

		public void RegisterType(Type type) {
			registeredAssemblies.Add(type.Assembly);
		}

		public IEnumerable<string> GetAllRequiredIncludes() {
			var asms = registeredAssemblies.Concat(GlobalServices.AllLoadedServices.Select(kvp => kvp.Value.GetType().Assembly));
			return earlyAdditionalIncludes.Concat(ModuleUtils.TopologicalSortAssembliesWithDependencies(asms).Select(a => urlService.GetAssemblyScriptUrl(a))).Concat(lateAdditionalIncludes);
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
			urlService = GlobalServices.Provider.GetService<IUrlService>();

			earlyAdditionalIncludes.Add(urlService.GetCoreScriptUrl("sscompat.debug.js"));
			earlyAdditionalIncludes.Add(urlService.GetCoreScriptUrl("sscorlib.debug.js"));
			earlyAdditionalIncludes.Add(urlService.GetCoreScriptUrl("jquery-1.4.js"));
			earlyAdditionalIncludes.Add(urlService.GetCoreScriptUrl("jquery-ui-1.7.2.js"));
			earlyAdditionalIncludes.Add(urlService.GetCoreScriptUrl("jquery.focus.js"));
			earlyAdditionalIncludes.Add(urlService.GetCoreScriptUrl("JQuerySharp.js"));
			earlyAdditionalIncludes.Add(urlService.GetCoreScriptUrl("jquery.json-1.3.js"));
			earlyAdditionalIncludes.Add(urlService.GetCoreScriptUrl("jquery.bgiframe.js"));
			earlyAdditionalIncludes.Add(urlService.GetCoreScriptUrl("date.js"));

			AddStartupScript(() => "if (typeof(Saltarelle) != 'undefined' && !Saltarelle.GlobalServices.hasService(" + typeof(IScriptManagerService) + ")) Saltarelle.GlobalServices.setService(" + typeof(IScriptManagerService).FullName + ", new " + typeof(DefaultScriptManagerProvider).FullName + "(" + Utils.ToStringInvariantInt(nextUniqueId) + "));");
			AddStartupScript(() => "if (typeof(Saltarelle) != 'undefined' && typeof(Saltarelle.Utils) != 'undefined' && Saltarelle.Utils.blankImageUrl == null) Saltarelle.Utils.blankImageUrl = " + Utils.ScriptStr(GlobalServices.GetService<IUrlService>().BlankImageUrl) + ";");
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
