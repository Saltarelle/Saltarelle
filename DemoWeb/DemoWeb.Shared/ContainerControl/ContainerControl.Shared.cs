using System;
using Saltarelle;
#if CLIENT
using System.XML;
using Saltarelle.UI;
#endif

namespace DemoWeb {
	[Record]
	public sealed class DataItem {
		public string s;
		public int i;
		public DataItem(string s, int i) {
			this.s = s;
			this.i = i;
		}
	}

	public partial class ContainerControl : IControl, IContainerControl {
#if SERVER
		private void AddNodesToTree(int parentId, int lvl) {
			if (lvl < 3) {
				for (int i = 0; i < 5; i++) {
					int newId = tree.AddNode(parentId, "Text " + i.ToString(), new DataItem(lvl.ToString() + ", " + i.ToString(), lvl * 10 + i), true);
					AddNodesToTree(newId, lvl + 1);
				}
			}
		}

		private void Init() {
			AddNodesToTree(0, 0);
		
			for (int i = 0; i < 10; i++)
				grid.AddItem(new string[] { "String " + Utils.ToStringInvariantInt(i), Utils.ToStringInvariantInt(i) }, new DataItem("String " + Utils.ToStringInvariantInt(i), i));
		}
#endif
#if CLIENT
		private void Init() {
			grid.CellClicked += grid_CellClicked;
			tree.SelectionChanged += tree_SelectionChanged;
			showDialogButton.click(showDialogButton_Click);
		}

		void tree_SelectionChanged(object sender, EventArgs e) {
			DataItem di = (DataItem)tree.SelectedData;
			if (di != null)
				Script.Alert(di.s);
		}

		void grid_CellClicked(object sender, GridCellClickedEventArgs e) {
			DataItem d = (DataItem)grid.GetData(e.Row);
			if (e.Col == 0)
				Script.Alert(d.s);
			else
				Script.Alert(d.i);
		}
		
		void showDialogButton_Click(JQueryEvent evt) {
			dialog.Open();
		}
#endif
	}
}