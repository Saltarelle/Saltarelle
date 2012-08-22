#pragma warning disable 1591
#if SERVER
using System;
using System.Collections.Generic;
using System.Text;
using Saltarelle;
using Saltarelle.Ioc;

namespace DemoWeb {
	public partial class Lesson5Control : IControl, INotifyCreated {
		partial void Constructed();

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
				__cfg["Nested"] = this.Nested.ConfigObject;
				return __cfg;
			}
		}

		private IContainer _container;
		[ClientInject]
		public IContainer Container {
			get { return _container; }
			set { _container = value; }
		}

		private DemoWeb.Lesson5InnerControl Nested {
			get { return (DemoWeb.Lesson5InnerControl)controls["Nested"]; }
		}

		private string GetHtml() {
			StringBuilder sb = new StringBuilder();
			sb.Append(@"<div id=""");
			sb.Append(Id);
			sb.Append(@""" style=""");
			sb.Append(PositionHelper.CreateStyle(Position, -1, -1));
			sb.Append(@"""> ");
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

		[Obsolete(@"Do not construct this type directly. Always use IContainer.Resolve*()")]
		public Lesson5Control() {
		}

		public void DependenciesAvailable() {
			{
			DemoWeb.Lesson5InnerControl c = (DemoWeb.Lesson5InnerControl)Container.CreateObject(typeof(DemoWeb.Lesson5InnerControl));
			c.Person = new Person(@"Erik", @"Källén");
			this.controls["Nested"] = c;
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
	public partial class Lesson5Control : IControl, INotifyCreated {
		partial void Constructed();
		partial void Attached();

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

		private DemoWeb.Lesson5InnerControl Nested {
			get { return (DemoWeb.Lesson5InnerControl)controls["Nested"]; }
		}

		private void AttachSelf() {
			this.isAttached = true;
			Attached();
		}

		[Obsolete(@"Do not construct this type directly. Always use IContainer.Resolve*()")]
		public Lesson5Control(object config) {
			__cfg = (!Script.IsUndefined(config) ? JsDictionary.GetDictionary(config) : null);
		}

		public void DependenciesAvailable() {
			if (!Utils.IsNull(__cfg)) {
				this.id = (string)__cfg["id"];
				this.controls["Nested"] = (DemoWeb.Lesson5InnerControl)Container.CreateObjectWithConstructorArg(typeof(DemoWeb.Lesson5InnerControl), __cfg["Nested"]);
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
