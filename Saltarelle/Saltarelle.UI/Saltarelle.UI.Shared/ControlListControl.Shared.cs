using System;
using Saltarelle;
#if CLIENT
using StringList = System.ArrayList;
using ControlList = System.ArrayList;
using ObjectList = System.ArrayList;
using ControlDictionary = System.Dictionary;
#endif
#if SERVER
using StringList  = System.Collections.Generic.List<string>;
using ControlList = System.Collections.Generic.List<Saltarelle.IControl>;
using ObjectList = System.Collections.Generic.List<object>;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using ControlDictionary = System.Collections.Generic.Dictionary<string, Saltarelle.IControl>;
#endif

namespace Saltarelle.UI {
	public class ControlListControl : IControl, IClientCreateControl {
		private string id;
		private string className;
		private Position position;
		private StringList controlIds;
		private ControlList controls;
		private ObjectList controlData;

		#if CLIENT
			private jQuery element;
		#endif
		
		public string Id {
			get { return id; }
			set {
				id = value;
				#if CLIENT
					if (!Utils.IsNull(element))
						element.attr("id", value);
				#endif
				for (int i = 0; i < Utils.ArrayLength(controls); i++) {
					((IControl)controls[i]).Id = id + "_" + Utils.ToStringInvariantInt(i);
				}
			}
		}
		
		public Position Position {
			get {
				#if CLIENT
					return !Utils.IsNull(element) ? PositionHelper.GetPosition(element) : position;
				#else
					return position;
				#endif
			}
			set {
				position = value;
				#if CLIENT
					if (!Utils.IsNull(element))
						PositionHelper.ApplyPosition(element, value);
				#endif
			}
		}
		
		public string ClassName {
			get { return className; }
			set {
				#if CLIENT
					if (!Utils.IsNull(element)) {
						if (!string.IsNullOrEmpty(className))
							element.removeClass(className);
						if (!string.IsNullOrEmpty(value))
							element.addClass(value);
					}
				#endif
				className = value;
			}
		}
		
		public IControl GetControlById(string controlId) {
			for (int i = 0; i < Utils.ArrayLength(controlIds); i++) {
				if ((string)controlIds[i] == controlId)
					return (IControl)controls[i];
			}
			return null;
		}

		public object GetControlDataById(string controlId) {
			for (int i = 0; i < Utils.ArrayLength(controlIds); i++) {
				if ((string)controlIds[i] == controlId)
					return controlData[i];
			}
			return null;
		}
		
		public ControlList ControlsList {
			get { return controls; }
		}
		
		public int NumControls { get { return Utils.ArrayLength(controlIds); } }

		public ControlDictionary Controls {
			get {
				ControlDictionary d = new ControlDictionary();
				for (int i = 0; i < Utils.ArrayLength(controlIds); i++) {
					d[(string)controlIds[i]] = controls[i];
				}
				return d;
			}
		}

		public string Html {
			get {
				if (string.IsNullOrEmpty(id))
					throw new Exception("Must set ID before render");
				StringBuilder sb = new StringBuilder();
				sb.Append("<div id=\"" + id + "\"" + (!string.IsNullOrEmpty(className) ? " class=\"" + className + "\"" : "") + " style=\"" + PositionHelper.CreateStyle(position, -1, -1) + "\""
				#if SERVER
					    + " __cfg=\"" + Utils.HtmlEncode(Utils.Json(ConfigObject)) + "\""
				#endif
				        + ">");
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
			controlIds = new StringList();
			controls = new ControlList();
			controlData = new ObjectList();
		}

#if SERVER
		public ControlListControl() {
			GlobalServices.Provider.GetService<IScriptManagerService>().RegisterType(GetType());
			InitDefault();
		}

		protected virtual void AddItemsToConfigObject(Dictionary<string, object> config) {
			config["className"]    = className;
			config["controlIds"]   = controlIds;
			config["controlData"]  = controlData;
			config["controlTypes"] = controls.Select(c => c.GetType().FullName).ToList();
		}

		private object ConfigObject {
			get {
				var config = new Dictionary<string, object>();
				AddItemsToConfigObject(config);
				return config;
			}
		}

		public void AddControl(string controlId, IControl control, object data) {
			if (!Utils.IsNull(id))
				control.Id = id + "_" + Utils.ToStringInvariantInt(Utils.ArrayLength(controls));
			controlIds.Add(controlId);
			controls.Add(control);
			controlData.Add(data);
		}
#endif
#if CLIENT
		[AlternateSignature]
		public extern ControlListControl();
		public ControlListControl(string id) {
			if (!Script.IsUndefined(id)) {
				this.id = id;
				Dictionary config = (Dictionary)Utils.EvalJson((string)JQueryProxy.jQuery("#" + id).attr("__cfg"));
				InitConfig(config);
			}
			else
				InitDefault();
		}

		protected virtual void InitConfig(Dictionary config) {
			className   = (string)config["className"];
			controlIds  = (StringList)config["controlIds"];
			controlData = (ObjectList)config["controlData"];
			controls    = new ControlList();
			
			StringList controlTypes = (StringList)config["controlTypes"];
			for (int i = 0; i < controlTypes.Length; i++) {
				Type tp = Type.GetType((string)controlTypes[i]);
				controls.Add(Type.CreateInstance(tp, id + "_" + Utils.ToStringInvariantInt(i)));
			}

			Attach();
		}

		public jQuery Element { get { return element; } }

		public void Attach() {
			if (Utils.IsNull(id) || !Utils.IsNull(element))
				throw new Exception("Must set ID and can only attach once");
			element = JQueryProxy.jQuery("#" + id);
		}

		public void AddControl(string controlId, IClientCreateControl control, object data) {
			if (!Utils.IsNull(((IControl)control).Element))
				throw new Exception("The control must not be rendered.");
			if (!Utils.IsNull(id))
				((IControl)control).Id = id + "_" + Utils.ToStringInvariantInt(Utils.ArrayLength(controls));
			if (!Utils.IsNull(element))
				Utils.RenderControl(control, element);
			controlIds.Add(controlId);
			controls.Add(control);
			controlData.Add(data);
		}
#endif
	}
}