using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Saltarelle;
using Saltarelle.UI;
#if CLIENT
using System.Html;
using jQueryApi;
#endif

namespace DemoWeb {
	[Record]
	public sealed class Employee {
		public string firstName;
		public string lastName;
		public string title;
		public string email;
		
		public Employee(string firstName, string lastName, string title, string email) {
			this.firstName = firstName;
			this.lastName  = lastName;
			this.title     = title;
			this.email     = email;
		}
	}

	[Record]
	public sealed class Department {
		public string name;
		public Department[] children;
		public Employee[] employees;
		
		public Department(string name, Department[] children, Employee[] employees) {
			this.name      = name;
			this.children  = children;
			this.employees = employees;
		}
	}

	public partial class Lesson4Control : IControl {
#if SERVER
		#region Data
		private Department[] data = new[] { new Department("Executive General and Administration",
		                                                   new[] { new Department("Facilities and Maintenance",
		                                                               null,
		                                                               new[] { new Employee("Taylor", "Maxwell", null, "taylor0@adventure-works.com"),
		                                                                       new Employee("Paula", "Barreto de Mattos", null, "paula0@adventure-works.com"),
		                                                                       new Employee("Sariya", "Harnpadoungsataya", null, "sariya0@adventure-works.com"),
		                                                                       new Employee("George", "Li", null, "george0@adventure-works.com") }),
		                                                           new Department("Finance",
		                                                               null,
		                                                               new[] { new Employee("Suroor", "Fatima", null, "suroor0@adventure-works.com"),
		                                                                       new Employee("Jean", "Trenary", null, "jean0@adventure-works.com"),
		                                                                       new Employee("Ivo", "Salmre", null, "ivo0@adventure-works.com"),
		                                                                       new Employee("Barry", "Johnson", null, "barry0@adventure-works.com") }),
		                                                           new Department("Human Resources",
		                                                               null,
		                                                               new[] { new Employee("Gail", "Erickson", "Ms.", "gail0@adventure-works.com"),
		                                                                       new Employee("Zheng", "Mu", null, "zheng0@adventure-works.com"),
		                                                                       new Employee("Peng", "Wu", null, "peng0@adventure-works.com"),
		                                                                       new Employee("Annik", "Stahl", null, "annik0@adventure-works.com") }),
		                                                           new Department("Information Services",
		                                                               null,
		                                                               new[] { new Employee("Deborah", "Poe", null, "deborah0@adventure-works.com"),
		                                                                       new Employee("Russell", "Hunter", null, "russell0@adventure-works.com"),
		                                                                       new Employee("Paul", "Komosinski", null, "paul0@adventure-works.com"),
		                                                                       new Employee("Jossef", "Goldberg", "Mr.", "jossef0@adventure-works.com") }) },
		                                                   null),
		                                    new Department("Inventory Management",
		                                                   new[] { new Department("Purchasing",
		                                                               null,
		                                                               new[] { new Employee("Thierry", "D'Hers", null, "thierry0@adventure-works.com"),
		                                                                       new Employee("Peter", "Krebs", null, "peter0@adventure-works.com"),
		                                                                       new Employee("Simon", "Rapier", null, "simon0@adventure-works.com"),
		                                                                       new Employee("David", "Ortiz", null, "david2@adventure-works.com") }),
		                                                           new Department("Shipping and Receiving",
		                                                               null,
		                                                               new[] { new Employee("Willis", "Johnson", null, "willis0@adventure-works.com"),
		                                                                       new Employee("Gary", "Yukish", null, "gary0@adventure-works.com"),
		                                                                       new Employee("Alejandro", "McGuel", null, "alejandro0@adventure-works.com"),
		                                                                       new Employee("Jeffrey", "Ford", null, "jeffrey0@adventure-works.com") }) },
		                                                   null),
		                                    new Department("Manufacturing",
		                                                   new[] { new Department("Production",
		                                                               null,
		                                                               new[] { new Employee("JoLynn", "Dobney", null, "jolynn0@adventure-works.com"),
		                                                                       new Employee("Greg", "Alderson", null, "greg0@adventure-works.com"),
		                                                                       new Employee("Hanying", "Feng", null, "hanying0@adventure-works.com"),
		                                                                       new Employee("Michael", "Hines", null, "michael0@adventure-works.com") }),
		                                                           new Department("Production Control",
		                                                               null,
		                                                               new[] { new Employee("Yvonne", "McKay", null, "yvonne0@adventure-works.com"),
		                                                                       new Employee("Kevin", "Liu", null, "kevin1@adventure-works.com"),
		                                                                       new Employee("David", "Johnson", null, "david1@adventure-works.com"),
		                                                                       new Employee("Ruth", "Ellerbrock", null, "ruth0@adventure-works.com") }) },
		                                                   null),
		                                    new Department("Quality Assurance",
		                                                   new[] { new Department("Document Control",
		                                                               null,
		                                                               new[] { new Employee("Terri", "Duffy", null, "terri0@adventure-works.com"),
		                                                                       new Employee("Ashvini", "Sharma", null, "ashvini0@adventure-works.com"),
		                                                                       new Employee("A. Scott", "Wright", null, "ascott0@adventure-works.com"),
		                                                                       new Employee("Jim", "Scardelis", null, "jim0@adventure-works.com") }),
		                                                           new Department("Quality Assurance",
		                                                               null,
		                                                               new[] { new Employee("Carole", "Poland", null, "carole0@adventure-works.com"),
		                                                                       new Employee("Fred", "Northup", null, "fred0@adventure-works.com"),
		                                                                       new Employee("Kendall", "Keil", null, "kendall0@adventure-works.com"),
		                                                                       new Employee("Sidney", "Higa", null, "sidney0@adventure-works.com") }) },
		                                                   null),
		                                    new Department("Research and Development",
		                                                   new[] { new Department("Engineering",
		                                                               null,
		                                                               new[] { new Employee("Doris", "Hartwig", null, "doris0@adventure-works.com"),
		                                                                       new Employee("Guy", "Gilbert", null, "guy1@adventure-works.com"),
		                                                                       new Employee("Jian Shuo", "Wang", "ULL", "jianshuo0@adventure-works.com"),
		                                                                       new Employee("Christian", "Kleinerman", null, "christian0@adventure-works.com") }),
		                                                           new Department("Research and Development",
		                                                               null,
		                                                               new[] { new Employee("Tengiz", "Kharatishvili", null, "tengiz0@adventure-works.com"),
		                                                                       new Employee("Jinghao", "Liu", null, "jinghao0@adventure-works.com"),
		                                                                       new Employee("Stuart", "Munson", null, "stuart0@adventure-works.com"),
		                                                                       new Employee("David", "Bradley", null, "david0@adventure-works.com") }),
		                                                           new Department("Tool Design",
		                                                               null,
		                                                               new[] { new Employee("Kevin", "Brown", null, "kevin0@adventure-works.com"),
		                                                                       new Employee("John", "Campbell", null, "john0@adventure-works.com"),
		                                                                       new Employee("Susan", "Eaton", null, "susan0@adventure-works.com"),
		                                                                       new Employee("Janaina", "Bueno", null, "janaina0@adventure-works.com") }) },
		                                                   null) };
		#endregion

