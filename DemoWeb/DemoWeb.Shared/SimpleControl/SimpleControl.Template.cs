#if SERVER
using System;
using System.Collections.Generic;
using System.Text;
using Saltarelle;

namespace DemoWeb {
	public partial class SimpleControl : IControl, IContainerControl {
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
			__cfg["person"] = this.person;
			return __cfg;
		}

		private DemoWeb.Person person;

		private readonly DemoWeb.TextInput Textbox;

		private string GetHtml() {
			StringBuilder sb = new StringBuilder();
			sb.Append(@"<div id=""");
			sb.Append(Id);
			sb.Append(@""" style=""");
			sb.Append(PositionHelper.CreateStyle(Position, -1, -1));
			sb.Append(@"""");
			sb.Append(" __cfg=\"" + Utils.HtmlEncode(Utils.Json(GetConfig())) + "\"");
			sb.Append(@"> ");
			sb.Append(((IControl)Textbox).Html);
			sb.Append(@" <button id=""");
			sb.Append(Id);
			sb.Append(@"_AlertButton"" type=""button"">Show value</button> <div id=""");
			sb.Append(Id);
			sb.Append(@"_ValueDisplayer"">&nbsp;</div> <img title=""CopyrightNotice""/> <copyright>Erik Källén</copyright> <br/> ");
			sb.Append("This expression will be printed as code");
			sb.Append(@" <br/> ");
			int i = 0;
			sb.Append(@" ");
			sb.Append("Before: " + i.ToString());
			sb.Append(@" <br/> ");
			i++;
			sb.Append(@" ");
			sb.Append("After: " + i.ToString());
			sb.Append(@" <br/> ");
			for (int a = 0; a < 10; a++) {
				sb.Append(@" <div>Iteration ");
				sb.Append(a);
				sb.Append(@".</div> ");
			}
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

		public SimpleControl() {
			GlobalServices.GetService<IScriptManagerService>().RegisterType(GetType());
			this.controls["Textbox"] = this.Textbox = new DemoWeb.TextInput();
			this.Textbox.Value = "Some value";

			Init();
		}
	}
}
#endif
#if CLIENT
using System;
using Saltarelle;

namespace DemoWeb {
	public partial class SimpleControl : IControl, IContainerControl {
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
				AlertButton.attr("id", value + "_AlertButton");
				ValueDisplayer.attr("id", value + "_ValueDisplayer");
			}
		}

		private DemoWeb.Person person;

		private readonly DemoWeb.TextInput Textbox;

		private jQuery AlertButton;

		private jQuery ValueDisplayer;

		public SimpleControl(string id) {
			if (!Script.IsUndefined(id)) {
				this.id = id;
				this.element = JQueryProxy.jQuery("#" + id);
				Dictionary __cfg = (Dictionary)Utils.EvalJson((string)this.element.attr("__cfg"));
				this.person = (DemoWeb.Person)__cfg["person"];
				this.controls["Textbox"] = this.Textbox = new DemoWeb.TextInput(id + "_Textbox");
				this.AlertButton = JQueryProxy.jQuery("#" + id + "_AlertButton");
				this.ValueDisplayer = JQueryProxy.jQuery("#" + id + "_ValueDisplayer");
			}
			else {
				throw new Exception("This control must be created server-side");
			}
			Init();
		}
	}
}
#endif
