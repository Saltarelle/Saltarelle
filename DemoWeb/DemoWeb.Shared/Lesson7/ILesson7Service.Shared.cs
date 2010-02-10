using System;
using Saltarelle;
#if SERVER
using System.Web.Mvc;
#endif

namespace DemoWeb {
#if CLIENT
	public delegate void CreateGridSuccessDelegate(ControlDocumentFragment f);
#endif

	public interface ILesson7Service {
#if SERVER
		[AcceptVerbs(HttpVerbs.Post)]
		ControlDocumentFragment CreateGrid(int numRows);
#endif
#if CLIENT
		void AsyncCreateGrid(int numRows, CreateGridSuccessDelegate success, Callback failure);
#endif
	}
}
