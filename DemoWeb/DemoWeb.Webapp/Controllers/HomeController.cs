using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Saltarelle.Mvc;
using System.Linq.Expressions;
using Saltarelle;
using System.Reflection;
using System.IO;

namespace DemoWeb.Webapp.Controllers {
	[HandleError]
	public class HomeController : Controller {
		private static string allModulesCss;

		static HomeController() {
			Assembly[] asms = AppDomain.CurrentDomain.GetAssemblies();
			allModulesCss = string.Join(Environment.NewLine, asms.Select(x => ModuleUtils.GetAssemblyCss(x)).Where(s => !Utils.IsNull(s)).ToArray());
		}

		public ActionResult Lesson1() {
			return View();
		}

		public ActionResult Lesson4() {
			return View();
		}

		public ActionResult Lesson5() {
			return View();
		}

		public ActionResult Lesson7() {
			return View();
		}
		
		public ActionResult TreeTest() {
			return View();
		}
		
		public ActionResult GridTest() {
			return View();
		}

		public ActionResult DialogTest() {
			return View();
		}

		public ActionResult Stylesheet() {
			return Content(allModulesCss, "text/css");
		}
	}
}
