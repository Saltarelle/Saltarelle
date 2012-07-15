using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Runtime.CompilerServices;
#if CLIENT
using System.Html;
using jQueryApi;
using jQueryApi.UI;
using jQueryApi.UI.Interactions;
#endif

namespace Saltarelle.UI {
	#if CLIENT
	public class GridSelectionChangingEventArgs : EventArgs {
		public int OldSelectionIndex;
		public int NewSelectionIndex;
		public bool Cancel;
	}
	public delegate void GridSelectionChangingEventHandler(object sender, GridSelectionChangingEventArgs e);
	
	public class GridCellClickedEventArgs : EventArgs {
		public int Row;
		public int Col;
		public bool PreventRowSelect;
	}
	public delegate void GridCellClickedEventHandler(object sender, GridCellClickedEventArgs e);
	
	public class GridKeyPressEventArgs : EventArgs {
		public int KeyCode;
		public bool PreventDefault;
	}
	public delegate void GridKeyPressEventHandler(object sender, GridKeyPressEventArgs e);
	
	public class GridDragDropCompletingEventArgs : CancelEventArgs {
		public int ItemIndex;
		public int DropIndex;
		public GridDragDropCompletingEventArgs(int itemIndex, int dropIndex) {
			this.ItemIndex = itemIndex;
			this.DropIndex = dropIndex;
			this.Cancel    = false;
		}
	}
	public delegate void GridDragDropCompletingEventHandler(object sender, GridDragDropCompletingEventArgs e);

	public class GridDragDropCompletedEventArgs : EventArgs {
		public int ItemIndex;
		public int DropIndex;
		public GridDragDropCompletedEventArgs(int itemIndex, int dropIndex) {
			this.ItemIndex = itemIndex;
			this.DropIndex = dropIndex;
		}
	}
	public delegate void GridDragDropCompletedEventHandler(object sender, GridDragDropCompletedEventArgs e);
	#endif

	public class Grid : IControl, IClientCreateControl, IResizableX, IResizableY {
		public const int BorderSize = 1;
	
		public const string DivClass = "Grid ui-widget-content";
		public const string DisabledDivClass = "DisabledGrid";
		public const string SpacerThClass = "spacer";
		public const string HeaderDivClass = "GridHeader";
		public const string HeaderTableClass = "GridHeader";
		public const string ValuesDivClass = "GridValues";
		public const string ValuesTableClass = "GridValues";
		public const string EvenRowClass = "GridRowEven";
		public const string OddRowClass = "GridRowOdd";
		public const string SelectedRowClass = "ui-state-highlight";
		public const string RowHoverClass = "DropHover";
		public const string CurrentDraggingRowClass = "CurrentDraggingRow";

		private const int HeaderHeight = 16;

		private string id;
		private List<int> colWidths = new List<int>();
		private List<string> colClasses = new List<string>();
		private List<string> colTitles = new List<string>();
		private List<List<string>> rowTextsIfNotRendered;
		private List<string> rowClassesIfNotRendered;
		private List<object> rowData;
		private Position position;
		private int width;
		private int height;
		private int tabIndex;
		private int numRows;
		private bool enabled = true;
		private bool colHeadersVisible = true;
		private bool enableDragDrop = false;
		private int selectedRowIndex = -1;
		
		#if CLIENT
			private bool isAttached = false;
			private bool rebuilding;
			private jQueryEventHandler dragFeedbackHandler;
			private Element currentDropTarget;
			private bool headersAreMadeResizable;
			private int rowHeight;	// Used for drag-drop.
		#endif

		public string Id {
			get { return id; }
			set {
				#if CLIENT
					if (isAttached)
						GetElement().ID = value;
				#endif
				id = value;
			}
		}
		
		public int TabIndex {
			get { return tabIndex; }
			set {
				#if CLIENT
					if (isAttached && enabled)
						GetElement().TabIndex = value;
				#endif
				tabIndex = value;
			}
		}
		
		public Position Position {
			get {
				#if CLIENT
					return isAttached ? PositionHelper.GetPosition(GetElement()) : position;
				#else
					return position;
				#endif
			}
			set {
				position = value;
				#if CLIENT
					if (isAttached)
						PositionHelper.ApplyPosition(GetElement(), value);
				#endif
			}
		}

		public int Width {
			get {
				return width;
			}
			set {
				#if CLIENT
					if (isAttached) {
						jQuery.FromElement(GetElement()).Children("div").AndSelf().Width(value - 2 * BorderSize);
					}
				#endif
				width = value;
			}
		}

