#pragma warning disable 1591
#if SERVER
using System;
using System.Collections.Generic;
using System.Text;
using Saltarelle;

namespace DemoWeb {
	public partial class Lesson4Control : IControl {
		private Dictionary<string, IControl> controls = new Dictionary<string, IControl>();

		private Position position = PositionHelper.NotPositioned;
		public Position Position { get { return position; } set { position = value; } }

		private string id;
		public string Id {
			get { return id; }
			set {
				foreach (KeyValuePair<string, IControl> kvp in controls)
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

		private readonly Saltarelle.UI.Tree DepartmentsTree;

		private readonly Saltarelle.UI.Grid EmployeesGrid;

		private string EditEmployeeDialog_inner() {
			StringBuilder sb = new StringBuilder();
			sb.Append(@" <table> <tr> <td style=""text-align: right; padding-bottom: 4px; padding-right: 4px; white-space: nowrap"">First Name:</td> <td style=""padding-bottom: 4px""><input type=""text"" id=""");
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
			sb.Append(@"_EditEmployeeCancelButton"">Cancel</button> </td> </tr> </table> ");
			return sb.ToString();
		}

		private readonly Saltarelle.UI.DialogFrame EditEmployeeDialog;

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
			((IControlHost)EditEmployeeDialog).SetInnerHtml(EditEmployeeDialog_inner());
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

		public Lesson4Control() {
			GlobalServices.GetService<IScriptManagerService>().RegisterClientType(GetType());
			this.controls["DepartmentsTree"] = this.DepartmentsTree = new Saltarelle.UI.Tree();
			this.DepartmentsTree.Width = 300;
			this.DepartmentsTree.Height = 300;

			this.controls["EmployeesGrid"] = this.EmployeesGrid = new Saltarelle.UI.Grid();
			this.EmployeesGrid.ColTitles = new string[] { @"First Name", @"Last Name", @"" };
			this.EmployeesGrid.ColWidths = new int[] { 200, 200, 50 };
			this.EmployeesGrid.ColClasses = new string[] { @"", @"", @"ActionCol" };
			this.EmployeesGrid.Width = 500;
			this.EmployeesGrid.Height = 200;

			this.controls["EditEmployeeDialog"] = this.EditEmployeeDialog = new Saltarelle.UI.DialogFrame();
			this.EditEmployeeDialog.ModalityInt = 1;
			this.EditEmployeeDialog.Title = @"Employee Details";

			Constructed();
		}
	}
}
#endif
#if CLIENT
using System;
using System.DHTML;
using Saltarelle;

namespace DemoWeb {
	public partial class Lesson4Control : IControl {
		private Dictionary controls = new Dictionary();

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
		public DOMElement GetElement() { return isAttached ? Document.GetElementById(id) : null; }

		private string id;
		public string Id {
			get { return id; }
			set {
				foreach (DictionaryEntry kvp in controls)
					((IControl)kvp.Value).Id = value + "_" + kvp.Key;
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

		private readonly Saltarelle.UI.Tree DepartmentsTree;

		private readonly Saltarelle.UI.Grid EmployeesGrid;

		private TextElement FirstNameInput { get { return (TextElement)Document.GetElementById(id + "_FirstNameInput"); } }

		private TextElement LastNameInput { get { return (TextElement)Document.GetElementById(id + "_LastNameInput"); } }

		private TextElement TitleInput { get { return (TextElement)Document.GetElementById(id + "_TitleInput"); } }

		private TextElement EmailInput { get { return (TextElement)Document.GetElementById(id + "_EmailInput"); } }

		private DOMElement EditEmployeeOKButton { get { return (DOMElement)Document.GetElementById(id + "_EditEmployeeOKButton"); } }

		private DOMElement EditEmployeeCancelButton { get { return (DOMElement)Document.GetElementById(id + "_EditEmployeeCancelButton"); } }

		private readonly Saltarelle.UI.DialogFrame EditEmployeeDialog;

		private void AttachSelf() {
			this.isAttached = true;
			Attached();
		}

		public Lesson4Control(object config) {
			if (!Script.IsUndefined(config)) {
				Dictionary __cfg = Dictionary.GetDictionary(config);
				this.id = (string)__cfg["id"];
				this.controls["DepartmentsTree"] = this.DepartmentsTree = new Saltarelle.UI.Tree(__cfg["DepartmentsTree"]);
				this.controls["EmployeesGrid"] = this.EmployeesGrid = new Saltarelle.UI.Grid(__cfg["EmployeesGrid"]);
				this.controls["EditEmployeeDialog"] = this.EditEmployeeDialog = new Saltarelle.UI.DialogFrame(__cfg["EditEmployeeDialog"]);
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
