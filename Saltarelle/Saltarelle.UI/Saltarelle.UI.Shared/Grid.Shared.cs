#if SERVER
using StringArrayList = System.Collections.Generic.List<string[]>;
using StringList = System.Collections.Generic.List<string>;
using ObjectList = System.Collections.Generic.List<object>;
using System.Text;
using System;
using System.Collections.Generic;
#elif CLIENT
using StringArrayList = System.ArrayList;
using StringList = System.ArrayList;
using ObjectList = System.ArrayList;
using System;
using System.DHTML;
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
		private int[] colWidths = new int[0];
		private string[] colClasses = new string[0];
		private string[] colTitles = new string[0];
		private StringArrayList rowTextsIfNotRendered;
		private StringList      rowClassesIfNotRendered;
		private ObjectList      rowData;
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
			private JQueryEventHandlerDelegate dragFeedbackHandler;
			private DOMElement currentDropTarget;
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
						JQueryProxy.jQuery(GetElement()).children("div").andSelf().width(value - 2 * BorderSize);
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
						JQueryProxy.jQuery(GetElement().Children[1]).height(value - 2 * BorderSize - (colHeadersVisible ? HeaderHeight : 0));
				#endif
				height = value;
			}
		}

		public int NumColumns {
			get {
				return colWidths.Length;
			}
			set {
				if (value == colWidths.Length)
					return;
			
				if (NumRows > 0)
					throw new Exception("Can only change number of columns when the grid is empty");
			
				colWidths = (int[])Utils.ArrayResize(colWidths, value, 100);
				colTitles = (string[])Utils.ArrayResize(colTitles, value, "");
				colClasses = (string[])Utils.ArrayResize(colClasses, value, "");
				#if CLIENT
					if (isAttached) {
						JQueryProxy.jQuery(GetElement()).html(InnerHtml);
						AttachInner(GetElement());
					}
				#endif
			}
		}

		public void SetColTitle(int col, string title) {
			#if CLIENT
				if (isAttached) {
					JQueryProxy.jQuery(GetHeaderRow().Cells[col]).children("div").children("div:eq(0)").text(title);
				}
			#endif
			colTitles[col] = title;
		}
		
		public string GetColTitle(int col) {
			return colTitles[col];
		}
		
		public string[] ColTitles {
			get {
				return (string[])colTitles.Clone();
			}
			set {
				if (value.Length != NumColumns)
					NumColumns = value.Length;
				for (int i = 0; i < NumColumns; i++)
					SetColTitle(i, value[i]);
			}
		}

		public void SetColWidth(int col, int width) {
			#if CLIENT
				if (isAttached) {
					GetHeaderRow().Cells[col].Children[0].Style.Width = width + "px";
					TableRowElement[] rows = GetAllRows();
					for (int i = 0; i < rows.Length; i++)
						rows[i].Cells[col].Children[0].Style.Width = width + "px";
					JQueryProxy.jQuery(GetElement().Children[1]).scroll();
				}
			#endif
			colWidths[col] = width;
		}
		
		public int GetColWidth(int col) {
			return colWidths[col];
		}
		
		public int[] ColWidths {
			get {
				int[] result = new int[NumColumns];
				for (int i = 0; i < result.Length; i++)
					result[i] = GetColWidth(i);
				return result;
			}
			set {
				if (value.Length != NumColumns)
					NumColumns = value.Length;
				for (int i = 0; i < NumColumns; i++)
					SetColWidth(i, value[i]);
			}
		}
		
		public void SetColClass(int col, string cls) {
			if (col < 0 || col > colClasses.Length)
				return;
			#if CLIENT
				if (isAttached) {
					var headerCell = JQueryProxy.jQuery(GetHeaderRow().Cells[col]);
					if (!string.IsNullOrEmpty(colClasses[col]))
						headerCell.removeClass(colClasses[col]);
					if (!string.IsNullOrEmpty(cls))
						headerCell.addClass(cls);

					TableRowElement[] rows = GetAllRows();
					for (int i = 0; i < rows.Length; i++) {
						jQuery q = JQueryProxy.jQuery(rows[i].Cells[col]);
						if (!string.IsNullOrEmpty(colClasses[col]))
							q.removeClass(colClasses[col]);
						if (!string.IsNullOrEmpty(cls))
							q.addClass(cls);
					}
				}
			#endif
			colClasses[col] = cls;
		}
		
		public string GetColClass(int col) {
			return colClasses[col];
		}
		
		public string[] ColClasses {
			get {
				string[] result = new string[NumColumns];
				for (int i = 0; i < result.Length; i++)
					result[i] = GetColClass(i);
				return result;
			}
			set {
				if (value.Length != NumColumns)
					NumColumns = value.Length;
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
								MakeDraggable(JQueryProxy.jQuery(SelectedRow));
								EnableDroppable(true);
							}
							else {
								JQueryProxy.jQuery(SelectedRow).draggable("destroy");
								EnableDroppable(false);
							}
						}
						
						jQuery elem = JQueryProxy.jQuery(GetElement());

						if (value) {
							elem.removeClass(DisabledDivClass);
							elem.attr("tabindex", tabIndex);
						}
						else {
							elem.addClass(DisabledDivClass);
							elem.removeAttr("tabindex");
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
						DOMElement elem = GetElement();
						elem.Children[0].Style.Display = (colHeadersVisible ? "" : "none");
						JQueryProxy.jQuery(elem.Children[1]).height(ContentHeight);
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
								MakeDraggable(JQueryProxy.jQuery(SelectedRow));
							EnableDroppable(true);
						}
						else {
							if (selectedRowIndex != -1)
								JQueryProxy.jQuery(SelectedRow).draggable("destroy");
							EnableDroppable(false);
						}
					}
				#endif
				enableDragDrop = value;
			}
		}

		public void AddItem(string[] cellTexts, object data) {
			InsertItem(numRows, cellTexts, data);
		}
		
		public void InsertItem(int index, string[] cellTexts, object data) {
			numRows++;
			if (selectedRowIndex >= index)
				selectedRowIndex++;

			rowData.Insert(index, data);

			#if CLIENT
				if (isAttached && !rebuilding) {
					StringBuilder sb = new StringBuilder();
					AddRowHtml(sb, cellTexts, null, (numRows % 2) == 1, false);
					jQuery q = JQueryProxy.jQuery(sb.ToString());
					if (index == numRows - 1) // remember we already incremented numRows
						q.appendTo(GetValuesTBody());
					else
						q.insertBefore(GetValuesTBody().Rows[index]);

					for (jQuery next = q.next(); next.size() > 0; next = next.next()) {
						if (next.isInExpression("." + EvenRowClass)) {
							next.removeClass(EvenRowClass);
							next.addClass(OddRowClass);
						}
						else {
							next.removeClass(OddRowClass);
							next.addClass(EvenRowClass);
						}
					}

					return;
				}
			#endif
			rowTextsIfNotRendered.Insert(index, cellTexts);
			rowClassesIfNotRendered.Insert(index, null);
		}
		
		public void SetRowClass(int index, string rowClass) {
			if (index < 0 || index >= numRows)
				return;
			
			#if CLIENT
				if (isAttached && !rebuilding) {
					TableRowElement tr = (TableRowElement)GetValuesTBody().Rows[index];
					ArrayList classes = (ArrayList)(object)tr.ClassName.Split(" ").Filter(delegate(object x) { string s = (string)x; return s == EvenRowClass || s == OddRowClass || s == SelectedRowClass; });
					classes.Add(rowClass);
					tr.ClassName = classes.Join(" ");
					return;
				}
			#endif
			rowClassesIfNotRendered[index] = rowClass;
		}

		public string GetRowClass(int index, string rowClass) {
			if (index < 0 || index >= numRows)
				return null;
			
			#if CLIENT
				if (isAttached && !rebuilding) {
					TableRowElement tr = (TableRowElement)GetValuesTBody().Rows[index];
					string[] classes = (string[])tr.ClassName.Split(" ").Filter(delegate(object x) { string s = (string)x; return s == EvenRowClass || s == OddRowClass || s == SelectedRowClass; });
					return classes.Length > 0 ? classes[0] : null;
				}
			#endif
			return (string)rowClassesIfNotRendered[index];
		}

		public void BeginRebuild() {
			#if CLIENT
				rebuilding = true;
				if (rowTextsIfNotRendered == null)
					rowTextsIfNotRendered = new StringArrayList();
				if (rowClassesIfNotRendered == null)
					rowClassesIfNotRendered = new StringList();
			#endif
			Clear();
		}
		
		public void EndRebuild() {
			#if CLIENT
				rebuilding = false;
				
				if (isAttached) {
					StringBuilder sb = new StringBuilder();
					for (int i = 0; i < rowTextsIfNotRendered.Length; i++) {
						AddRowHtml(sb, (string[])rowTextsIfNotRendered[i], (string)rowClassesIfNotRendered[i], (i % 2) == 0, i == selectedRowIndex);
					}
					JQueryProxy.jQuery(GetValuesTBody()).html(sb.ToString());
					
					if (selectedRowIndex >= 0) {
						EnsureVisible(selectedRowIndex);
						if (enableDragDrop && enabled)
							MakeDraggable(JQueryProxy.jQuery(SelectedRow));
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
					JQueryProxy.jQuery(GetValuesTBody()).empty();
					OnSelectionChanged(EventArgs.Empty);
				}
			#endif
			if (rowTextsIfNotRendered != null)
				rowTextsIfNotRendered.Clear();
			if (rowClassesIfNotRendered != null)
				rowClassesIfNotRendered.Clear();
			rowData.Clear();
		}
		
		public void UpdateItem(int row, string[] cellTexts, object data) {
			rowData[row] = data;
			#if CLIENT
				if (isAttached && !rebuilding) {
					TableRowElement tr = (TableRowElement)GetValuesTBody().Rows[row];
					for (int i = 0; i < tr.Cells.Length; i++) {
						string text = i < cellTexts.Length ? cellTexts[i] : "";
						tr.Cells[i].Children[0].Children[0].InnerHTML = !string.IsNullOrEmpty(text) ? Utils.HtmlEncode(text) : "&nbsp;";
					}
					return;
				}
			#endif
			rowTextsIfNotRendered[row] = cellTexts;
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
					jQuery q = JQueryProxy.jQuery(GetValuesTBody().Rows[row]).remove(), next = q.next();
					q.remove();
					for (; next.size() > 0; next = next.next()) {
						if (next.isInExpression("." + EvenRowClass)) {
							next.removeClass(EvenRowClass);
							next.addClass(OddRowClass);
						}
						else {
							next.removeClass(OddRowClass);
							next.addClass(EvenRowClass);
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

		public string[] GetTexts(int row) {
			if (row < 0 || row >= numRows)
				return null;
			#if CLIENT
				if (isAttached && !rebuilding) {
					TableRowElement tr = (TableRowElement)GetValuesTBody().Rows[row];
					string[] result = new string[tr.Cells.Length];
					for (int i = 0; i < result.Length; i++)
						result[i] = JQueryProxy.jQuery(tr.Cells[i]).text();
					return result;
				}
			#endif
			return (string[])rowTextsIfNotRendered[row];
		}
		
		public int NumRows {
			get {
				return numRows;
			}
		}
		
		private void AddRowHtml(StringBuilder sb, string[] cellTexts, string addClass, bool even, bool selected) {
			sb.Append("<tr class=\"" + (even ? EvenRowClass : OddRowClass) + (selected ? " " + SelectedRowClass : "") + (!string.IsNullOrEmpty(addClass) ? " " + addClass : "") + "\">");
			for (int c = 0; c < NumColumns; c++)
				sb.Append("<td " + (string.IsNullOrEmpty(colClasses[c]) ? "" : (" class=\"" + colClasses[c] + "\"")) + "><div style=\"width: " + Utils.ToStringInvariantInt(colWidths[c]) + "px\"><div>" + (c < cellTexts.Length && !string.IsNullOrEmpty(cellTexts[c]) ? Utils.HtmlEncode(cellTexts[c]) : "&nbsp;") + "</div></div></td>");
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
					for (int i = 0; i < Utils.ArrayLength(rowTextsIfNotRendered); i++) {
						AddRowHtml(sb, (string[])rowTextsIfNotRendered[i], (string)rowClassesIfNotRendered[i], (i % 2) == 0, i == selectedRowIndex);
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
			rowTextsIfNotRendered   = new StringArrayList();
			rowClassesIfNotRendered = new StringList();
			rowData = new ObjectList();
		}
		
#if SERVER
		public Grid() {
			GlobalServices.Provider.GetService<IScriptManagerService>().RegisterClientType(GetType());
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
		public extern Grid();
		public Grid(object config) {
			dragFeedbackHandler = new JQueryEventHandlerDelegate(ValuesDiv_DragFeedback);
			if (!Script.IsUndefined(config)) {
				InitConfig(Dictionary.GetDictionary(config));
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
		
		protected virtual void InitConfig(Dictionary config) {
			id = (string)config["id"];
			colTitles = (string[])config["colTitles"];
			colWidths = (int[])config["colWidths"];
			colClasses = (string[])config["colClasses"];
			width = (int)config["width"];
			height = (int)config["height"];
			tabIndex = (int)config["tabIndex"];
			numRows = (int)config["numRows"];
			enabled = (bool)config["enabled"];
			colHeadersVisible = (bool)config["colHeadersVisible"];
			enableDragDrop = (bool)config["enableDragDrop"];
			selectedRowIndex = (int)config["selectedRowIndex"];
			rowData = (ObjectList)config["rowData"];

			Attach();
		}
		
		public DOMElement GetElement() { return isAttached ? Document.GetElementById(id) : null; }
		
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
					jQuery row = JQueryProxy.jQuery(SelectedRow);
					row.removeClass(SelectedRowClass);
					if (enableDragDrop)
						row.draggable("destroy");
				}
				selectedRowIndex = value;
				if (selectedRowIndex != -1 && isAttached && !rebuilding) {
					EnsureVisible(selectedRowIndex);
					jQuery row = JQueryProxy.jQuery(SelectedRow);
					row.addClass(SelectedRowClass);
					if (enableDragDrop && enabled)
						MakeDraggable(row);
				}
				OnSelectionChanged(EventArgs.Empty);
			}
		}
		
		public void EnsureVisible(int rowIndex) {
			jQuery row = JQueryProxy.jQuery(GetValuesTBody().Rows[rowIndex]);
			jQuery valuesDiv = JQueryProxy.jQuery(GetElement().Children[1]);
			DOMElement d = valuesDiv.get(0);
			double offsetTop = row.offset().top - valuesDiv.offset().top, scrollTop = valuesDiv.scrollTop(), rowHeight = row.height(), tblHeight = d.ClientHeight;

			if (offsetTop < 0) {
				valuesDiv.scrollTop(Math.Round(scrollTop + offsetTop));
			}
			else if (offsetTop + rowHeight > tblHeight) {
				valuesDiv.scrollTop(Math.Round(scrollTop + offsetTop + rowHeight - tblHeight));
			}
		}
		
		private void ChangeDropTarget(DOMElement newTarget) {
			if (newTarget == currentDropTarget)
				return;
			if (currentDropTarget != null && currentDropTarget.TagName.ToLowerCase() == "tr")
				JQueryProxy.jQuery(currentDropTarget).removeClass(RowHoverClass);
			if (newTarget != null && newTarget.TagName.ToLowerCase() == "tr")
				JQueryProxy.jQuery(newTarget).addClass(RowHoverClass);
			currentDropTarget = newTarget;
		}
		
		private void ValuesDiv_DragFeedback(JQueryEvent evt) {
			DOMElement valuesDiv = GetElement().Children[1], newDropTarget = null;

			int valuesDivTop = (int)JQueryProxy.jQuery(valuesDiv).offset().top;
			int offset = evt.pageY - valuesDivTop + valuesDiv.ScrollTop;
			int rowIndex = Math.Truncate(offset / (float)rowHeight);	// Need to do this because Script# doesn't do integer division correctly.
			if (rowIndex > selectedRowIndex)
				rowIndex++;

			if (rowIndex >= 0 && rowIndex < numRows)
				newDropTarget = ((TableElement)valuesDiv.Children[0]).Rows[rowIndex];
			else
				newDropTarget = valuesDiv;

			ChangeDropTarget(newDropTarget);
		}
		
		private void ValuesDiv_Drop(JQueryEvent evt, DroppableEventObject ui) {
			if (currentDropTarget == null) {
				DragEnded();
				return;
			}
			
			int draggingIndex = selectedRowIndex;

			DOMElement valuesDiv = GetElement().Children[1];
			DOMElement draggedElem = ui.draggable.get(0);
			DOMElement valuesTbodyEl = GetValuesTBody();

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
			JQueryProxy.jQuery(Window.Document).unbind("mousemove", dragFeedbackHandler);
		}
		
		private void MakeDraggable(jQuery row) {
			row.draggable(new Dictionary("helper", "clone",
				                         "appendTo", row.parent(),
				                         "scroll", true,
			                             "containment", "parent",
			                             "start", Utils.Wrap(new UnwrappedDraggableEventHandlerDelegate(delegate(DOMElement d, JQueryEvent evt, DraggableEventObject ui) { JQueryProxy.jQuery(d).addClass(CurrentDraggingRowClass); })),
			                             "stop", Utils.Wrap(new UnwrappedDraggableEventHandlerDelegate(delegate(DOMElement d, JQueryEvent evt, DraggableEventObject ui) { JQueryProxy.jQuery(d).removeClass(CurrentDraggingRowClass); }))
			                        ));
		}
		
		private TableRowElement GetHeaderRow() {
			return (TableRowElement)((TableElement)GetElement().Children[0].Children[0]).Rows[0];
		}

		private TableSectionElement GetValuesTBody() {
			return (TableSectionElement)((TableElement)GetElement().Children[1].Children[0]).tBodies[0];
		}
		
		private TableRowElement[] GetAllRows() {
			return (TableRowElement[])(object)((TableSectionElement)((TableElement)GetElement().Children[1].Children[0]).tBodies[0]).Rows;
		}

		private void EnableDroppable(bool enable) {
			jQuery el = JQueryProxy.jQuery(GetElement().Children[1]);
			if (enable) {
				el.droppable(new Dictionary("tolerance", "pointer",
				                            "greedy",    true,
				                            "over",      (Callback)delegate() {
				                                             TableRowElement[] rows = GetAllRows();
				                                             rowHeight = (rows.Length > 1 ? Math.Max(rows[0].OffsetHeight, rows[1].OffsetHeight) : (Number)1);	// We need to take the maximum of two rows because one of them might be the currently dragged one, which is hidden.
				                                             currentDropTarget = null;
				                                             JQueryProxy.jQuery(Window.Document).mousemove(dragFeedbackHandler);
				                                         },
				                            "out",       (Callback)DragEnded, 
				                            "drop",      new DroppableEventHandlerDelegate(ValuesDiv_Drop)));
			}
			else {
				el.droppable("destroy");
			}
		}

		private void MakeHeadersResizable() {
			jQuery headerTr = JQueryProxy.jQuery(((TableElement)GetElement().Children[0].Children[0]).Rows[0]);
			headerTr.children(":not(:last-child)").children().resizable(new Dictionary("handles", "e",
			                                                                           "stop", Utils.Wrap(new UnwrappedResizableEventHandlerDelegate(
			                                                                                   delegate(DOMElement d, JQueryEvent evt, ResizableEventObject ui) {
			                                                                                       int index = headerTr.children().index(d.ParentNode);
			                                                                                       SetColWidth(index, Math.Round(ui.size.width));
			                                                                                   }))
			                                                           ));
			if (jQuery.browser.msie && Utils.ParseDouble(jQuery.browser.version) < 8)
				headerTr.find(".ui-resizable-e").height(HeaderHeight);
		}
		
		private void AttachInner(DOMElement element) {
			if (enableDragDrop && enabled) {
				if (selectedRowIndex >= 0)
					MakeDraggable(JQueryProxy.jQuery(SelectedRow));
				EnableDroppable(true);
			}

			if (colHeadersVisible) {
				jQuery headerTr = JQueryProxy.jQuery(((TableElement)GetElement().Children[0].Children[0]).Rows[0]);
				headerTr.one("mouseover", null, (JQueryEventHandlerDelegate)delegate(JQueryEvent evt) {
					// Delaying this improves load time by perhaps 50 or so ms (which can make a difference).
					if (!headersAreMadeResizable)
						MakeHeadersResizable();
					headersAreMadeResizable = true;
				});
			}

			JQueryProxy.jQuery(element.Children[1].Children[0]).click(ValueTable_Click);

			JQueryProxy.jQuery(element.Children[1]).scroll(delegate {
				DOMElement elem = GetElement();
				JQueryProxy.jQuery(elem.Children[0]).scrollLeft(Math.Round(JQueryProxy.jQuery(elem.Children[1]).scrollLeft()));
			});
		}
		
		private void ValueTable_Click(JQueryEvent evt) {
			if (!enabled)
				return;
			if (evt.target == GetElement().Children[1].Children[0])	// Sometimes it is possible for the user to click on the table, as opposed to a table row.
				return;

			jQuery cell = Utils.Parent(JQueryProxy.jQuery(evt.target), "td"),
			       row  = Utils.Parent(cell, "tr");

			int rowIndex = ((TableRowElement)row.get(0)).RowIndex;

			GridCellClickedEventArgs ea = new GridCellClickedEventArgs();
			ea.Row = rowIndex;
			ea.PreventRowSelect = false;
			ea.Col = (int)Type.GetField(cell.get(0), "cellIndex"); // missing property from Script#

			OnCellClicked(ea);
			if (!ea.PreventRowSelect)
				SelectedRowIndex = rowIndex;
		}
		
		public void Attach() {
			if (Utils.IsNull(id) || isAttached)
				throw new Exception("Must set ID and can only attach once");
			isAttached = true;
			DOMElement elem = GetElement();

			AttachInner(elem);

			UIUtils.AttachKeyPressHandler(elem, el_KeyDown);
		}

		private void el_KeyDown(JQueryEvent e) {
			if (!enabled)
				return;

			GridKeyPressEventArgs ev = new GridKeyPressEventArgs();
			ev.KeyCode = e.keyCode;
			OnKeyPress(ev);
			if (ev.PreventDefault) {
				e.preventDefault();
				return;
			}

			switch (e.keyCode) {
				case 38:
					// key up
					if (NumRows > 0 && selectedRowIndex > 0)
						SelectedRowIndex = (selectedRowIndex == -1 ? 0 : SelectedRowIndex - 1);
					e.preventDefault();
					break;

				case 40:
					// key down
					if (NumRows > 0 && selectedRowIndex < NumRows - 1)
						SelectedRowIndex = (SelectedRowIndex == -1 ? 0 : SelectedRowIndex + 1);
					e.preventDefault();
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
			if (!Utils.IsNull(SelectionChanging))
				SelectionChanging(this, e);
		}
		
		protected virtual void OnSelectionChanged(EventArgs e) {
			if (!Utils.IsNull(SelectionChanged))
				SelectionChanged(this, e);
		}

		protected virtual void OnCellClicked(GridCellClickedEventArgs e) {
			if (!Utils.IsNull(CellClicked))
				CellClicked(this, e);
		}
		
		protected virtual void OnKeyPress(GridKeyPressEventArgs e) {
			if (!Utils.IsNull(KeyPress))
				KeyPress(this, e);
		}
		
		protected virtual void OnDragDropCompleting(GridDragDropCompletingEventArgs e) {
			if (!Utils.IsNull(DragDropCompleting))
				DragDropCompleting(this, e);
		}

		protected virtual void OnDragDropCompleted(GridDragDropCompletedEventArgs e) {
			if (!Utils.IsNull(DragDropCompleted))
				DragDropCompleted(this, e);
		}

		public void Focus() {
			if (isAttached)
				GetElement().Focus();
		}
#endif
	}
}
