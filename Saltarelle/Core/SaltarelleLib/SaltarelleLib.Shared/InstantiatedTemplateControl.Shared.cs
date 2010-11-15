using System;
#if CLIENT
using ControlDictionary = System.Dictionary;
using ControlEntry      = System.DictionaryEntry;
using StringList        = System.ArrayList;
using System.DHTML;
#endif
#if SERVER
using ControlDictionary = System.Collections.Generic.Dictionary<string, Saltarelle.IControl>;
using ControlEntry      = System.Collections.Generic.KeyValuePair<string, Saltarelle.IControl>;
using StringList        = System.Collections.Generic.List<string>;
using System.Collections.Generic;
#endif
namespace Saltarelle {
	public interface IInstantiatedTemplateControl
	#if SERVER
		: IControl
	#endif
	{
		StringList NamedElementNames { get; }
		ControlDictionary Controls { get; }

		void AddControl(string name, IControl control);
		void AddNamedElement(string name);
		#if CLIENT
			DOMElement GetNamedElement(string name);
		#endif
	}

	public delegate string InstantiatedTemplateControlGetHtmlDelegate(IInstantiatedTemplateControl ctl);

	public sealed class InstantiatedTemplateControl : IInstantiatedTemplateControl, IControl
	#if CLIENT
		, IClientCreateControl
	#endif
	{
		private string id;
		private Position position = PositionHelper.NotPositioned;
		private ControlDictionary controls = new ControlDictionary();
		private StringList namedElements;
		private InstantiatedTemplateControlGetHtmlDelegate getHtml;
		
		#if CLIENT
			private bool isAttached = false;
		#endif
		
		public void AddControl(string name, IControl control) {
			controls[name] = control;
			if (!Utils.IsNull(id))
				control.Id = id + "_" + name;
		}
		
		public void AddNamedElement(string name) {
			namedElements.Add(name);
		}

		public ControlDictionary Controls {
			get { return controls; }
		}

		public StringList NamedElementNames {
			get { return namedElements; }
		}

		public Position Position {
			get {
				#if CLIENT
					return isAttached ? PositionHelper.GetPosition(GetElement()) : position;
				#else
					return position;
				#endif
			}
			set {
				position = value;
				#if CLIENT
					if (isAttached)
						PositionHelper.ApplyPosition(GetElement(), value);
				#endif
			}
		}

		public string Id {
			get { return id; }
			set {
				id = value;
				foreach (ControlEntry kvp in controls)
					((IControl)kvp.Value).Id = value + "_" + kvp.Key;
				#if CLIENT
					if (isAttached) {
						foreach (string s in namedElements)
							GetNamedElement(s).ID = value + "_" + s;
					}
				#endif
			}
		}

		public string Html {
			get {
				if (string.IsNullOrEmpty(id))
					throw new Exception("Must set ID before render");
				if (Utils.IsNull(getHtml))
					throw new Exception("This control was created server-side");
				return getHtml(this);
			}
		}

#if SERVER
		public InstantiatedTemplateControl(InstantiatedTemplateControlGetHtmlDelegate getHtml) {
			GlobalServices.GetService<IScriptManagerService>().RegisterType(GetType());
			this.getHtml       = getHtml;
			this.namedElements = new StringList();
		}
		
		public object ConfigObject {
			get {
				if (string.IsNullOrEmpty(id))
					throw new Exception("Must set ID before render");
				var controlConfig = new Dictionary<string, object>();
				foreach (var c in controls)
					controlConfig[c.Key] = new { type = c.Value.GetType(), cfg = c.Value.ConfigObject };
				return new { id, controls = controlConfig, namedElements };
			}
		}
#endif

#if CLIENT
		public DOMElement GetElement() { return isAttached ? Document.GetElementById(id) : null; }

		[AlternateSignature]
		public extern InstantiatedTemplateControl(InstantiatedTemplateControlGetHtmlDelegate getHtml);
		
		public InstantiatedTemplateControl(object configObject) {
			if (Type.GetScriptType(id) == "string") {
				Dictionary dict = Dictionary.GetDictionary(configObject);
				this.id = (string)dict["id"];
				Dictionary controlConfig = (Dictionary)dict["controls"];
				foreach (DictionaryEntry de in controlConfig) {
					Dictionary config = (Dictionary)de.Value;
					Type tp = Utils.FindType((string)config["type"]);
					this.controls[de.Key] = Type.CreateInstance(tp, config["cfg"]);
				}
				this.namedElements = (StringList)dict["namedElements"];
				isAttached = true;
			}
			else {
				this.getHtml       = (InstantiatedTemplateControlGetHtmlDelegate)(object)id;
				this.namedElements = new StringList();
			}
		}

		public DOMElement GetNamedElement(string name) {
			if (!isAttached)
				throw new Exception("Must attach first");
			return Document.GetElementById(id + "_" + name);
		}
		
		public void Attach() {
			foreach (DictionaryEntry de in controls) {
				IClientCreateControl cc = (de.Value as IClientCreateControl);
				if (Utils.IsNull(cc))
					throw new Exception("The control " + de.Key + " does not implement IClientCreateControl.");
				cc.Attach();
			}
			isAttached = true;
		}
#endif
	}
}
