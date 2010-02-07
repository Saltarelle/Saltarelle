#if SERVER
using System;
using System.Collections.Generic;
using System.Text;
using Saltarelle;

namespace DemoWeb {
	public partial class Lesson1Control : IControl {
		private Dictionary<string, IControl> controls = new Dictionary<string, IControl>();
		public Dictionary<string, IControl> Controls { get { return controls; } }

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

		private readonly Saltarelle.UI.TextInput TheText;

		private string GetHtml() {
			StringBuilder sb = new StringBuilder();
			sb.Append(@"<div class=""Lesson1Control"" id=""");
			sb.Append(Id);
			sb.Append(@""" style=""");
			sb.Append(PositionHelper.CreateStyle(Position, -1, -1));
			sb.Append(@"""");
			sb.Append(" __cfg=\"" + Utils.HtmlEncode(Utils.Json(GetConfig())) + "\"");
			sb.Append(@"> ");
			sb.Append(((IControl)TheText).Html);
			sb.Append(@" <button type=""button"" id=""");
			sb.Append(Id);
			sb.Append(@"_AddMessageButton"">Add Message</button> <div> Current Message: <span id=""");
			sb.Append(Id);
			sb.Append(@"_CurrentMessageDiv"">&nbsp;</span> </div> <div style=""padding-top: 20px""> Logged Messages: </div> <div id=""");
			sb.Append(Id);
			sb.Append(@"_MessageLogDiv"" class=""MessageLog""> </div> </div>");
			return sb.ToString();
		}

		public string Html {
			get {
				if (string.IsNullOrEmpty(id))
					throw new InvalidOperationException("Must assign Id before rendering.");
				return GetHtml();
			}
		}

		public Lesson1Control() {
			GlobalServices.GetService<IScriptManagerService>().RegisterType(GetType());
			this.controls["TheText"] = this.TheText = new Saltarelle.UI.TextInput();

			Constructed();
		}
	}
}
#endif
#if CLIENT
using System;
using Saltarelle;

namespace DemoWeb {
	public partial class Lesson1Control : IControl {
		private Dictionary controls = new Dictionary();
		public Dictionary Controls { get { return controls; } }

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
				AddMessageButton.attr("id", value + "_AddMessageButton");
				CurrentMessageDiv.attr("id", value + "_CurrentMessageDiv");
				MessageLogDiv.attr("id", value + "_MessageLogDiv");
			}
		}

		private readonly Saltarelle.UI.TextInput TheText;

		private jQuery AddMessageButton;

		private jQuery CurrentMessageDiv;

		private jQuery MessageLogDiv;

		private void AttachSelf() {
			this.AddMessageButton = JQueryProxy.jQuery("#" + id + "_AddMessageButton");
			this.CurrentMessageDiv = JQueryProxy.jQuery("#" + id + "_CurrentMessageDiv");
			this.MessageLogDiv = JQueryProxy.jQuery("#" + id + "_MessageLogDiv");
			Attached();
		}

		public Lesson1Control(string id) {
			if (!Script.IsUndefined(id)) {
				this.id = id;
				this.element = JQueryProxy.jQuery("#" + id);
				Dictionary __cfg = (Dictionary)Utils.EvalJson((string)this.element.attr("__cfg"));
				this.controls["TheText"] = this.TheText = new Saltarelle.UI.TextInput(id + "_TheText");
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
