using System;
using Saltarelle;
#if SERVER
using System.Web.Mvc;
#endif

namespace DemoWeb {
	public interface ILesson7Service {
#if SERVER
		[AcceptVerbs(HttpVerbs.Post)]
		ControlDocumentFragment CreateGrid(int numRows);
#endif
#if CLIENT
		void AsyncCreateGrid(int numRows, Action<ControlDocumentFragment> success, Action failure);
#endif
	}
}
