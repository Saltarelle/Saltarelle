using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Saltarelle {
    public interface IModuleUtils {
        string GetAssemblyScriptContent(Assembly asm, bool debug);
        string GetAssemblyCss(Assembly assembly);
        string GetCss(string lessSource, Assembly contextAssembly);
        IList<Assembly> TopologicalSortAssembliesWithDependencies(IEnumerable<Assembly> list);
    }
}
