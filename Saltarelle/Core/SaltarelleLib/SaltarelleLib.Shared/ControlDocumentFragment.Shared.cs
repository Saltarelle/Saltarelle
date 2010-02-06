using System;
using Saltarelle;
#if SERVER
using System.Linq;
#endif
#if CLIENT
using System.DHTML;
#endif

namespace Saltarelle {
	[Record]
	public sealed class ControlDocumentFragment {
		public string[] scriptReferences;
		public string[] startupScripts;
		public string controlType;
		public string id;
		public string html;
		
#if SERVER
		public ControlDocumentFragment(IControl control) {
			var sm = GlobalServices.Provider.GetService<IScriptManagerService>();
			this.id = control.Id = Guid.NewGuid().ToString().Replace("-", "");
			this.scriptReferences = sm.GetAllRequiredIncludes().ToArray();
			this.startupScripts = sm.GetStartupScripts().ToArray();
			this.html = control.Html;
			this.controlType = control.GetType().FullName;
		}
#endif
	}

#if CLIENT
	public static class DocumentFragmentHelper {
		public static IControl Inject(ControlDocumentFragment f, string newId, jQuery parent) {
			for (int i = 0; i < f.scriptReferences.Length; i++)
				((IScriptManagerService)GlobalServices.Provider.GetService(typeof(IScriptManagerService))).EnsureScriptIncluded(f.scriptReferences[i]);

			parent.html(f.html);
			foreach (string s in f.startupScripts)
				Script.Eval(s);

			Type tp = Utils.FindType(f.controlType);
			IControl control = (IControl)Type.CreateInstance(tp, f.id);
			control.Id = newId;
			
			return control;
		}
	}
#endif
}