		public int Height {
			get {
				return height;
			}
			set {
				#if CLIENT
					if (isAttached)
						jQuery.FromElement(GetElement().Children[1]).Height(value - 2 * BorderSize - (colHeadersVisible ? HeaderHeight : 0));
				#endif
				height = value;
			}
		}

		private void ResizeList<T>(List<T> l, int size, T def) {
			if (l.Count > size) {
				l.RemoveRange(size, l.Count - size);
			}
			else {
				while (l.Count < size)
					l.Add(def);
			}
		}

		public int NumColumns {
			get {
				return colWidths.Count;
			}
			set {
				if (value == colWidths.Count)
					return;
			
				if (NumRows > 0)
					throw new Exception("Can only change number of columns when the grid is empty");
			
				ResizeList(colWidths, value, 100);
				ResizeList(colTitles, value, "");
				ResizeList(colClasses, value, "");
				#if CLIENT
					if (isAttached) {
						jQuery.FromElement(GetElement()).Html(InnerHtml);
						AttachInner(GetElement());
					}
				#endif
			}
		}

		public void SetColTitle(int col, string title) {
			#if CLIENT
				if (isAttached) {
					jQuery.FromElement(GetHeaderRow().Cells[col]).Children("div").Children("div:eq(0)").Text(title);
				}
			#endif
			colTitles[col] = title;
		}
		
		public string GetColTitle(int col) {
			return colTitles[col];
		}
		
		public IList<string> ColTitles {
			get {
				return new List<string>(colTitles);
			}
			set {
				if (value.Count != NumColumns)
					NumColumns = value.Count;
				for (int i = 0; i < NumColumns; i++)
					SetColTitle(i, value[i]);
			}
		}

		public void SetColWidth(int col, int width) {
			#if CLIENT
				if (isAttached) {
					GetHeaderRow().Cells[col].Children[0].Style.Width = width + "px";
					var rows = GetAllRows();
					for (int i = 0; i < rows.Length; i++)
						((TableRowElement)rows[i]).Cells[col].Children[0].Style.Width = width + "px";
					jQuery.FromElement(GetElement().Children[1]).Scroll();
				}
			#endif
			colWidths[col] = width;
		}
		
		public int GetColWidth(int col) {
			return colWidths[col];
		}
		
		public IList<int> ColWidths {
			get {
				var result = new List<int>();
				for (int i = 0; i < result.Count; i++)
					result.Add(GetColWidth(i));
				return result;
			}
			set {
				if (value.Count != NumColumns)
					NumColumns = value.Count;
				for (int i = 0; i < NumColumns; i++)
					SetColWidth(i, value[i]);
			}
		}
		
		public void SetColClass(int col, string cls) {
			if (col < 0 || col >= colClasses.Count)
				return;
			#if CLIENT
				if (isAttached) {
					var headerCell = jQuery.FromElement(GetHeaderRow().Cells[col]);
					if (!string.IsNullOrEmpty(colClasses[col]))
						headerCell.RemoveClass(colClasses[col]);
					if (!string.IsNullOrEmpty(cls))
						headerCell.AddClass(cls);

					var rows = GetAllRows();
					for (int i = 0; i < rows.Length; i++) {
						var q = jQuery.FromElement(((TableRowElement)rows[i]).Cells[col]);
						if (!string.IsNullOrEmpty(colClasses[col]))
							q.RemoveClass(colClasses[col]);
						if (!string.IsNullOrEmpty(cls))
							q.AddClass(cls);
					}
				}
			#endif
			colClasses[col] = cls;
		}
		
		public string GetColClass(int col) {
			return colClasses[col];
		}
		
		public IList<string> ColClasses {
			get {
				var result = new List<string>();
				for (int i = 0; i < NumColumns; i++)
					result.Add(GetColClass(i));
				return result;
			}
			set {
				if (value.Count != NumColumns)
					NumColumns = value.Count;
				for (int i = 0; i < NumColumns; i++)
					SetColClass(i, value[i]);
			}
		}
		
		public bool Enabled {
			get { return enabled; }
			set {
				#if CLIENT
					if (isAttached && value != enabled) {
						if (selectedRowIndex != -1) {
							if (value && enableDragDrop) {
								MakeDraggable(jQuery.FromElement(SelectedRow));
								EnableDroppable(true);
							}
							else {
								((DraggableObject)jQuery.FromElement(SelectedRow)).Destroy();
								EnableDroppable(false);
							}
						}
						
						var elem = jQuery.FromElement(GetElement());

						if (value) {
							elem.RemoveClass(DisabledDivClass);
							elem.Attribute("tabindex", Utils.ToStringInvariantInt(tabIndex));
						}
						else {
							elem.AddClass(DisabledDivClass);
							elem.RemoveAttr("tabindex");
						}
					}
				#endif						
				enabled = value;
			}
		}
		