		private TreeNode CreateTreeNode(Department d) {
			TreeNode n = Tree.CreateTreeNode();
			Tree.SetTreeNodeText(n, d.name);
			Tree.SetTreeNodeData(n, d.employees);
			if (d.children != null) {
				foreach (var c in d.children) {
					TreeNode x = CreateTreeNode(c);
					Tree.AddTreeNodeChild(x, n);
				}
			}
			return n;
		}

		private void Constructed() {
			foreach (var d in data) {
				TreeNode n = CreateTreeNode(d);
				Tree.AddTreeNodeChild(n, DepartmentsTree.InvisibleRoot);
			}
			Tree.SetTreeNodeExpanded(DepartmentsTree.InvisibleRoot, true, true);
			DepartmentsTree.SelectedNode = Tree.FollowTreeNodePath(DepartmentsTree.InvisibleRoot, new int[] { 0 });
		}
#endif

#if CLIENT
		private void Constructed() {
			DepartmentsTree.SelectionChanged += new EventHandler(DepartmentsTree_SelectionChanged);
			EmployeesGrid.CellClicked += EmployeesGrid_CellClicked;
		}
		
		private void Attached() {
			jQuery.FromElement(EditEmployeeOKButton).Click(EditEmployeeOKButton_Click);
			jQuery.FromElement(EditEmployeeCancelButton).Click(delegate { EditEmployeeDialog.Close(); });
			DepartmentsTree_SelectionChanged(DepartmentsTree, EventArgs.Empty);
		}
		
