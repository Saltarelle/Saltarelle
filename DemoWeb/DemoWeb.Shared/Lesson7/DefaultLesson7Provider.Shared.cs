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
		}
		
		public object ConfigObject {
			get {
				return new { urlTemplate = Routes.GetDelegateUrlTemplate(typeof(ILesson7Service)) };
			}
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
		public DefaultLesson7Provider(object config) {
			Dictionary cfg = Dictionary.GetDictionary(config);
			this.urlTemplate = (string)cfg["urlTemplate"];
		}

		public void AsyncCreateGrid(int numRows, CreateGridSuccessDelegate success, Callback failure) {
			Utils.Ajax(new Dictionary("numRows", numRows), string.Format(urlTemplate, "CreateGrid"), true, success, failure);
		}
	}
#endif
}