		private int ContentHeight { get { return height - 2 * BorderSize - (colHeadersVisible ? HeaderHeight : 0); } }
		
		public bool ColHeadersVisible {
			get { return colHeadersVisible; }
			set {
				colHeadersVisible = value;
				#if CLIENT
					if (isAttached) {
						var elem = GetElement();
						elem.Children[0].Style.Display = (colHeadersVisible ? "" : "none");
						jQuery.FromElement(elem.Children[1]).Height(ContentHeight);
						if (colHeadersVisible && !headersAreMadeResizable)
							MakeHeadersResizable();
					}
				#endif
			}
		}
		
		public bool EnableDragDrop {
			get {
				return enableDragDrop;
			}
			set {
				#if CLIENT
					if (isAttached && value != enableDragDrop) {
						if (value && enabled) {
							if (selectedRowIndex != -1)
								MakeDraggable(jQuery.FromElement(SelectedRow));
							EnableDroppable(true);
						}
						else {
							if (selectedRowIndex != -1)
								((DraggableObject)jQuery.FromElement(SelectedRow)).Destroy();
							EnableDroppable(false);
						}
					}
				#endif
				enableDragDrop = value;
			}
		}

		public void AddItem(IList<string> cellTexts, object data) {
			InsertItem(numRows, cellTexts, data);
		}
		
		public void InsertItem(int index, IList<string> cellTexts, object data) {
			numRows++;
			if (selectedRowIndex >= index)
				selectedRowIndex++;

			rowData.Insert(index, data);

			#if CLIENT
				if (isAttached && !rebuilding) {
					var sb = new StringBuilder();
					AddRowHtml(sb, cellTexts, null, (numRows % 2) == 1, false);
					var q = jQuery.FromHtml(sb.ToString());
					if (index == numRows - 1) // remember we already incremented numRows
						q.AppendTo(GetValuesTBody());
					else
						q.InsertBefore(GetValuesTBody().Rows[index]);

					for (var next = q.Next(); next.Size() > 0; next = next.Next()) {
						if (next.Is("." + EvenRowClass)) {
							next.RemoveClass(EvenRowClass);
							next.AddClass(OddRowClass);
						}
						else {
							next.RemoveClass(OddRowClass);
							next.AddClass(EvenRowClass);
						}
					}

					return;
				}
			#endif
			var l = new List<string>();
			l.AddRange(cellTexts);
			rowTextsIfNotRendered.Insert(index, l);
			rowClassesIfNotRendered.Insert(index, null);
		}
		
		public void SetRowClass(int index, string rowClass) {
			if (index < 0 || index >= numRows)
				return;
			
			#if CLIENT
				if (isAttached && !rebuilding) {
					var tr = (TableRowElement)GetValuesTBody().Rows[index];
					var classes = new List<string>((string[])tr.ClassName.Split(" ").Filter(delegate(object x) { var s = (string)x; return s == EvenRowClass || s == OddRowClass || s == SelectedRowClass; }));
					classes.Add(rowClass);
					tr.ClassName = classes.Join(" ");
					return;
				}
			#endif
			rowClassesIfNotRendered[index] = rowClass;
		}

		public string GetRowClass(int index) {
			if (index < 0 || index >= numRows)
				return null;
			
			#if CLIENT
				if (isAttached && !rebuilding) {
					TableRowElement tr = (TableRowElement)GetValuesTBody().Rows[index];
					var classes = (string[])tr.ClassName.Split(" ").Filter(delegate(object x) { string s = (string)x; return s == EvenRowClass || s == OddRowClass || s == SelectedRowClass; });
					return classes.Length > 0 ? classes[0] : null;
				}
			#endif
			return (string)rowClassesIfNotRendered[index];
		}

		public void BeginRebuild() {
			#if CLIENT
				rebuilding = true;
				if (rowTextsIfNotRendered == null)
					rowTextsIfNotRendered = new List<List<string>>();
				if (rowClassesIfNotRendered == null)
					rowClassesIfNotRendered = new List<string>();
			#endif
			Clear();
		}
		
