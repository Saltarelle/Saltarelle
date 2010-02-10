using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Saltarelle.Mvc;
using System.Linq.Expressions;
using Saltarelle;

namespace DemoWeb.Webapp.Controllers {
	[HandleError]
	public class HomeController : Controller {
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
	}
}
