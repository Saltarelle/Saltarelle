#if SERVER
using System;
using System.Collections.Generic;
using System.Text;
using Saltarelle;

namespace DemoWeb {
	public partial class Lesson5Control : IControl {
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

		private readonly DemoWeb.Lesson5InnerControl Nested;

		private string GetHtml() {
			StringBuilder sb = new StringBuilder();
			sb.Append(@"<div id=""");
			sb.Append(Id);
			sb.Append(@""" style=""");
			sb.Append(PositionHelper.CreateStyle(Position, -1, -1));
			sb.Append(@"""");
			sb.Append(" __cfg=\"" + Utils.HtmlEncode(Utils.Json(GetConfig())) + "\"");
			sb.Append(@"> ");
			sb.Append(((IControl)Nested).Html);
			sb.Append(@" </div>");
			return sb.ToString();
		}

		public string Html {
			get {
				if (string.IsNullOrEmpty(id))
					throw new InvalidOperationException("Must assign Id before rendering.");
				return GetHtml();
			}
		}

		public Lesson5Control() {
			GlobalServices.GetService<IScriptManagerService>().RegisterType(GetType());
			this.controls["Nested"] = this.Nested = new DemoWeb.Lesson5InnerControl();
			this.Nested.Person = new Person(@"Erik", @"Källén");

			Constructed();
		}
	}
}
#endif
#if CLIENT
using System;
using Saltarelle;

namespace DemoWeb {
	public partial class Lesson5Control : IControl {
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
			}
		}

		private readonly DemoWeb.Lesson5InnerControl Nested;

		private void AttachSelf() {
			this.element = JQueryProxy.jQuery("#" + id);
			Attached();
		}

		public Lesson5Control(string id) {
			if (!Script.IsUndefined(id)) {
				this.id = id;
				Dictionary __cfg = (Dictionary)Utils.EvalJson((string)JQueryProxy.jQuery("#" + id).attr("__cfg"));
				this.controls["Nested"] = this.Nested = new DemoWeb.Lesson5InnerControl(id + "_Nested");
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
