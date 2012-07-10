#pragma warning disable 1591
#if SERVER
using System;
using System.Collections.Generic;
using System.Text;
using Saltarelle;
using Saltarelle.Ioc;

namespace DemoWeb {
	public partial class Lesson5InnerControl : IControl, INotifyCreated {
		private Dictionary<string, IControl> controls = new Dictionary<string, IControl>();

		private Position position = PositionHelper.NotPositioned;
		public Position Position { get { return position; } set { position = value; } }

		private string id;
		public string Id {
			get { return id; }
			set {
				foreach (var kvp in controls)
					kvp.Value.Id = value + "_" + kvp.Key;
				this.id = value;
			}
		}

		public object ConfigObject {
			get {
				Dictionary<string, object> __cfg = new Dictionary<string, object>();
				__cfg["id"] = id;
				__cfg["person"] = this.person;
				__cfg["copyrightYear"] = this.copyrightYear;
				return __cfg;
			}
		}

		private string GetHtml() {
			StringBuilder sb = new StringBuilder();
			sb.Append(@" <div id=""");
			sb.Append(Id);
			sb.Append(@""" style=""");
			sb.Append(PositionHelper.CreateStyle(Position, -1, -1));
			sb.Append(@"""> <div id=""");
			sb.Append(Id);
			sb.Append(@"_PersonDisplay"">&nbsp;</div> <img title=""");
			sb.Append("Copyright &copy; " + Utils.ToStringInvariantInt(this.copyrightYear) + @" Erik Källén");
			sb.Append(@""" src=""");
			sb.Append(UIService.BlankImageUrl);
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

		[Obsolete(@"Do not construct this type directly. Always use IContainer.Resolve*()")]
		public Lesson5InnerControl() {
		}

		public void DependenciesAvailable() {
			copyrightYear = DateTime.Now.Year;
			Constructed();
		}
	}
}
#endif
#if CLIENT
using System;
using System.Collections;
using System.Collections.Generic;
using System.Html;
using Saltarelle;
using Saltarelle.Ioc;

namespace DemoWeb {
	public partial class Lesson5InnerControl : IControl, INotifyCreated {
		private Dictionary<string, IControl> controls = new Dictionary<string, IControl>();
		private JsDictionary __cfg;

		private Position position;
		public Position Position {
			get { return isAttached ? PositionHelper.GetPosition(GetElement()) : position; }
			set {
				position = value;
				if (isAttached)
					PositionHelper.ApplyPosition(GetElement(), value);
			}
		}

		private bool isAttached = false;
		public Element GetElement() { return isAttached ? Document.GetElementById(id) : null; }

		private string id;
		public string Id {
			get { return id; }
			set {
				foreach (var kvp in controls)
					kvp.Value.Id = value + "_" + kvp.Key;
				this.PersonDisplay.ID = value + "_PersonDisplay";
				if (isAttached)
					GetElement().ID = value;
				this.id = value;
			}
		}

		private DemoWeb.Person person;

		private DivElement PersonDisplay { get { return (DivElement)Document.GetElementById(id + "_PersonDisplay"); } }

		private int copyrightYear;
		public int CopyrightYear {
			get { return copyrightYear; }
			set { copyrightYear = value; }
		}

		private void AttachSelf() {
			this.isAttached = true;
			Attached();
		}

		[Obsolete(@"Do not construct this type directly. Always use IContainer.Resolve*()")]
		public Lesson5InnerControl(object config) {
			__cfg = (!Script.IsUndefined(config) ? JsDictionary.GetDictionary(config) : null);
		}

		public void DependenciesAvailable() {
			if (!Utils.IsNull(__cfg)) {
				this.id = (string)__cfg["id"];
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
