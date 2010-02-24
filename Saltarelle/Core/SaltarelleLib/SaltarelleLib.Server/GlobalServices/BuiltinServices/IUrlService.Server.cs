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
		/// <summary>
		/// Get the URL to an assembly script
		/// </summary>
		/// <param name="asm">Assembly of interest</param>
		/// <returns>The relative URL to the assembly script, relative to the server root</returns>
		string GetAssemblyScriptUrl(Assembly asm);

		/// <summary>
		/// Get the path to a core script (jquery, jquery-ui, etc.)
		/// </summary>
		/// <param name="script">Filename of the script</param>
		/// <returns>The URL to the script, relative to the server root</returns>
		string GetCoreScriptUrl(string script);

		/// <summary>
		/// Get the URL for an assembly resource.
		/// </summary>
		/// <param name="asm">Assembly containing the resource.</param>
		/// <param name="name">Name of the resource, as specified in the [Saltarelle.Mvc.WebResource] attribute.</param>
		/// <returns>The URL to the resource, relative to the server root.</returns>
		string GetAssemblyResourceUrl(Assembly asm, string name);

		/// <summary>
		/// Get the path to a 1x1 pixel, transparent image.
		/// </summary>
		/// <returns>The relative path to the image, relative to the server root</returns>
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
