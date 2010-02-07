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
//		public ActionResult SimpleControl() {
//			string s = GlobalServices.GetService<IUrlService>().GetActionUrl((HomeController c) => c.ContainerControl);
//			s = GlobalServices.GetService<IUrlService>().GetActionUrl<HomeController, int>(c => c.SomethingElse);
//			return View();
//		}

		public ActionResult SimpleControl() {
			string s = GlobalServices.GetService<IUrlService>().GetTypedActionUrl((HomeController c) => c.ContainerControl);
			s = GlobalServices.GetService<IUrlService>().GetTypedActionUrl((HomeController c) => c.SomethingElse, Unspecified<int>.Value);
			s = GlobalServices.GetService<IUrlService>().GetTypedActionUrl((HomeController c) => c.SomethingElse, 0);
			return View();
		}

		public ActionResult ContainerControl() {
			return View();
		}

		public ActionResult SomethingElse(int x) {
			return View();
		}
	}
}
