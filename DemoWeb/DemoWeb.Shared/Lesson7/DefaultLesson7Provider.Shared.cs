using System;
using Saltarelle;
using Saltarelle.UI;
#if SERVER
using Saltarelle.Mvc;
using System.Linq;
#endif

namespace DemoWeb {
#if SERVER
	[GlobalService(typeof(ILesson7Service))]
	public class DefaultLesson7Provider : ILesson7Service, IGlobalService {
		public void Setup() {
			var sm = GlobalServices.Provider.GetService<IScriptManagerService>();
			sm.AddStartupScript("if (!" + typeof(GlobalServices).FullName + ".hasService(" + typeof(ILesson7Service).FullName + ")) " + typeof(GlobalServices).FullName + ".setService(" + typeof(ILesson7Service).FullName + ", new " + typeof(DefaultLesson7Provider).FullName + "(" + Utils.ScriptStr(GlobalServices.GetService<IUrlService>().GetNonControllerActionUrlTemplate(typeof(ILesson7Service))) + "));");
		}

		public ControlDocumentFragment CreateGrid(int numRows) {
			const int numCols = 5;
			var grid = new Grid();
			grid.ColTitles = Enumerable.Range(1, numCols).Select(col => "Column " + col).ToArray();
			for (int row = 1; row <= numRows; row++) {
				grid.AddItem(Enumerable.Range(1, numCols).Select(col => "Cell " + row + ", " + col).ToArray(), null);
			}
			return new ControlDocumentFragment(grid);
		}
	}
#endif

#if CLIENT
	public class DefaultLesson7Provider : ILesson7Service {
		private string urlTemplate;
		public DefaultLesson7Provider(string urlTemplate) {
			this.urlTemplate = urlTemplate;
		}

		public void AsyncCreateGrid(int numRows, CreateGridSuccessDelegate success, Callback failure) {
			Utils.Ajax(new Dictionary("numRows", numRows), string.Format(urlTemplate, "CreateGrid"), true, success, failure);
		}
	}
#endif
}
