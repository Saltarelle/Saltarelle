using System;
using Saltarelle.Ioc;
using System.Runtime.CompilerServices;
#if SERVER
using System.Linq;
#endif
#if CLIENT
using System.Html;
using jQueryApi;

#endif

namespace Saltarelle {
	[Record]
#if SERVER
	[Serializable]
#endif
	public sealed class ControlDocumentFragment {
		public string[] scriptReferences;
		public ScriptManagerConfig scriptManagerConfig;
		public string controlType;
		public string html;
		public object configObject;

#if SERVER
        /// <summary>
        /// Usually not used. Use <see cref="IScriptManagerService.CreateControlDocumentFragment"/> instead.
        /// </summary>
	    public ControlDocumentFragment(string[] scriptReferences, ScriptManagerConfig scriptManagerConfig, string controlType, string html, object configObject) {
	        this.scriptReferences = scriptReferences;
	        this.scriptManagerConfig = scriptManagerConfig;
	        this.controlType = controlType;
	        this.html = html;
	        this.configObject = configObject;
	    }
#endif
	}

#if CLIENT
	public static class DocumentFragmentHelper {
		public static void PrepareForInject(ControlDocumentFragment f) {
			for (int i = 0; i < f.scriptReferences.Length; i++)
				GlobalServices.ScriptManager.EnsureScriptIncluded(f.scriptReferences[i]);
            GlobalServices.Initialize(f.scriptManagerConfig);
		}
	
		public static IControl Inject(ControlDocumentFragment f, string newId, IContainer container, Element parent) {
			PrepareForInject(f);
			jQuery.FromElement(parent).Html(f.html);

			IControl control = (IControl)container.CreateObjectByTypeNameWithConstructorArg(f.controlType, f.configObject);
			control.Id = newId;
			
			return control;
		}
	}
#endif
}