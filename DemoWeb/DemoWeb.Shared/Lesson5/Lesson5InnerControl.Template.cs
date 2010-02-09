#if SERVER
using System;
using System.Collections.Generic;
using System.Text;
using Saltarelle;

namespace DemoWeb {
	public partial class Lesson5InnerControl : IControl {
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
			__cfg["person"] = this.person;
			__cfg["copyrightYear"] = this.copyrightYear;
			return __cfg;
		}

		private string GetHtml() {
			StringBuilder sb = new StringBuilder();
			sb.Append(@" <div id=""");
			sb.Append(Id);
			sb.Append(@""" style=""");
			sb.Append(PositionHelper.CreateStyle(Position, -1, -1));
			sb.Append(@"""");
			sb.Append(" __cfg=\"" + Utils.HtmlEncode(Utils.Json(GetConfig())) + "\"");
			sb.Append(@"> <div id=""");
			sb.Append(Id);
			sb.Append(@"_PersonDisplay"">&nbsp;</div> <img title=""");
			sb.Append("Copyright &copy; " + Utils.ToStringInvariantInt(this.copyrightYear) + @" Erik Källén");
			sb.Append(@""" src=""");
			sb.Append(GlobalServices.GetService<IUrlService>().BlankImageUrl);
			sb.Append(@""" width=""100"" height=""100"" style=""background-color: blue""/> ");
			sb.Append("Copyright &copy; " + Utils.ToStringInvariantInt(this.copyrightYear) + @" Erik Källén");
			sb.Append(@" </div> ");
			return sb.ToString();
		}

		private DemoWeb.Person person;

		private int copyrightYear;
		public int CopyrightYear {
			get { return copyrightYear; }
			set { copyrightYear = value; }
		}

		public string Html {
			get {
				if (string.IsNullOrEmpty(id))
					throw new InvalidOperationException("Must assign Id before rendering.");
				return GetHtml();
			}
		}

		public Lesson5InnerControl() {
			GlobalServices.GetService<IScriptManagerService>().RegisterType(GetType());
			copyrightYear = DateTime.Now.Year;
			Constructed();
		}
	}
}
#endif
#if CLIENT
using System;
using Saltarelle;

namespace DemoWeb {
	public partial class Lesson5InnerControl : IControl {
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
				PersonDisplay.attr("id", value + "_PersonDisplay");
			}
		}

		private DemoWeb.Person person;

		private jQuery PersonDisplay;

		private int copyrightYear;
		public int CopyrightYear {
			get { return copyrightYear; }
			set { copyrightYear = value; }
		}

		private void AttachSelf() {
			this.element = JQueryProxy.jQuery("#" + id);
			this.PersonDisplay = JQueryProxy.jQuery("#" + id + "_PersonDisplay");
			Attached();
		}

		public Lesson5InnerControl(string id) {
			if (!Script.IsUndefined(id)) {
				this.id = id;
				Dictionary __cfg = (Dictionary)Utils.EvalJson((string)JQueryProxy.jQuery("#" + id).attr("__cfg"));
				this.person = (DemoWeb.Person)__cfg["person"];
				copyrightYear = (int)__cfg["copyrightYear"];
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
