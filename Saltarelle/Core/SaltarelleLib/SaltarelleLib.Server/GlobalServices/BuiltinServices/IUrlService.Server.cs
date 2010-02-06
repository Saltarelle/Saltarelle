using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Linq.Expressions;
using System.Web.Routing;
using System.Web.Mvc;

namespace Saltarelle {
	public interface IUrlService {
		string GetAssemblyScriptPath(Assembly asm);
		string GetCoreScriptPath(string script);
		string BlankImageUrl { get; }
		
		/// <summary>
		/// Retrieves the url for a specific action
		/// </summary>
		/// <param name="action">A method that denotes an action. This should be an instance method on a class that implements IController.</param>
		/// <param name="routeValues">Additional parameters</param>
		/// <returns>The Url</returns>
		string GetActionUrl(MethodInfo action, RouteValueDictionary routeValues);
	}
}
