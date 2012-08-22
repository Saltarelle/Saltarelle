#pragma warning disable 1591
#if SERVER
using System;
using System.Collections.Generic;
using System.Text;
using Saltarelle;
using Saltarelle.Ioc;

namespace DemoWeb {
	public partial class Lesson4Control : IControl, INotifyCreated {
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
				__cfg["DepartmentsTree"] = this.DepartmentsTree.ConfigObject;
				__cfg["EmployeesGrid"] = this.EmployeesGrid.ConfigObject;
				__cfg["EditEmployeeDialog"] = this.EditEmployeeDialog.ConfigObject;
				return __cfg;
			}
		}

		private IContainer _container;
		[ClientInject]
		public IContainer Container {
			get { return _container; }
			set { _container = value; }
		}

		private Saltarelle.UI.Tree DepartmentsTree {
			get { return (Saltarelle.UI.Tree)controls["DepartmentsTree"]; }
		}

		private Saltarelle.UI.Grid EmployeesGrid {
			get { return (Saltarelle.UI.Grid)controls["EmployeesGrid"]; }
		}

		private string EditEmployeeDialog_inner1() {
			StringBuilder sb = new StringBuilder();
			sb.Append(@"<table> <tr> <td style=""text-align: right; padding-bottom: 4px; padding-right: 4px; white-space: nowrap"">First Name:</td> <td style=""padding-bottom: 4px""><input type=""text"" id=""");
			sb.Append(Id);
			sb.Append(@"_FirstNameInput"" style=""width: 200px""/></td> </tr> <tr> <td style=""text-align: right; padding-bottom: 4px; padding-right: 4px; white-space: nowrap"">Last Name:</td> <td style=""padding-bottom: 4px""><input type=""text"" id=""");
			sb.Append(Id);
			sb.Append(@"_LastNameInput"" style=""width: 200px""/></td> </tr> <tr> <td style=""text-align: right; padding-bottom: 4px; padding-right: 4px; white-space: nowrap"">Title:</td> <td style=""padding-bottom: 4px""><input type=""text"" id=""");
			sb.Append(Id);
			sb.Append(@"_TitleInput"" style=""width: 40px""/></td> </tr> <tr> <td style=""text-align: right; padding-bottom: 4px; padding-right: 4px; white-space: nowrap"">Email Address:</td> <td style=""padding-bottom: 4px""><input type=""text"" id=""");
			sb.Append(Id);
			sb.Append(@"_EmailInput"" style=""width: 250px""/></td> </tr> <tr> <td colspan=""2"" style=""text-align: center""> <button type=""button"" style=""width: 80px"" id=""");
			sb.Append(Id);
			sb.Append(@"_EditEmployeeOKButton"">OK</button> <button type=""button"" style=""width: 80px"" id=""");
			sb.Append(Id);
			sb.Append(@"_EditEmployeeCancelButton"">Cancel</button> </td> </tr> </table>");
			return sb.ToString();
		}

		private Saltarelle.UI.DialogFrame EditEmployeeDialog {
			get { return (Saltarelle.UI.DialogFrame)controls["EditEmployeeDialog"]; }
		}

		private string GetHtml() {
			StringBuilder sb = new StringBuilder();
			sb.Append(@"<div id=""");
			sb.Append(Id);
			sb.Append(@""" style=""");
			sb.Append(PositionHelper.CreateStyle(Position, -1, -1));
			sb.Append(@"""> <table> <tr> <td style=""padding-right: 20px""> ");
			sb.Append(((IControl)DepartmentsTree).Html);
			sb.Append(@" </td> <td> ");
			sb.Append(((IControl)EmployeesGrid).Html);
			sb.Append(@" </td> </tr> </table> ");
			((IControlHost)EditEmployeeDialog).SetInnerFragments(new string[] { EditEmployeeDialog_inner1() });
			sb.Append(((IControl)EditEmployeeDialog).Html);
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
		public Lesson4Control() {
		}

		public void DependenciesAvailable() {
			{
			Saltarelle.UI.Tree c = (Saltarelle.UI.Tree)Container.CreateObject(typeof(Saltarelle.UI.Tree));
			c.Width = 300;
			c.Height = 300;
			this.controls["DepartmentsTree"] = c;
			}
			{
			Saltarelle.UI.Grid c = (Saltarelle.UI.Grid)Container.CreateObject(typeof(Saltarelle.UI.Grid));
			c.ColTitles = new string[] { @"First Name", @"Last Name", @"" };
			c.ColWidths = new int[] { 200, 200, 50 };
			c.ColClasses = new string[] { @"", @"", @"ActionCol" };
			c.Width = 500;
			c.Height = 200;
			this.controls["EmployeesGrid"] = c;
			}
			{
			Saltarelle.UI.DialogFrame c = (Saltarelle.UI.DialogFrame)Container.CreateObject(typeof(Saltarelle.UI.DialogFrame));
			c.ModalityInt = 1;
			c.Title = @"Employee Details";
			this.controls["EditEmployeeDialog"] = c;
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
	public partial class Lesson4Control : IControl, INotifyCreated {
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
				this.FirstNameInput.ID = value + "_FirstNameInput";
				this.LastNameInput.ID = value + "_LastNameInput";
				this.TitleInput.ID = value + "_TitleInput";
				this.EmailInput.ID = value + "_EmailInput";
				this.EditEmployeeOKButton.ID = value + "_EditEmployeeOKButton";
				this.EditEmployeeCancelButton.ID = value + "_EditEmployeeCancelButton";
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

		private Saltarelle.UI.Tree DepartmentsTree {
			get { return (Saltarelle.UI.Tree)controls["DepartmentsTree"]; }
		}

		private Saltarelle.UI.Grid EmployeesGrid {
			get { return (Saltarelle.UI.Grid)controls["EmployeesGrid"]; }
		}

		private TextElement FirstNameInput { get { return (TextElement)Document.GetElementById(id + "_FirstNameInput"); } }

		private TextElement LastNameInput { get { return (TextElement)Document.GetElementById(id + "_LastNameInput"); } }

		private TextElement TitleInput { get { return (TextElement)Document.GetElementById(id + "_TitleInput"); } }

		private TextElement EmailInput { get { return (TextElement)Document.GetElementById(id + "_EmailInput"); } }

		private Element EditEmployeeOKButton { get { return (Element)Document.GetElementById(id + "_EditEmployeeOKButton"); } }

		private Element EditEmployeeCancelButton { get { return (Element)Document.GetElementById(id + "_EditEmployeeCancelButton"); } }

		private Saltarelle.UI.DialogFrame EditEmployeeDialog {
			get { return (Saltarelle.UI.DialogFrame)controls["EditEmployeeDialog"]; }
		}

		private void AttachSelf() {
			this.isAttached = true;
			Attached();
		}

		[Obsolete(@"Do not construct this type directly. Always use IContainer.Resolve*()")]
		public Lesson4Control(object config) {
			__cfg = (!Script.IsUndefined(config) ? JsDictionary.GetDictionary(config) : null);
		}

		public void DependenciesAvailable() {
			if (!Utils.IsNull(__cfg)) {
				this.id = (string)__cfg["id"];
				this.controls["DepartmentsTree"] = (Saltarelle.UI.Tree)Container.CreateObjectWithConstructorArg(typeof(Saltarelle.UI.Tree), __cfg["DepartmentsTree"]);
				this.controls["EmployeesGrid"] = (Saltarelle.UI.Grid)Container.CreateObjectWithConstructorArg(typeof(Saltarelle.UI.Grid), __cfg["EmployeesGrid"]);
				this.controls["EditEmployeeDialog"] = (Saltarelle.UI.DialogFrame)Container.CreateObjectWithConstructorArg(typeof(Saltarelle.UI.DialogFrame), __cfg["EditEmployeeDialog"]);
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
