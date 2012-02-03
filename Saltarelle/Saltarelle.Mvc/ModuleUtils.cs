using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using dotless.Core;
using dotless.Core.configuration;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection.Emit;
using Mono.Cecil;

namespace Saltarelle.Mvc {
	public class ModuleUtils : IModuleUtils {
	    private readonly IRouteService routes;
	    private object scriptLock = new object();
		private object cssLock    = new object();
		
		private IEnumerable<Assembly> GetClientReferencedAssembliesAssumingLock(Assembly asm) {
			AssemblyDefinition def = GetClientAssemblyAssumingLock(asm);
			if (def == null)
				return new Assembly[0];
				
			return (  from m in def.Modules.Cast<ModuleDefinition>()
			          from r in m.AssemblyReferences.Cast<AssemblyNameReference>()
			        select r
			       ).Distinct()	// If the assembly name does not end with .Client, we assume we don't have to handle the dependency. sscorlib is a prime example
			       .Where(x => x.Name.EndsWith(".Client"))
			       .Select(x => Assembly.Load(new AssemblyName(x.FullName) { Name = x.Name.Substring(0, x.Name.Length - 7) }));
		}

		private Dictionary<Assembly, ReadOnlyCollection<Assembly>> assemblyDependencies = new Dictionary<Assembly, ReadOnlyCollection<Assembly>>();
		private Dictionary<Assembly, string> debugAssemblyScripts = new Dictionary<Assembly, string>();
		private Dictionary<Assembly, string> releaseAssemblyScripts = new Dictionary<Assembly, string>();
		private Dictionary<Assembly, string> assemblyCss = new Dictionary<Assembly, string>();

		private IList<Assembly> GetClientDependencies(Assembly asm) {
			lock (scriptLock) {
				ReadOnlyCollection<Assembly> result;
				if (!assemblyDependencies.TryGetValue(asm, out result)) {
					var l = new List<Assembly>();
					foreach (var refAsm in GetClientReferencedAssembliesAssumingLock(asm)) {
						if (!debugAssemblyScripts.ContainsKey(refAsm)) {
							debugAssemblyScripts[refAsm] = GetAssemblyScriptAssumingLock(refAsm, true);
							releaseAssemblyScripts[refAsm] = GetAssemblyScriptAssumingLock(refAsm, false);
						}
						l.Add(refAsm);
					}
					assemblyDependencies[asm] = result = l.AsReadOnly();
				}
				return result;
			}
		}
		
		private AssemblyDefinition GetClientAssemblyAssumingLock(Assembly asm) {
			if (asm.IsDynamic)
				return null;
			string name = asm.GetManifestResourceNames().FirstOrDefault(s => s.EndsWith("Client.dll"));
			return !Utils.IsNull(name) ? AssemblyDefinition.ReadAssembly(asm.GetManifestResourceStream(name)) : null;
		}
		
		private string GetAssemblyScriptAssumingLock(Assembly asm, bool debug) {
			if (asm.IsDynamic)
				return null;
		
			string scriptName = asm.GetManifestResourceNames().FirstOrDefault(s => s.EndsWith(debug ? "Script.js" : "Script.min.js"));
			if (!Utils.IsNull(scriptName)) {
				using (var strm = asm.GetManifestResourceStream(scriptName))
				using (var rdr = new StreamReader(strm)) {
					return rdr.ReadToEnd();
				}
			}
			return null;
		}

		public string GetAssemblyScriptContent(Assembly asm, bool debug) {
			string s;
			lock (scriptLock) {
				var x = (debug ? debugAssemblyScripts : releaseAssemblyScripts);
				if (!x.TryGetValue(asm, out s)) {
					x[asm] = s = GetAssemblyScriptAssumingLock(asm, debug);
				}
			}
			return s;
		}

		private string GetLessVariableDefinitions(Assembly asm) {
			return string.Join(Environment.NewLine,
			                   asm.GetCustomAttributes(typeof(CssResourceAttribute), false)
			                      .Cast<CssResourceAttribute>()
			                      .Select(a => "@" + a.CssVariableName + ": url(" + routes.GetAssemblyResourceUrl(asm, a.PublicResourceName) + ");")
			           .Concat(asm.GetCustomAttributes(typeof(ImportCssResourceAttribute), false)
			                      .Cast<ImportCssResourceAttribute>()
			                      .Select(a => "@" + a.CssVariableName + ": url(" + routes.GetAssemblyResourceUrl(a.ResourceAssembly, a.PublicResourceName) + ");"))
			           .ToArray());
		}
		
		private string GenerateAssemblyCss(Assembly asm) {
			if (asm.IsDynamic)
				return null;

			string resName = asm.GetManifestResourceNames().FirstOrDefault(s => s.EndsWith("Module.less"));
			if (!Utils.IsNull(resName)) {
				using (var strm = asm.GetManifestResourceStream(resName))
				using (var rdr = new StreamReader(strm)) {
					return GetCss(rdr.ReadToEnd(), asm);
				}
			}
			return null;
		}
		
		public string GetAssemblyCss(Assembly assembly) {
			lock (cssLock) {
				string s;
				if (!assemblyCss.TryGetValue(assembly, out s)) {
					assemblyCss[assembly] = s = GenerateAssemblyCss(assembly);
				}
				return s;
			}
		}
		
		public string GetCss(string lessSource, Assembly contextAssembly) {
			var engine = new EngineFactory().GetEngine();
			return engine.TransformToCss((!Utils.IsNull(contextAssembly) ? GetLessVariableDefinitions(contextAssembly) + Environment.NewLine : "") + lessSource, "Module.less");
		}
		
		private void AddAssembliesInCorrectOrder(Assembly asm, IList<Assembly> l) {
			if (!l.Contains(asm)) {
				// add all references (recursively) before adding the current one
				foreach (var dep in GetClientDependencies(asm))
					AddAssembliesInCorrectOrder(dep, l);
				if (GetAssemblyScriptContent(asm, true) != null)
					l.Add(asm);
			}
		}
		
		public IList<Assembly> TopologicalSortAssembliesWithDependencies(IEnumerable<Assembly> list) {
			IList<Assembly> result = new List<Assembly>();
			foreach (Assembly a in list)
				AddAssembliesInCorrectOrder(a, result);
			return result;
		}

        public ModuleUtils(IRouteService routes) {
            this.routes = routes;
        }
	}
}
