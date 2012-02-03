using System;
using Saltarelle.Ioc;
#if SERVER
using System.Linq;
#endif
#if CLIENT
using System.DHTML;
#endif

namespace Saltarelle {
	[Record]
#if SERVER
	[Serializable]
#endif
	public sealed class ControlDocumentFragment {
		public string[] scriptReferences;
		public string[] startupScripts;
		public string controlType;
		public string html;
		public object configObject;
		
#if SERVER
#warning TODO: Fix this
		public ControlDocumentFragment(IControl control) {
#if false
			var sm = GlobalServices.Provider.GetService<IScriptManagerService>();
			control.Id = Guid.NewGuid().ToString().Replace("-", "");
			this.scriptReferences = sm.GetAllRequiredIncludes().ToArray();
			this.startupScripts = sm.GetStartupScripts().ToArray();
			this.html = control.Html;
			this.configObject = control.ConfigObject;
			this.controlType = control.GetType().FullName;
#endif
		}

#warning TODO: Fix client code below
#endif
	}

#if CLIENT
	public static class DocumentFragmentHelper {
		public static void PrepareForInject(ControlDocumentFragment f) {
//			for (int i = 0; i < f.scriptReferences.Length; i++)
//				((IScriptManagerService)GlobalServices.Provider.GetService(typeof(IScriptManagerService))).EnsureScriptIncluded(f.scriptReferences[i]);
			foreach (string s in f.startupScripts)
				Script.Eval(s);
		}
	
		public static IControl Inject(ControlDocumentFragment f, string newId, IContainer container, DOMElement parent) {
			PrepareForInject(f);
			JQueryProxy.jQuery(parent).html(f.html);

			IControl control = (IControl)container.ResolveByTypeNameWithConstructorArg(f.controlType, f.configObject);
			control.Id = newId;
			
			return control;
		}
	}
#endif
}