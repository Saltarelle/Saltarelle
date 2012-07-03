using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Saltarelle.Ioc;
#if CLIENT
using System.Html;
using System.Collections;
#endif

namespace Saltarelle {
	public interface IInstantiatedTemplateControl
	#if SERVER
		: IControl
	#endif
	{
		IList<string> NamedElementNames { get; }
		IDictionary<string, IControl> Controls { get; }

		void AddControl(string name, IControl control);
		void AddNamedElement(string name);
		#if CLIENT
			Element GetNamedElement(string name);
		#endif
	}

	public delegate string InstantiatedTemplateControlGetHtmlDelegate(IInstantiatedTemplateControl ctl);

	public sealed class InstantiatedTemplateControl : IInstantiatedTemplateControl, IControl, INotifyCreated, IClientCreateControl
	{
		private string id;
		private Position position = PositionHelper.NotPositioned;
		private IDictionary<string, IControl> controls = new Dictionary<string, IControl>();
		private List<string> namedElements;
		private InstantiatedTemplateControlGetHtmlDelegate getHtml;

		private IContainer container;
		#if SERVER
		[ClientInject]
		#endif
		public IContainer Container { get { return container; } set { container = value; } }
		
		#if CLIENT
			private bool isAttached = false;
		#endif
		
		public void AddControl(string name, IControl control) {
			controls[name] = control;
			if (id != null)
				control.Id = id + "_" + name;
		}
		
		public void AddNamedElement(string name) {
			namedElements.Add(name);
		}

		public IDictionary<string, IControl> Controls {
			get { return controls; }
		}

		public IList<string> NamedElementNames {
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
				foreach (var kvp in controls)
					kvp.Value.Id = value + "_" + kvp.Key;
				#if CLIENT
					if (isAttached) {
						foreach (var s in namedElements)
							GetNamedElement(s).ID = value + "_" + s;
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
			this.getHtml       = getHtml;
			this.namedElements = new StringList();
		}
		
		public object ConfigObject {
			get {
				if (string.IsNullOrEmpty(id))
					throw new Exception("Must set ID before render");
				var controlConfig = new Dictionary<string, object>();
				foreach (var c in controls)
					controlConfig[c.Key] = new { type = c.Value.GetType().FullName, cfg = c.Value.ConfigObject };
				return new { id, controls = controlConfig, namedElements };
			}
		}

		public void DependenciesAvailable() {
		}
#endif

#if CLIENT
		public Element GetElement() { return isAttached ? Document.GetElementById(id) : null; }

		[AlternateSignature]
		public extern InstantiatedTemplateControl(InstantiatedTemplateControlGetHtmlDelegate getHtml);

		private JsDictionary config;
		
		public void DependenciesAvailable() {
			if (config != null) {
				this.id = (string)config["id"];
				var controlConfig = (JsDictionary)config["controls"];
				foreach (var de in controlConfig) {
					var ncfg = (JsDictionary)de.Value;
					this.controls[de.Key] = (IControl)Container.CreateObjectByTypeNameWithConstructorArg((string)ncfg["type"], ncfg["cfg"]);
				}
				this.namedElements = (List<string>)config["namedElements"];
				isAttached = true;
				config = null;
			}
		}

		/// <summary>
		/// This constructor should only be used for initialization on load
		/// </summary>
		public InstantiatedTemplateControl(object config) {
			if (Type.GetScriptType(config) == "function") {
				this.getHtml       = (InstantiatedTemplateControlGetHtmlDelegate)config;
				this.namedElements = new List<string>();
				this.config = null;
			}
			else {
				this.config = (JsDictionary)config;
			}
		}

		public Element GetNamedElement(string name) {
			if (!isAttached)
				throw new Exception("Must attach first");
			return Document.GetElementById(id + "_" + name);
		}
		
		public void Attach() {
			foreach (var de in controls) {
				var cc = (de.Value as IClientCreateControl);
				if (cc == null)
					throw new Exception("The control " + de.Key + " does not implement IClientCreateControl.");
				cc.Attach();
			}
			isAttached = true;
		}
#endif
	}
}