		public void EndRebuild() {
			#if CLIENT
				rebuilding = false;
				
				if (isAttached) {
					StringBuilder sb = new StringBuilder();
					for (int i = 0; i < rowTextsIfNotRendered.Count; i++) {
						AddRowHtml(sb, rowTextsIfNotRendered[i], (string)rowClassesIfNotRendered[i], (i % 2) == 0, i == selectedRowIndex);
					}
					jQuery.FromElement(GetValuesTBody()).Html(sb.ToString());
					
					if (selectedRowIndex >= 0) {
						EnsureVisible(selectedRowIndex);
						if (enableDragDrop && enabled)
							MakeDraggable(jQuery.FromElement(SelectedRow));
					}

					rowTextsIfNotRendered   = null;
					rowClassesIfNotRendered = null;
				}
			#endif
		}
		
		public void Clear() {
			numRows = 0;
			selectedRowIndex = -1;
			#if CLIENT
				if (isAttached) {
					jQuery.FromElement(GetValuesTBody()).Empty();
					OnSelectionChanged(EventArgs.Empty);
				}
			#endif
			if (rowTextsIfNotRendered != null)
				rowTextsIfNotRendered.Clear();
			if (rowClassesIfNotRendered != null)
				rowClassesIfNotRendered.Clear();
			rowData.Clear();
		}
		
		public void UpdateItem(int row, IList<string> cellTexts, object data) {
			rowData[row] = data;
			#if CLIENT
				if (isAttached && !rebuilding) {
					var tr = (TableRowElement)GetValuesTBody().Rows[row];
					for (int i = 0; i < tr.Cells.Length; i++) {
						string text = i < cellTexts.Count ? cellTexts[i] : "";
						tr.Cells[i].Children[0].Children[0].InnerHTML = !string.IsNullOrEmpty(text) ? Utils.HtmlEncode(text) : "&nbsp;";
					}
					return;
				}
			#endif
			var l = new List<string>();
			l.AddRange(cellTexts);
			rowTextsIfNotRendered[row] = l;
		}
		
		public void DeleteItem(int row) {
			numRows--;
			rowData.RemoveAt(row);
			#if CLIENT
				if (isAttached && !rebuilding) {
					int newSelection = SelectedRowIndex;
					bool changeSelection = false;
					if (SelectedRowIndex == row) {
						if (numRows > 0) {
							if (newSelection == numRows)
								newSelection = numRows - 1;
						}
						else
							newSelection = -1;
						changeSelection = true;
					}
					jQueryObject q = jQuery.FromElement(GetValuesTBody().Rows[row]).Remove(), next = q.Next();
					q.Remove();
					for (; next.Size() > 0; next = next.Next()) {
						if (next.Is("." + EvenRowClass)) {
							next.RemoveClass(EvenRowClass);
							next.AddClass(OddRowClass);
						}
						else {
							next.RemoveClass(OddRowClass);
							next.AddClass(EvenRowClass);
						}
					}
					if (changeSelection) {
						selectedRowIndex = -1; // hack to make the next procedure sure the GUI must be updated
						SelectedRowIndex = newSelection;
						OnSelectionChanged(EventArgs.Empty);
					}
					return;
				}
			#endif
			if (selectedRowIndex >= row)
				selectedRowIndex--;
			rowTextsIfNotRendered.RemoveAt(row);
			rowClassesIfNotRendered.RemoveAt(row);
		}
		
		public object GetData(int row) {
			if (row < 0 || row >= numRows)
				return null;
			return rowData[row];
		}

		public IList<string> GetTexts(int row) {
			if (row < 0 || row >= numRows)
				return null;
			#if CLIENT
				if (isAttached && !rebuilding) {
					TableRowElement tr = (TableRowElement)GetValuesTBody().Rows[row];
					var result = new List<string>();
					for (int i = 0; i < tr.Cells.Length; i++)
						result.Add(jQuery.FromElement(tr.Cells[i]).GetText());
					return result;
				}
				return rowTextsIfNotRendered[row];
			#else
				return new List<string>(rowTextsIfNotRendered[row]);
			#endif
		}
		
		public int NumRows {
			get {
				return numRows;
			}
		}
		
		private void AddRowHtml(StringBuilder sb, IList<string> cellTexts, string addClass, bool even, bool selected) {
			sb.Append("<tr class=\"" + (even ? EvenRowClass : OddRowClass) + (selected ? " " + SelectedRowClass : "") + (!string.IsNullOrEmpty(addClass) ? " " + addClass : "") + "\">");
			for (int c = 0; c < NumColumns; c++)
				sb.Append("<td " + (string.IsNullOrEmpty(colClasses[c]) ? "" : (" class=\"" + colClasses[c] + "\"")) + "><div style=\"width: " + Utils.ToStringInvariantInt(colWidths[c]) + "px\"><div>" + (c < cellTexts.Count && !string.IsNullOrEmpty(cellTexts[c]) ? Utils.HtmlEncode(cellTexts[c]) : "&nbsp;") + "</div></div></td>");
			sb.Append("</tr>");
		}
		
