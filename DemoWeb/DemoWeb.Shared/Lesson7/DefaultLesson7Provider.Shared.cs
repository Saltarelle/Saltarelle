using System;
using System.Collections;
using Saltarelle;
using Saltarelle.Ioc;
using Saltarelle.UI;
#if CLIENT
using jQueryApi;
#endif

#if SERVER
using Saltarelle.Mvc;
using System.Linq;
#endif

namespace DemoWeb {
#if SERVER
	public class DefaultLesson7Provider : ILesson7Service {
		private readonly IContainer _container;
		private readonly IScriptManagerService _scriptManager;
		private readonly IRouteService _routes;

		public DefaultLesson7Provider(IContainer container, IScriptManagerService scriptManager, IRouteService routes)
		{
			_container = container;
			_scriptManager = scriptManager;
			_routes = routes;
		}

		public object ConfigObject {
			get {
				return new { urlTemplate = _routes.GetDelegateUrlTemplate(typeof(ILesson7Service)) };
			}
		}

		public ControlDocumentFragment CreateGrid(int numRows) {
			const int numCols = 5;
			var grid = _container.CreateObject<Grid>();
			grid.ColTitles = Enumerable.Range(1, numCols).Select(col => "Column " + col).ToArray();
			for (int row = 1; row <= numRows; row++) {
				grid.AddItem(Enumerable.Range(1, numCols).Select(col => "Cell " + row + ", " + col).ToArray(), null);
			}
			return _scriptManager.CreateControlDocumentFragment(_container, grid);
		}
	}
#endif

#if CLIENT
	public class DefaultLesson7Provider : ILesson7Service {
		private string urlTemplate;
		public DefaultLesson7Provider(object config) {
			var cfg = JsDictionary.GetDictionary(config);
			this.urlTemplate = (string)cfg["urlTemplate"];
		}

		public void AsyncCreateGrid(int numRows, Action<ControlDocumentFragment> success, Action failure) {
			Utils.Ajax(JsDictionary.GetDictionary(new { numRows }), string.Format(urlTemplate, "CreateGrid"), true, (o, _1, _2) => success((ControlDocumentFragment)o), (_1, _2, _3) => failure());
		}
	}
#endif
}
