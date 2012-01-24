using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Saltarelle.Mvc {
	public static class HtmlExtensions {
		public static void Scripts(this HtmlHelper helper) {
			var writer = helper.ViewContext.HttpContext.Response.Output;

			var sm = GlobalServices.GetService<IScriptManagerService>();
			
			sm.ExecuteBeforeRenderCallbacks();

			foreach (var script in sm.GetAllRequiredIncludes()) {
				writer.WriteLine("<script language=\"javascript\" type=\"text/javascript\" src=\"" + script + "\"></script>");
			}

			writer.WriteLine("<script language=\"javascript\" type=\"text/javascript\">");
			writer.WriteLine("$(function() {");
			foreach (var s in sm.GetStartupScripts())
				writer.WriteLine("\t" + s);

			writer.WriteLine("\tif (typeof(init) == 'function') init();");
			writer.WriteLine("});");
			writer.WriteLine("</script>");
		}
	}
}
