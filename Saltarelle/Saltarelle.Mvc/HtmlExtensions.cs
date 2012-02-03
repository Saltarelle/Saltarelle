using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Saltarelle.Ioc;

namespace Saltarelle.Mvc {
	public static class HtmlExtensions {
		public static void Scripts(this HtmlHelper helper) {
            var sm = DependencyResolver.Current.GetService<IScriptManagerService>();
            var c  = DependencyResolver.Current.GetService<IContainer>();
            c.ApplyToScriptManager(sm);

            helper.ViewContext.Writer.Write(sm.GetMarkup().ToString());
		}
	}
}