		private string InnerHtml {
			get {
				StringBuilder sb = new StringBuilder();
				// position: relative on the header div solves an IE6 rendering bug.
				sb.Append("<div class=\"" + HeaderDivClass + "\" style=\"position: relative; width: " + (this.width - 2 * BorderSize) + "px; height: " + Utils.ToStringInvariantInt(HeaderHeight) + "px" + (colHeadersVisible ? "" : "; display: none") + "\"><table cellpadding=\"0\" cellspacing=\"0\" class=\"" + HeaderTableClass + "\"><thead><tr>");
				for (int c = 0; c < NumColumns; c++)
					sb.Append("<th " + (string.IsNullOrEmpty(colClasses[c]) ? "" : (" class=\"" + colClasses[c] + "\"")) + "><div style=\"width: " + Utils.ToStringInvariantInt(colWidths[c]) + "px\"><div>" + (!string.IsNullOrEmpty(colTitles[c]) ? Utils.HtmlEncode(colTitles[c]) : "&nbsp;") + "</div></div></th>");
				sb.Append("<th class=\"" + SpacerThClass + "\"><div>&nbsp;</div></th></tr></thead></table></div><div class=\"" + ValuesDivClass + "\" style=\"width: " + (this.width - 2 * BorderSize) + "px; height: " + Utils.ToStringInvariantInt(ContentHeight) + "px\"><table cellpadding=\"0\" cellspacing=\"0\" class=\"" + ValuesTableClass + "\"><tbody>");
				if (rowTextsIfNotRendered != null) {
					for (int i = 0; i < rowTextsIfNotRendered.Count; i++) {
						AddRowHtml(sb, rowTextsIfNotRendered[i], (string)rowClassesIfNotRendered[i], (i % 2) == 0, i == selectedRowIndex);
					}
				}
				sb.Append("</tbody></table></div>");
				return sb.ToString();
			}
		}
		
		public virtual string Html {
			get {
				if (string.IsNullOrEmpty(id))
					throw new Exception("Must set ID before render");
				string style = PositionHelper.CreateStyle(position, width - 2 * BorderSize, -1);
				return "<div id=\"" + id + "\" class=\"" + DivClass + (enabled ? "" : (" " + DisabledDivClass)) + "\" style=\"" + style + "\"" + (enabled ? " tabindex=\"" + Utils.ToStringInvariantInt(tabIndex) + "\"" : "") + ">"
				     +     InnerHtml
				     + "</div>";
			}
		}
		
		protected virtual void InitDefault() {
			position = PositionHelper.NotPositioned;
			width = 300;
			height = 300;
			rowTextsIfNotRendered   = new List<List<string>>();
			rowClassesIfNotRendered = new List<string>();
			rowData = new List<object>();
		}
		
#if SERVER
		public Grid() {
			InitDefault();
		}

		protected virtual void AddItemsToConfigObject(Dictionary<string, object> config) {
			config["id"] = id;
			config["colTitles"] = colTitles;
			config["colWidths"] = colWidths;
			config["width"] = width;
			config["height"] = height;
			config["tabIndex"] = tabIndex;
			config["numRows"] = numRows;
			config["colClasses"] = colClasses;
			config["enabled"] = enabled;
			config["colHeadersVisible"] = colHeadersVisible;
			config["enableDragDrop"] = enableDragDrop;
			config["selectedRowIndex"] = selectedRowIndex;
			config["rowData"] = rowData;
		}

		public object ConfigObject {
			get {
				var config = new Dictionary<string, object>();
				AddItemsToConfigObject(config);
				return config;
			}
		}

		public int SelectedRowIndex {
			get {
				return selectedRowIndex;
			}
			set {
				if (selectedRowIndex >= numRows)
					throw new ArgumentException("value");
				selectedRowIndex = value;
			}
		}
#endif

#if CLIENT
		[AlternateSignature]
		public Grid() {}
		public Grid(object config) {
			dragFeedbackHandler = new jQueryEventHandler(ValuesDiv_DragFeedback);
			if (!Script.IsUndefined(config)) {
				InitConfig(JsDictionary.GetDictionary(config));
			}
			else
				InitDefault();
		}

		public event GridKeyPressEventHandler KeyPress;
		public event GridSelectionChangingEventHandler SelectionChanging;
		public event GridCellClickedEventHandler CellClicked;
		public event EventHandler SelectionChanged;
		public event GridDragDropCompletingEventHandler DragDropCompleting;
		public event GridDragDropCompletedEventHandler DragDropCompleted;
		
