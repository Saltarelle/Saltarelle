#pragma warning disable 1591
#if SERVER
using System;
using System.Collections.Generic;
using System.Text;
using Saltarelle;
using Saltarelle.Ioc;

namespace DemoWeb {
	public partial class Lesson7Control : IControl, INotifyCreated {
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
				return __cfg;
			}
		}

		private string GetHtml() {
			StringBuilder sb = new StringBuilder();
			sb.Append(@"<div id=""");
			sb.Append(Id);
			sb.Append(@""" style=""");
			sb.Append(PositionHelper.CreateStyle(Position, -1, -1));
			sb.Append(@"""> <div> Enter some markup:<br/> <textarea id=""");
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

		[Obsolete(@"Do not construct this type directly. Always use IContainer.Resolve*()")]
		public Lesson7Control() {
		}

		public void DependenciesAvailable() {
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
	public partial class Lesson7Control : IControl, INotifyCreated {
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
				this.DynamicMarkupInput.ID = value + "_DynamicMarkupInput";
				this.InsertDynamicControlButton.ID = value + "_InsertDynamicControlButton";
				this.DynamicControlContainer.ID = value + "_DynamicControlContainer";
				this.NumRowsInput.ID = value + "_NumRowsInput";
				this.AjaxButton.ID = value + "_AjaxButton";
				this.AjaxControlContainer.ID = value + "_AjaxControlContainer";
				if (isAttached)
					GetElement().ID = value;
				this.id = value;
			}
		}

		private TextAreaElement DynamicMarkupInput { get { return (TextAreaElement)Document.GetElementById(id + "_DynamicMarkupInput"); } }

		private Element InsertDynamicControlButton { get { return (Element)Document.GetElementById(id + "_InsertDynamicControlButton"); } }

		private DivElement DynamicControlContainer { get { return (DivElement)Document.GetElementById(id + "_DynamicControlContainer"); } }

		private TextElement NumRowsInput { get { return (TextElement)Document.GetElementById(id + "_NumRowsInput"); } }

		private Element AjaxButton { get { return (Element)Document.GetElementById(id + "_AjaxButton"); } }

		private DivElement AjaxControlContainer { get { return (DivElement)Document.GetElementById(id + "_AjaxControlContainer"); } }

		private void AttachSelf() {
			this.isAttached = true;
			Attached();
		}

		[Obsolete(@"Do not construct this type directly. Always use IContainer.Resolve*()")]
		public Lesson7Control(object config) {
			__cfg = (!Script.IsUndefined(config) ? JsDictionary.GetDictionary(config) : null);
		}

		public void DependenciesAvailable() {
			if (!Utils.IsNull(__cfg)) {
				this.id = (string)__cfg["id"];
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
