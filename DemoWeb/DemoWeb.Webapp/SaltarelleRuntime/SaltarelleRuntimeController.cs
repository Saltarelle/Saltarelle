using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.UI;
using Saltarelle;

namespace DemoWeb.Webapp.Controllers {
	[OutputCache(Duration=int.MaxValue, Location=OutputCacheLocation.None, VaryByParam="*")]
	public class SaltarelleRuntimeController : Controller {
		public ActionResult AssemblyScript(string assemblyName) {
			return JavaScript(GlobalServices.GetService<IScriptManagerService>().GetAssemblyScriptContent(AppDomain.CurrentDomain.GetAssemblies().Where(asm => asm.GetName().Name == assemblyName).SingleOrDefault(), true));
		}
	}
}