		protected virtual void InitConfig(JsDictionary config) {
			id = (string)config["id"];
			colTitles = (List<string>)config["colTitles"];
			colWidths = (List<int>)config["colWidths"];
			colClasses = (List<string>)config["colClasses"];
			width = (int)config["width"];
			height = (int)config["height"];
			tabIndex = (int)config["tabIndex"];
			numRows = (int)config["numRows"];
			enabled = (bool)config["enabled"];
			colHeadersVisible = (bool)config["colHeadersVisible"];
			enableDragDrop = (bool)config["enableDragDrop"];
			selectedRowIndex = (int)config["selectedRowIndex"];
			rowData = (List<object>)config["rowData"];

			Attach();
		}
		
		public Element GetElement() { return isAttached ? Document.GetElementById(id) : null; }
		
		private TableRowElement SelectedRow {
			get { return (TableRowElement)GetValuesTBody().Rows[selectedRowIndex]; }
		}

		public int SelectedRowIndex {
			get {
				return selectedRowIndex;
			}
			set {
				if (selectedRowIndex == value)
					return;
				if (!RaiseSelectionChanging(value))
					return;
					
				if (selectedRowIndex != -1 && isAttached && !rebuilding) {
					var row = jQuery.FromElement(SelectedRow);
					row.RemoveClass(SelectedRowClass);
					if (enableDragDrop)
						((DraggableObject)row).Destroy();
				}
				selectedRowIndex = value;
				if (selectedRowIndex != -1 && isAttached && !rebuilding) {
					EnsureVisible(selectedRowIndex);
					var row = jQuery.FromElement(SelectedRow);
					row.AddClass(SelectedRowClass);
					if (enableDragDrop && enabled)
						MakeDraggable(row);
				}
				OnSelectionChanged(EventArgs.Empty);
			}
		}
		
		public void EnsureVisible(int rowIndex) {
			var row = jQuery.FromElement(GetValuesTBody().Rows[rowIndex]);
			var valuesDiv = jQuery.FromElement(GetElement().Children[1]);
			Element d = valuesDiv.GetElement(0);
			int offsetTop = row.GetOffset().Top - valuesDiv.GetOffset().Top, scrollTop = valuesDiv.GetScrollTop(), rowHeight = row.GetHeight(), tblHeight = d.ClientHeight;

			if (offsetTop < 0) {
				valuesDiv.ScrollTop(scrollTop + offsetTop);
			}
			else if (offsetTop + rowHeight > tblHeight) {
				valuesDiv.ScrollTop(scrollTop + offsetTop + rowHeight - tblHeight);
			}
		}
		
		private void ChangeDropTarget(Element newTarget) {
			if (newTarget == currentDropTarget)
				return;
			if (currentDropTarget != null && currentDropTarget.TagName.ToLowerCase() == "tr")
				jQuery.FromElement(currentDropTarget).RemoveClass(RowHoverClass);
			if (newTarget != null && newTarget.TagName.ToLowerCase() == "tr")
				jQuery.FromElement(newTarget).AddClass(RowHoverClass);
			currentDropTarget = newTarget;
		}
		
		private void ValuesDiv_DragFeedback(jQueryEvent evt) {
			Element valuesDiv = GetElement().Children[1], newDropTarget = null;

			int valuesDivTop = (int)jQuery.FromElement(valuesDiv).GetOffset().Top;
			int offset = evt.PageY - valuesDivTop + valuesDiv.ScrollTop;
			int rowIndex = Math.Truncate(offset / (float)rowHeight);	// Need to do this because Script# doesn't do integer division correctly.
			if (rowIndex > selectedRowIndex)
				rowIndex++;

			if (rowIndex >= 0 && rowIndex < numRows)
				newDropTarget = ((TableElement)valuesDiv.Children[0]).Rows[rowIndex];
			else
				newDropTarget = valuesDiv;

			ChangeDropTarget(newDropTarget);
		}
		
