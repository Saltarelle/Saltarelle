using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Reflection;
using Saltarelle;
using System.Web.Routing;
using System.Web.Mvc;
using System.Web;

namespace DemoWeb.Webapp {
	[GlobalService(typeof(IUrlService))]
	public class DefaultUrlProvider : IUrlService {
		public string GetAssemblyScriptPath(Assembly asm) {
			var n = asm.GetName();
			return "/AssemblyScripts/" + n.Name + ".js" + (n.Version.Major > 0 || n.Version.Minor > 0 || n.Version.Build > 0 || n.Version.Revision > 0 ? "?v=" + n.Version.ToString() : "");
		}

		public string GetCoreScriptPath(string script) {
			return "/Scripts/" + script;
		}
		
		public string BlankImageUrl {
			get { return "/Content/blank.gif"; }
		}
		
		public string GetActionUrl(MethodInfo action, RouteValueDictionary routeValues) {
			string controllerName = action.DeclaringType.Name;
			controllerName = (controllerName.EndsWith("Controller") ? controllerName.Substring(0, controllerName.Length - 10) : controllerName);
			var all = new RouteValueDictionary(routeValues);
			all["controller"] = controllerName;
			all["action"] = action.Name;
			VirtualPathData p = RouteTable.Routes.GetVirtualPath(((MvcHandler)HttpContext.Current.Handler).RequestContext, all);
			return p != null ? p.VirtualPath : null;
		}
	}
}
