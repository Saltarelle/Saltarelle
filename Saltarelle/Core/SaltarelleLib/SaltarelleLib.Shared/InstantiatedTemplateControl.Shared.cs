using System;
#if CLIENT
using ControlDictionary = System.Dictionary;
using ControlEntry      = System.DictionaryEntry;
using StringList        = System.ArrayList;
#endif
#if SERVER
using ControlDictionary = System.Collections.Generic.Dictionary<string, Saltarelle.IControl>;
using ControlEntry      = System.Collections.Generic.KeyValuePair<string, Saltarelle.IControl>;
using StringList        = System.Collections.Generic.List<string>;
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
			jQuery GetNamedElement(string name);
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
			private jQuery element;
		#endif
		
		public void AddControl(string name, IControl control) {
			controls[name] = control;
			if (id != null)
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
					return element != null ? PositionHelper.GetPosition(element) : position;
				#else
					return position;
				#endif
			}
			set {
				position = value;
				#if CLIENT
					if (element != null)
						PositionHelper.ApplyPosition(element, value);
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
					if (element != null) {
						foreach (string s in namedElements)
							GetNamedElement(s).attr("id", value + "_" + s);
					}
				#endif
			}
		}

		public string Html {
			get {
				if (string.IsNullOrEmpty(id))
					throw new Exception("Must set ID before render");
				if (getHtml == null)
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
#endif

#if CLIENT
		public jQuery Element { get { return element; } }

		[AlternateSignature]
		public extern InstantiatedTemplateControl(InstantiatedTemplateControlGetHtmlDelegate getHtml);

		public InstantiatedTemplateControl(string id) {
			if (Type.GetScriptType(id) == "string") {
				this.id = id;
				this.element = JQueryProxy.jQuery("#" + id);
				Dictionary config = (Dictionary)Utils.EvalJson((string)element.attr("__cfg"));
				Dictionary controlTypes = (Dictionary)config["controlTypes"];
				foreach (DictionaryEntry de in controlTypes) {
					Type tp = Utils.FindType((string)de.Value);
					this.controls[de.Key] = Type.CreateInstance(tp, id + "_" + de.Key);
				}
				this.namedElements = (StringList)config["namedElements"];
			}
			else {
				this.getHtml       = (InstantiatedTemplateControlGetHtmlDelegate)(object)id;
				this.namedElements = new StringList();
			}
		}

		public jQuery GetNamedElement(string name) {
			if (element == null)
				throw new Exception("Must attach first");
			return JQueryProxy.jQuery("#" + id + "_" + name);
		}
		
		public void Attach() {
			foreach (DictionaryEntry de in controls) {
				IClientCreateControl cc = (de.Value as IClientCreateControl);
				if (cc == null)
					throw new Exception("The control " + de.Key + " does not implement IClientCreateControl.");
				cc.Attach();
			}
		}
#endif
	}
}