		private void ValuesDiv_Drop(jQueryEvent evt, DropEvent ui) {
			if (currentDropTarget == null) {
				DragEnded();
				return;
			}
			
			int draggingIndex = selectedRowIndex;

			Element valuesDiv = GetElement().Children[1];
			Element draggedElem = ui.Draggable.GetElement(0);
			Element valuesTbodyEl = GetValuesTBody();

			if (currentDropTarget == valuesDiv) {
				// Dropping as the last element
				if (selectedRowIndex == NumRows - 1) {
					DragEnded();
					return;
				}

				GridDragDropCompletingEventArgs e = new GridDragDropCompletingEventArgs(draggingIndex, NumRows - 1);
				OnDragDropCompleting(e);
				if (e.Cancel) {
					DragEnded();
					return;
				}

				valuesTbodyEl.RemoveChild(draggedElem);
				valuesTbodyEl.AppendChild(draggedElem);
				object data = rowData[draggingIndex];
				rowData.RemoveAt(draggingIndex);
				rowData.Add(data);
				selectedRowIndex = NumRows - 1;
				GetElement().Focus();

				OnDragDropCompleted(new GridDragDropCompletedEventArgs(draggingIndex, NumRows - 1));
			}
			else {
				// Dropping on a row
				int newIndex = ((TableRowElement)currentDropTarget).RowIndex;
				newIndex = (newIndex > selectedRowIndex ? newIndex - 1 : newIndex); // If dragging down we have to pretend that the original row does not exist.
				if (newIndex == selectedRowIndex)
					return;

				GridDragDropCompletingEventArgs e = new GridDragDropCompletingEventArgs(draggingIndex, newIndex);
				OnDragDropCompleting(e);
				if (e.Cancel) {
					DragEnded();
					return;
				}

				valuesTbodyEl.RemoveChild(draggedElem);
				valuesTbodyEl.InsertBefore(draggedElem, currentDropTarget);
				object data = rowData[draggingIndex];
				rowData.RemoveAt(draggingIndex);
				rowData.Insert(newIndex, data);
				selectedRowIndex = newIndex;
				GetElement().Focus();

				OnDragDropCompleted(new GridDragDropCompletedEventArgs(draggingIndex, newIndex));
			}

			DragEnded();
		}
		
		private void DragEnded() {
			ChangeDropTarget(null);
			jQuery.Document.Unbind("mousemove", dragFeedbackHandler);
		}
		
		private void MakeDraggable(jQueryObject row) {
			row.Draggable(new DraggableOptions { Helper      = "clone",
			                                     AppendTo    = row.Parent().GetElement(0),
			                                     Scroll      = true,
			                                     Containment = "parent",
			                                     OnStart     = new jQueryUIEventHandler<DragStartEvent>(Delegate.ThisFix((Action<Element, jQueryEvent, DragStartEvent>)((d, evt, ui) => { jQuery.FromElement(d).AddClass(CurrentDraggingRowClass); }))),
			                                     OnStop      = new jQueryUIEventHandler<DragStopEvent>(Delegate.ThisFix((Action<Element, jQueryEvent, DragStopEvent>)((d, evt, ui) => { jQuery.FromElement(d).RemoveClass(CurrentDraggingRowClass); }))) });
		}
		
		private TableRowElement GetHeaderRow() {
			return (TableRowElement)((TableElement)GetElement().Children[0].Children[0]).Rows[0];
		}

		private TableSectionElement GetValuesTBody() {
			return (TableSectionElement)((TableElement)GetElement().Children[1].Children[0]).tBodies[0];
		}
		
		private ElementCollection GetAllRows() {
			return ((TableSectionElement)((TableElement)GetElement().Children[1].Children[0]).tBodies[0]).Rows;
		}

		private void EnableDroppable(bool enable) {
			var el = jQuery.FromElement(GetElement().Children[1]);
			if (enable) {
				el.Droppable(new DroppableOptions { Tolerance = "pointer",
				                                    Greedy    = true,
				                                    OnOver    = (_1, _2) => {
				                                                                var rows = GetAllRows();
				                                                                rowHeight = (rows.Length > 1 ? Math.Max(rows[0].OffsetHeight, rows[1].OffsetHeight) : 1);	// We need to take the maximum of two rows because one of them might be the currently dragged one, which is hidden.
				                                                                currentDropTarget = null;
				                                                                jQuery.Document.MouseMove(dragFeedbackHandler);
				                                                            },
				                                    OnOut     = (_1, _2) => DragEnded(),
				                                    OnDrop    = ValuesDiv_Drop });
			}
			else {
				((DroppableObject)el).Destroy();
			}
		}

		private void MakeHeadersResizable() {
			var headerTr = jQuery.FromElement(((TableElement)GetElement().Children[0].Children[0]).Rows[0]);
			headerTr.Children(":not(:last-child)").Children().Resizable(new ResizableOptions {
				Handles = "e",
				OnStop  = new jQueryUIEventHandler<ResizeStopEvent>(Delegate.ThisFix((Action<Element, jQueryEvent, ResizeStopEvent>)((d, evt, ui) => {
			                  int index = headerTr.Children().Index(d.ParentNode);
			                  SetColWidth(index, Math.Round(ui.Size.Width));
			              })))
			});
			if (jQuery.Browser.MSIE && Utils.ParseDouble(jQuery.Browser.Version) < 8)
				headerTr.Find(".ui-resizable-e").Height(HeaderHeight);
		}
		
