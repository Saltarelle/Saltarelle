#if SERVER
using System;
using System.Collections.Generic;
using System.Text;
using Saltarelle;

namespace DemoWeb {
	public partial class Lesson7Control : IControl {
		private Dictionary<string, IControl> controls = new Dictionary<string, IControl>();

		private Position position = PositionHelper.NotPositioned;
		public Position Position { get { return position; } set { position = value; } }

		private string id;
		public string Id {
			get { return id; }
			set {
				this.id = value;
				foreach (KeyValuePair<string, IControl> kvp in controls)
					kvp.Value.Id = value + "_" + kvp.Key;
			}
		}

		private Dictionary<string, object> GetConfig() {
			Dictionary<string, object> __cfg = new Dictionary<string, object>();
			return __cfg;
		}

		private string GetHtml() {
			StringBuilder sb = new StringBuilder();
			sb.Append(@"<div id=""");
			sb.Append(Id);
			sb.Append(@""" style=""");
			sb.Append(PositionHelper.CreateStyle(Position, -1, -1));
			sb.Append(@"""");
			sb.Append(" __cfg=\"" + Utils.HtmlEncode(Utils.Json(GetConfig())) + "\"");
			sb.Append(@"> <div> Enter some markup:<br/> <textarea id=""");
			sb.Append(Id);
			sb.Append(@"_DynamicMarkupInput"" rows=""10"" cols=""80""> <div style=""background-color: red""> <control type=""Saltarelle.UI.Label"" Text=""str:Label text""/> <div> Some text </div> </div> </textarea> <br/> <button type=""button"" id=""");
			sb.Append(Id);
			sb.Append(@"_InsertDynamicControlButton"">Insert Control</button> </div> <div id=""");
			sb.Append(Id);
			sb.Append(@"_DynamicControlContainer"">&nbsp;</div> <div> Number of rows: <input type=""text"" id=""");
			sb.Append(Id);
			sb.Append(@"_NumRowsInput"" value=""0"" style=""width: 50px""/> <button type=""button"" id=""");
			sb.Append(Id);
			sb.Append(@"_AjaxButton"">Create grid using Ajax</button> </div> <div id=""");
			sb.Append(Id);
			sb.Append(@"_AjaxControlContainer"">&nbsp;</div> </div>");
			return sb.ToString();
		}

		public string Html {
			get {
				if (string.IsNullOrEmpty(id))
					throw new InvalidOperationException("Must assign Id before rendering.");
				return GetHtml();
			}
		}

		public Lesson7Control() {
			GlobalServices.GetService<IScriptManagerService>().RegisterType(GetType());
			Constructed();
		}
	}
}
#endif
#if CLIENT
using System;
using Saltarelle;

namespace DemoWeb {
	public partial class Lesson7Control : IControl {
		private Dictionary controls = new Dictionary();

		private Position position;
		public Position Position {
			get { return element != null ? PositionHelper.GetPosition(element) : position; }
			set {
				position = value;
				if (element != null)
					PositionHelper.ApplyPosition(element, value);
			}
		}

		private jQuery element;
		public jQuery Element { get { return element; } }

		private string id;
		public string Id {
			get { return id; }
			set {
				this.id = value;
				foreach (DictionaryEntry kvp in controls)
					((IControl)kvp.Value).Id = value + "_" + kvp.Key;
				DynamicMarkupInput.attr("id", value + "_DynamicMarkupInput");
				InsertDynamicControlButton.attr("id", value + "_InsertDynamicControlButton");
				DynamicControlContainer.attr("id", value + "_DynamicControlContainer");
				NumRowsInput.attr("id", value + "_NumRowsInput");
				AjaxButton.attr("id", value + "_AjaxButton");
				AjaxControlContainer.attr("id", value + "_AjaxControlContainer");
			}
		}

		private jQuery DynamicMarkupInput;

		private jQuery InsertDynamicControlButton;

		private jQuery DynamicControlContainer;

		private jQuery NumRowsInput;

		private jQuery AjaxButton;

		private jQuery AjaxControlContainer;

		private void AttachSelf() {
			this.element = JQueryProxy.jQuery("#" + id);
			this.DynamicMarkupInput = JQueryProxy.jQuery("#" + id + "_DynamicMarkupInput");
			this.InsertDynamicControlButton = JQueryProxy.jQuery("#" + id + "_InsertDynamicControlButton");
			this.DynamicControlContainer = JQueryProxy.jQuery("#" + id + "_DynamicControlContainer");
			this.NumRowsInput = JQueryProxy.jQuery("#" + id + "_NumRowsInput");
			this.AjaxButton = JQueryProxy.jQuery("#" + id + "_AjaxButton");
			this.AjaxControlContainer = JQueryProxy.jQuery("#" + id + "_AjaxControlContainer");
			Attached();
		}

		public Lesson7Control(string id) {
			if (!Script.IsUndefined(id)) {
				this.id = id;
				Dictionary __cfg = (Dictionary)Utils.EvalJson((string)JQueryProxy.jQuery("#" + id).attr("__cfg"));
				Constructed();
				AttachSelf();
			}
			else {
				throw new Exception("This control must be created server-side");
			}
		}
	}
}
#endif