		private void EmployeesGrid_CellClicked(object sender, GridCellClickedEventArgs e) {
			if (e.Col == 2) {
				EditEmployee((Employee)EmployeesGrid.GetData(e.Row));
			}
		}
		
		private string[] GetGridTexts(Employee e) {
			return new string[] { e != null ? e.firstName : "<<New>>", e != null ? e.lastName : "", e != null ? "[Edit]" : "[New]" };
		}

		private void DepartmentsTree_SelectionChanged(object sender, EventArgs _) {
			TreeNode node = DepartmentsTree.SelectedNode;
			Employee[] emps = ((node != null ? (Employee[])Tree.GetTreeNodeData(node) : null) ?? new Employee[0]);
			EmployeesGrid.BeginRebuild();
			foreach (Employee e in emps)
				EmployeesGrid.AddItem(GetGridTexts(e), e);
			EmployeesGrid.AddItem(GetGridTexts(null), null);
			EmployeesGrid.EndRebuild();
		}

		private void EditEmployee(Employee e) {
			FirstNameInput.Value = ((e != null ? e.firstName : null) ?? "");
			LastNameInput.Value  = ((e != null ? e.lastName : null) ?? "");
			TitleInput.Value     = ((e != null ? e.title : null) ?? "");
			EmailInput.Value     = ((e != null ? e.email : null) ?? "");
			EditEmployeeDialog.Open();
		}
		
		private void EditEmployeeOKButton_Click(jQueryEvent evt) {
			string firstName = FirstNameInput.Value.Trim(),
			       lastName  = LastNameInput.Value.Trim(),
			       title     = TitleInput.Value.Trim(),
			       email     = EmailInput.Value.Trim();
			if (firstName == "") {
				Window.Alert("You must enter a first name.");
				FirstNameInput.Focus();
				return;
			}
			if (lastName == "") {
				Window.Alert("You must enter a last name.");
				LastNameInput.Focus();
				return;
			}
			if (title == "")
				title = null;
			if (email == "") {
				Window.Alert("You must enter an email address.");
				EmailInput.Focus();
				return;
			}
			
			bool add = (EmployeesGrid.GetData(EmployeesGrid.SelectedRowIndex) == null);
			Employee emp = new Employee(firstName, lastName, title, email);
			EmployeesGrid.UpdateItem(EmployeesGrid.SelectedRowIndex, GetGridTexts(emp), emp);
			if (add)
				EmployeesGrid.AddItem(GetGridTexts(null), null);

			Tree.SetTreeNodeData(DepartmentsTree.SelectedNode, GetCurrentEmployees());

			EditEmployeeDialog.Close();
		}
		
		private List<Employee> GetCurrentEmployees() {
			var l = new List<Employee>();
			for (int i = 0, n = EmployeesGrid.NumRows; i < n; i++) {
				var e = (Employee)EmployeesGrid.GetData(i);
				if (e != null)
					l.Add(e);
			}
			return l;
		}

#endif
	}
}