		private void AttachInner(Element element) {
			if (enableDragDrop && enabled) {
				if (selectedRowIndex >= 0)
					MakeDraggable(jQuery.FromElement(SelectedRow));
				EnableDroppable(true);
			}

			if (colHeadersVisible) {
				var headerTr = jQuery.FromElement(((TableElement)GetElement().Children[0].Children[0]).Rows[0]);
				headerTr.One("mouseover", null, evt => {
					// Delaying this improves load time by perhaps 50 or so ms (which can make a difference).
					if (!headersAreMadeResizable)
						MakeHeadersResizable();
					headersAreMadeResizable = true;
				});
			}

			jQuery.FromElement(element.Children[1].Children[0]).Click(ValueTable_Click);

			jQuery.FromElement(element.Children[1]).Scroll(delegate {
				Element elem = GetElement();
				jQuery.FromElement(elem.Children[0]).ScrollLeft(jQuery.FromElement(elem.Children[1]).GetScrollLeft());
			});
		}
		
		private void ValueTable_Click(jQueryEvent evt) {
			if (!enabled)
				return;
			if (evt.Target == GetElement().Children[1].Children[0])	// Sometimes it is possible for the user to click on the table, as opposed to a table row.
				return;

			var cell = jQuery.FromElement(evt.Target).Closest("td");
			var row  = cell.Closest("tr");

			int rowIndex = ((TableRowElement)row.GetElement(0)).RowIndex;

			GridCellClickedEventArgs ea = new GridCellClickedEventArgs();
			ea.Row = rowIndex;
			ea.PreventRowSelect = false;
			ea.Col = ((TableCellElement)cell.GetElement(0)).CellIndex;

			OnCellClicked(ea);
			if (!ea.PreventRowSelect)
				SelectedRowIndex = rowIndex;
		}
		
		public void Attach() {
			if (id == null || isAttached)
				throw new Exception("Must set ID and can only attach once");
			isAttached = true;
			var elem = GetElement();

			AttachInner(elem);

			UIUtils.AttachKeyPressHandler(elem, el_KeyDown);
		}

		private void el_KeyDown(jQueryEvent e) {
			if (!enabled)
				return;

			GridKeyPressEventArgs ev = new GridKeyPressEventArgs();
			ev.KeyCode = e.Which;
			OnKeyPress(ev);
			if (ev.PreventDefault) {
				e.PreventDefault();
				return;
			}

			switch (e.Which) {
				case 38:
					// key up
					if (NumRows > 0 && selectedRowIndex > 0)
						SelectedRowIndex = (selectedRowIndex == -1 ? 0 : SelectedRowIndex - 1);
					e.PreventDefault();
					break;

				case 40:
					// key down
					if (NumRows > 0 && selectedRowIndex < NumRows - 1)
						SelectedRowIndex = (SelectedRowIndex == -1 ? 0 : SelectedRowIndex + 1);
					e.PreventDefault();
					break;
			}
		}

		private bool RaiseSelectionChanging(int newSelection) {
			GridSelectionChangingEventArgs e = new GridSelectionChangingEventArgs();
			e.OldSelectionIndex = selectedRowIndex;
			e.NewSelectionIndex = newSelection;
			OnSelectionChanging(e);
			return !e.Cancel;
		}

		protected virtual void OnSelectionChanging(GridSelectionChangingEventArgs e) {
			if (SelectionChanging != null)
				SelectionChanging(this, e);
		}
		
		protected virtual void OnSelectionChanged(EventArgs e) {
			if (SelectionChanged != null)
				SelectionChanged(this, e);
		}

		protected virtual void OnCellClicked(GridCellClickedEventArgs e) {
			if (CellClicked != null)
				CellClicked(this, e);
		}
		
		protected virtual void OnKeyPress(GridKeyPressEventArgs e) {
			if (KeyPress != null)
				KeyPress(this, e);
		}
		
		protected virtual void OnDragDropCompleting(GridDragDropCompletingEventArgs e) {
			if (DragDropCompleting != null)
				DragDropCompleting(this, e);
		}

		protected virtual void OnDragDropCompleted(GridDragDropCompletedEventArgs e) {
			if (DragDropCompleted != null)
				DragDropCompleted(this, e);
		}

		public void Focus() {
			if (isAttached)
				GetElement().Focus();
		}
#endif
	}
}
