#pragma warning disable 1591
#if SERVER
using System;
using System.Collections.Generic;
using System.Text;
using Saltarelle;
using Saltarelle.Ioc;

namespace DemoWeb {
	public partial class Lesson1Control : IControl, INotifyCreated {
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
				__cfg["TheText"] = this.TheText.ConfigObject;
				return __cfg;
			}
		}

		private IContainer _container;
		[ClientInject]
		public IContainer Container {
			get { return _container; }
			set { _container = value; }
		}

		private Saltarelle.UI.TextInput TheText {
			get { return (Saltarelle.UI.TextInput)controls["TheText"]; }
		}

		private string GetHtml() {
			StringBuilder sb = new StringBuilder();
			sb.Append(@"<div class=""Lesson1Control"" id=""");
			sb.Append(Id);
			sb.Append(@""" style=""");
			sb.Append(PositionHelper.CreateStyle(Position, -1, -1));
			sb.Append(@"""> ");
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

		[Obsolete(@"Do not construct this type directly. Always use IContainer.Resolve*()")]
		public Lesson1Control() {
		}

		public void DependenciesAvailable() {
			{
			Saltarelle.UI.TextInput c = (Saltarelle.UI.TextInput)Container.CreateObject(typeof(Saltarelle.UI.TextInput));
			this.controls["TheText"] = c;
			}
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
	public partial class Lesson1Control : IControl, INotifyCreated {
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
				this.AddMessageButton.ID = value + "_AddMessageButton";
				this.CurrentMessageDiv.ID = value + "_CurrentMessageDiv";
				this.MessageLogDiv.ID = value + "_MessageLogDiv";
				if (isAttached)
					GetElement().ID = value;
				this.id = value;
			}
		}

		private IContainer _container;
		public IContainer Container {
			get { return _container; }
			set { _container = value; }
		}

		private Saltarelle.UI.TextInput TheText {
			get { return (Saltarelle.UI.TextInput)controls["TheText"]; }
		}

		private Element AddMessageButton { get { return (Element)Document.GetElementById(id + "_AddMessageButton"); } }

		private Element CurrentMessageDiv { get { return (Element)Document.GetElementById(id + "_CurrentMessageDiv"); } }

		private DivElement MessageLogDiv { get { return (DivElement)Document.GetElementById(id + "_MessageLogDiv"); } }

		private void AttachSelf() {
			this.isAttached = true;
			Attached();
		}

		[Obsolete(@"Do not construct this type directly. Always use IContainer.Resolve*()")]
		public Lesson1Control(object config) {
			__cfg = (!Script.IsUndefined(config) ? JsDictionary.GetDictionary(config) : null);
		}

		public void DependenciesAvailable() {
			if (!Utils.IsNull(__cfg)) {
				this.id = (string)__cfg["id"];
				this.controls["TheText"] = (Saltarelle.UI.TextInput)Container.CreateObjectWithConstructorArg(typeof(Saltarelle.UI.TextInput), __cfg["TheText"]);
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
