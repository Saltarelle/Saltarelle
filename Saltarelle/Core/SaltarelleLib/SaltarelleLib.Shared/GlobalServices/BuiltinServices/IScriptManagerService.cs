using System;
#if SERVER
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
#endif

namespace Saltarelle {
	public interface IScriptManagerService {
		string GetUniqueId();

#if SERVER
		void RegisterType(Type type);
		void AddScriptInclude(string url, bool includeBeforeAssemblyScripts);
		
		string GetAssemblyScriptContent(Assembly assemblyName);
		
		IEnumerable<string> GetAllRequiredIncludes();
		
		void RegisterTopLevelControl(IControl control);
		void AddStartupScript(Func<string> scriptRetriever);
		void AddStartupScript(string script);
		IEnumerable<string> GetStartupScripts();
#endif
#if CLIENT
		void EnsureScriptIncluded(string relativeUrl);
#endif
	}
}
