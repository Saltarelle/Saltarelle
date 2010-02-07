#if SERVER
using System;
using System.Collections.Generic;
using System.Text;
using Saltarelle;

namespace DemoWeb {
	public partial class Lesson4Control : IControl, IContainerControl {
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
			sb.Append(@"""");
			sb.Append(" __cfg=\"" + Utils.HtmlEncode(Utils.Json(GetConfig())) + "\"");
			sb.Append(@"> <table> <tr> <td style=""padding-right: 20px""> ");
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
			GlobalServices.GetService<IScriptManagerService>().RegisterType(GetType());
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

			Init();
		}
	}
}
#endif
#if CLIENT
using System;
using Saltarelle;

namespace DemoWeb {
	public partial class Lesson4Control : IControl, IContainerControl {
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
				FirstNameInput.attr("id", value + "_FirstNameInput");
				LastNameInput.attr("id", value + "_LastNameInput");
				TitleInput.attr("id", value + "_TitleInput");
				EmailInput.attr("id", value + "_EmailInput");
				EditEmployeeOKButton.attr("id", value + "_EditEmployeeOKButton");
				EditEmployeeCancelButton.attr("id", value + "_EditEmployeeCancelButton");
			}
		}

		private readonly Saltarelle.UI.Tree DepartmentsTree;

		private readonly Saltarelle.UI.Grid EmployeesGrid;

		private jQuery FirstNameInput;

		private jQuery LastNameInput;

		private jQuery TitleInput;

		private jQuery EmailInput;

		private jQuery EditEmployeeOKButton;

		private jQuery EditEmployeeCancelButton;

		private readonly Saltarelle.UI.DialogFrame EditEmployeeDialog;

		public Lesson4Control(string id) {
			if (!Script.IsUndefined(id)) {
				this.id = id;
				this.element = JQueryProxy.jQuery("#" + id);
				Dictionary __cfg = (Dictionary)Utils.EvalJson((string)this.element.attr("__cfg"));
				this.controls["DepartmentsTree"] = this.DepartmentsTree = new Saltarelle.UI.Tree(id + "_DepartmentsTree");
				this.controls["EmployeesGrid"] = this.EmployeesGrid = new Saltarelle.UI.Grid(id + "_EmployeesGrid");
				this.FirstNameInput = JQueryProxy.jQuery("#" + id + "_FirstNameInput");
				this.LastNameInput = JQueryProxy.jQuery("#" + id + "_LastNameInput");
				this.TitleInput = JQueryProxy.jQuery("#" + id + "_TitleInput");
				this.EmailInput = JQueryProxy.jQuery("#" + id + "_EmailInput");
				this.EditEmployeeOKButton = JQueryProxy.jQuery("#" + id + "_EditEmployeeOKButton");
				this.EditEmployeeCancelButton = JQueryProxy.jQuery("#" + id + "_EditEmployeeCancelButton");
				this.controls["EditEmployeeDialog"] = this.EditEmployeeDialog = new Saltarelle.UI.DialogFrame(id + "_EditEmployeeDialog");
			}
			else {
				throw new Exception("This control must be created server-side");
			}
			Init();
		}
	}
}
#endif
