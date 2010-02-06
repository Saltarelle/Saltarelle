using System;
#if SERVER
using System.Reflection;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Web;
using System.IO;
using System.Linq;
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
	
		private static object padlock = new object();
		
		private HashSet<Assembly> registeredAssemblies = new HashSet<Assembly>();
		private List<string> earlyAdditionalIncludes = new List<string>();
		private List<string> lateAdditionalIncludes = new List<string>();
		private List<Func<string>> startupScripts = new List<Func<string>>();
		
		private static Dictionary<Assembly, ReadOnlyCollection<Assembly>> assemblyDependencies = new Dictionary<Assembly, ReadOnlyCollection<Assembly>>();
		private static Dictionary<Assembly, string> assemblyScripts = new Dictionary<Assembly, string>();
		private static IList<Assembly> GetDependencies(Assembly asm) {
			lock (padlock) {
				ReadOnlyCollection<Assembly> result;
				if (!assemblyDependencies.TryGetValue(asm, out result)) {
					var l = new List<Assembly>();
					foreach (var refName in asm.GetReferencedAssemblies()) {
						Assembly refAsm = Assembly.Load(refName);
						if (!assemblyScripts.ContainsKey(refAsm))
							assemblyScripts[refAsm] = GetAssemblyScriptAssumingLock(refAsm);
						if (assemblyScripts[refAsm] != null)
							l.Add(refAsm);
					}
					assemblyDependencies[asm] = result = l.AsReadOnly();
				}
				return result;
			}
		}
		
		private static string GetAssemblyScriptAssumingLock(Assembly asm) {
			string scriptName = asm.GetManifestResourceNames().FirstOrDefault(s => s.EndsWith("Script.js"));
			if (scriptName != null) {
				using (var strm = asm.GetManifestResourceStream(scriptName))
				using (var rdr = new StreamReader(strm)) {
					return rdr.ReadToEnd();
				}
			}
			return null;
		}
		
		private string InternalGetAssemblyScriptContent(Assembly asm) {
			string s;
			lock (padlock) {
				if (!assemblyScripts.TryGetValue(asm, out s)) {
					assemblyScripts[asm] = s = GetAssemblyScriptAssumingLock(asm);
				}
			}
			return s;
		}

		private static string GetAssemblyIdentifier(Assembly a) {
			return Path.GetFileNameWithoutExtension(a.GetName().Name);
		}

		public void RegisterType(Type type) {
			registeredAssemblies.Add(type.Assembly);
		}
		
		private void AddAssembliesInCorrectOrder(Assembly asm, IList<Assembly> l) {
			if (!l.Contains(asm)) {
				// add all references (recursively) before adding the current one
				foreach (var dep in GetDependencies(asm))
					AddAssembliesInCorrectOrder(dep, l);
				if (InternalGetAssemblyScriptContent(asm) != null)
					l.Add(asm);
			}
		}
		
		public IEnumerable<string> GetAllRequiredIncludes() {
			IList<Assembly> asms = new List<Assembly>();
			foreach (Assembly a in registeredAssemblies.Concat(GlobalServices.AllLoadedServices.Select(kvp => kvp.Value.GetType().Assembly)))
				AddAssembliesInCorrectOrder(a, asms);
			return earlyAdditionalIncludes.Concat(asms.Select(a => urlService.GetAssemblyScriptPath(a))).Concat(lateAdditionalIncludes);
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
		
		public string GetAssemblyScriptContent(Assembly asm) {
			string s = InternalGetAssemblyScriptContent(asm);
			if (s == null)
				throw new HttpException(404, "File not found");
			return s;
		}
		
		public IEnumerable<string> GetStartupScripts() {
			return startupScripts.Select(f => f()).Where(s => !string.IsNullOrEmpty(s));
		}
		
		public string GetUniqueId() {
			return "id" + Utils.ToStringInvariantInt(nextUniqueId++);
		}
		
		public void Setup() {
			urlService = GlobalServices.Provider.GetService<IUrlService>();

			earlyAdditionalIncludes.Add(urlService.GetCoreScriptPath("sscompat.debug.js"));
			earlyAdditionalIncludes.Add(urlService.GetCoreScriptPath("sscorlib.debug.js"));
			earlyAdditionalIncludes.Add(urlService.GetCoreScriptPath("jquery-1.4.js"));
			earlyAdditionalIncludes.Add(urlService.GetCoreScriptPath("jquery-ui-1.7.2.js"));
			earlyAdditionalIncludes.Add(urlService.GetCoreScriptPath("jquery.focus.js"));
			earlyAdditionalIncludes.Add(urlService.GetCoreScriptPath("JQuerySharp.js"));
			earlyAdditionalIncludes.Add(urlService.GetCoreScriptPath("jquery.json-1.3.js"));
			earlyAdditionalIncludes.Add(urlService.GetCoreScriptPath("jquery.bgiframe.js"));
			earlyAdditionalIncludes.Add(urlService.GetCoreScriptPath("date.js"));

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
