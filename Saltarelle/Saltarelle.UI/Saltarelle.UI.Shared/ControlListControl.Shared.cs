using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Saltarelle;
using Saltarelle.Ioc;
#if SERVER
using System.Linq;
#endif
#if CLIENT
using System.Html;
using jQueryApi;

#endif

namespace Saltarelle.UI {
	public class ControlListControl : IControl, IClientCreateControl, INotifyCreated
	{
		private string id;
		private string className;
		private Position position;
		private List<string> controlIds;
		private List<IControl> controls;
		private List<object> controlData;

		#if CLIENT
			private bool isAttached;
		#endif

		private IContainer container;
		#if SERVER
		[ClientInject]
		#endif
		public IContainer Container { get { return container; } set { container = value; } }
		
		public string Id {
			get { return id; }
			set {
				id = value;
				#if CLIENT
					if (isAttached)
						GetElement().ID = value;
				#endif
				for (int i = 0; i < controls.Count; i++) {
					controls[i].Id = id + "_" + Utils.ToStringInvariantInt(i);
				}
			}
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
		
		public string ClassName {
			get { return className; }
			set {
				#if CLIENT
					if (isAttached) {
						var element = jQuery.FromElement(GetElement());
						if (!string.IsNullOrEmpty(className))
							element.RemoveClass(className);
						if (!string.IsNullOrEmpty(value))
							element.AddClass(value);
					}
				#endif
				className = value;
			}
		}
		
		public IControl GetControlById(string controlId) {
			for (int i = 0; i < controlIds.Count; i++) {
				if (controlIds[i] == controlId)
					return controls[i];
			}
			return null;
		}

		public object GetControlDataById(string controlId) {
			for (int i = 0; i < controlIds.Count; i++) {
				if (controlIds[i] == controlId)
					return controlData[i];
			}
			return null;
		}
		
		public IList<IControl> ControlsList {
			get { return controls; }
		}
		
		public int NumControls { get { return controlIds.Count; } }

		public IDictionary<string, IControl> Controls {
			get {
				var d = new Dictionary<string, IControl>();
				for (int i = 0; i < controlIds.Count; i++) {
					d[controlIds[i]] = controls[i];
				}
				return d;
			}
		}

		public string Html {
			get {
				if (string.IsNullOrEmpty(id))
					throw new Exception("Must set ID before render");
				var sb = new StringBuilder();
				sb.Append("<div id=\"" + id + "\"" + (!string.IsNullOrEmpty(className) ? " class=\"" + className + "\"" : "") + " style=\"" + PositionHelper.CreateStyle(position, -1, -1) + "\">");
				#if SERVER
					foreach (IControl c in controls)
						sb.Append(c.Html);
				#endif
				#if CLIENT
					foreach (IClientCreateControl c in controls)
						sb.Append(c.Html);
				#endif
				sb.Append("</div>");
				return sb.ToString();
			}
		}
		
		protected virtual void InitDefault() {
			position = PositionHelper.NotPositioned;
			controlIds = new List<string>();
			controls = new List<IControl>();
			controlData = new List<object>();
		}

#if SERVER
		public ControlListControl() {
			InitDefault();
		}

		protected virtual void AddItemsToConfigObject(Dictionary<string, object> config) {
			config["id"]           = id;
			config["className"]    = className;
			config["controlIds"]   = controlIds;
			config["controlData"]  = controlData;
			config["controlTypes"] = controls.Select(c => c.GetType().FullName).ToList();
			config["controlCfg"]   = controls.Select(c => c.ConfigObject);
		}

		public object ConfigObject {
			get {
				var config = new Dictionary<string, object>();
				AddItemsToConfigObject(config);
				return config;
			}
		}

		public void AddControl(string controlId, IControl control, object data) {
			if (!Utils.IsNull(id))
				control.Id = id + "_" + Utils.ToStringInvariantInt(controls.Count);
			controlIds.Add(controlId);
			controls.Add(control);
			controlData.Add(data);
		}

		public void DependenciesAvailable() {
		}
#endif
#if CLIENT
		private JsDictionary config;

		[AlternateSignature]
		public ControlListControl() {}
		public ControlListControl(object config) {
			this.config = !Script.IsUndefined(config) ? JsDictionary.GetDictionary(config) : null;
		}

		protected virtual void InitConfig(JsDictionary config) {
			id          = (string)config["id"];
			className   = (string)config["className"];
			controlIds  = (List<string>)config["controlIds"];
			controlData = (List<object>)config["controlData"];
			controls    = new List<IControl>();
			
			var controlTypes = (List<string>)config["controlTypes"];
			var controlCfg   = (List<object>)config["controlCfg"];
			for (int i = 0; i < controlTypes.Count; i++) {
				controls.Add((IControl)Container.CreateObjectByTypeNameWithConstructorArg((string)controlTypes[i], controlCfg[i]));
			}

			Attach();
		}

		public Element GetElement() { return isAttached ? Document.GetElementById(id) : null; }

		public void Attach() {
			if (Utils.IsNull(id) || isAttached)
				throw new Exception("Must set ID and can only attach once");
			isAttached = true;
		}

		public void AddControl(string controlId, IClientCreateControl control, object data) {
			if (!Utils.IsNull(((IControl)control).GetElement()))
				throw new Exception("The control must not be rendered.");
			if (!Utils.IsNull(id))
				control.Id = id + "_" + Utils.ToStringInvariantInt(controls.Count);
			if (isAttached) {
				Utils.RenderControl(control, GetElement());
			}
			controlIds.Add(controlId);
			controls.Add(control);
			controlData.Add(data);
		}

		public void DependenciesAvailable() {
			if (!Utils.IsNull(config))
				InitConfig(config);
			else
				InitDefault();
		}
#endif
	}
}