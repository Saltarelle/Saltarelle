using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Saltarelle {
    public interface IRouteService {
		string GetAssemblyResourceUrl(Assembly asm, string resourcePublicName);
		string GetAssemblyScriptUrl(Assembly asm);
		string GetAssemblyCssUrl(Assembly asm);
		string GetDelegateUrl(MethodInfo mi);
		string GetDelegateUrlTemplate(Type type);
		string GetDelegateUrl<T>(Expression<Func<T>> method);

		string GetVirtualPath(string routeName, IDictionary<string, object> routeValues);
		string GetVirtualPath(string routeName);
    }
}
