#if SERVER
using RowDataList = System.Collections.Generic.List<Saltarelle.UI.GridRowData>;
using System.Text;
using System;
using System.Collections.Generic;
#elif CLIENT
using RowDataList = System.ArrayList;
using System;
using System.DHTML;
#endif

namespace Saltarelle.UI {
	[Record]
	internal sealed class GridRowData {
		public string[] cellTexts;
		public object data;
		public GridRowData(string[] cellTexts, object data) {
			this.cellTexts = cellTexts;
			this.data = data;
		}
	}

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
		public const string RowHoverClass = "DropHover";
		public const string CurrentDraggingRowClass = "CurrentDraggingRow";
	
		private string id;
		private int[] colWidths = new int[0];
		private string[] colClasses = new string[0];
		private string[] colTitles = new string[0];
		private RowDataList rowsIfNotRendered = new RowDataList();
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
			private jQuery gridElement;
			private bool rebuilding;
			private int headerHeight;
			
			private jQuery headerTr;
			private jQuery valuesTbody;
		#endif

		public string Id {
			get { return id; }
			set {
				id = value;
				#if CLIENT
					if (isAttached)
						GetElement().ID = value;
				#endif
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
						gridElement.children("div").andSelf().width(value - 2 * BorderSize);
					}
				#endif
				width = value;
			}
		}
		public int MinWidth { get { return 10; } }
		public int MaxWidth { get { return 1000; } }

		public int Height {
			get {
				return height;
			}
			set {
				#if CLIENT
					if (isAttached)
						gridElement.children("div:eq(1)").height(value - 2 * BorderSize - (colHeadersVisible ? headerHeight : 0));
				#endif
				height = value;
			}
		}
		public int MinHeight { get { return 10; } }
		public int MaxHeight { get { return 1000; } }

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
						gridElement.html(InnerHtml);
						AttachInner();
					}
				#endif
			}
		}

		public void SetColTitle(int col, string title) {
			#if CLIENT
				if (isAttached)
					headerTr.children("th:eq(" + col.ToString() + ")").children("div").children("div:eq(0)").text(title);
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
					gridElement.children("div").children("table").children("thead,tbody").children("tr").children("th,td").filter(":nth-child(" + (col + 1).ToString() + ")").children("div").width(width);
					gridElement.children("div:eq(1)").scroll();
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
			#if CLIENT
				if (isAttached) {
					jQuery cells = valuesTbody.children("tr").children("td:nth-child(" + (col + 1).ToString() + ")");
					if (!string.IsNullOrEmpty(colClasses[col]))
						cells.removeClass(colClasses[col]);
					if (!string.IsNullOrEmpty(cls))
						cells.addClass(cls);
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
								MakeDraggable(SelectedRow);
								EnableDroppableRows(valuesTbody.children());
								EnableDroppableValueDiv();
							}
							else {
								SelectedRow.draggable("destroy");
								valuesTbody.children().droppable("destroy");
								gridElement.children("div:eq(1)").droppable("destroy");
							}
						}

						if (value) {
							gridElement.removeClass(DisabledDivClass);
							gridElement.attr("tabindex", tabIndex);
						}
						else {
							gridElement.addClass(DisabledDivClass);
							gridElement.removeAttr("tabindex");
						}
					}
				#endif						
				enabled = value;
			}
		}
		
		public bool ColHeadersVisible {
			get { return colHeadersVisible; }
			set {
				colHeadersVisible = value;
				#if CLIENT
					if (isAttached) {
						gridElement.children("div:eq(0)").css("display", colHeadersVisible ? "" : "none");
						gridElement.children("div:eq(1)").height(height - 2 * BorderSize - (colHeadersVisible ? headerHeight : 0));
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
						if (selectedRowIndex != -1) {
							if (value && enabled) {
								MakeDraggable(SelectedRow);
								EnableDroppableRows(valuesTbody.children());
								EnableDroppableValueDiv();
							}
							else {
								SelectedRow.draggable("destroy");
								valuesTbody.children().droppable("destroy");
								gridElement.children("div:eq(1)").droppable("destroy");
							}
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
			#if CLIENT
				if (isAttached && !rebuilding) {
					StringBuilder sb = new StringBuilder();
					AddRowHtml(sb, cellTexts, (numRows % 2) == 1, false, null);
					jQuery q = JQueryProxy.jQuery(sb.ToString());
					Type.SetField(q.get(0), "__data", data);
					q.click(rowClickHandler);
					if (enabled && enableDragDrop)
						EnableDroppableRows(q);
					if (index == numRows - 1) // remember we already incremented numRows
						q.appendTo(valuesTbody);
					else
						q.insertBefore(valuesTbody.children().eq(index));

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
			rowsIfNotRendered.Insert(index, new GridRowData(cellTexts, data));
		}
		

		public void BeginRebuild() {
			#if CLIENT
				rebuilding = true;
			#endif
			Clear();
		}
		
		public void EndRebuild() {
			#if CLIENT
				rebuilding = false;
				
				if (isAttached) {
					int index = 0;
					StringBuilder sb = new StringBuilder();
					foreach (GridRowData r in rowsIfNotRendered) {
						AddRowHtml(sb, r.cellTexts, (index % 2) == 0, index == selectedRowIndex, r.data);
						index++;
					}
					valuesTbody.html(sb.ToString());

					AttachToValuesTbody();
				}
			#endif
		}
		
		public void Clear() {
			numRows = 0;
			selectedRowIndex = -1;
			#if CLIENT
				if (isAttached) {
					valuesTbody.empty();
					OnSelectionChanged(EventArgs.Empty);
				}
			#endif
			rowsIfNotRendered.Clear();
		}
		
		public void UpdateItem(int row, string[] cellTexts, object data) {
			#if CLIENT
				if (isAttached && !rebuilding) {
					jQuery q = valuesTbody.children(":eq(" + row.ToString() + ")");
					Type.SetField(q.get(0), "__data", data);
					q.children("td").each(delegate(int col, DOMElement e) {
						JQueryProxy.jQuery(e).children("div").children("div").text(col < cellTexts.Length ? cellTexts[col] : "");
						return true;
					});
				}
			#endif
			rowsIfNotRendered[row] = new GridRowData(cellTexts, data);
		}
		
		public void DeleteItem(int row) {
			numRows--;
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
					jQuery q = valuesTbody.children(":eq(" + row.ToString() + ")"), next = q.next();
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
			rowsIfNotRendered.RemoveAt(row);
		}
		
		public object GetData(int row) {
			if (row < 0 || row >= numRows)
				return null;
			#if CLIENT
				if (isAttached && !rebuilding)
					return Type.GetField(((TableSectionElement)valuesTbody.get(0)).Rows[row], "__data");
			#endif
			return ((GridRowData)rowsIfNotRendered[row]).data;
		}

		public string[] GetTexts(int row) {
			if (row < 0 || row >= numRows)
				return null;
			#if CLIENT
				if (isAttached && !rebuilding) {
					jQuery jq = JQueryProxy.jQuery(((TableSectionElement)valuesTbody.get(0)).Rows[row]);
					string[] result = new string[jq.children().size()];
					for (int i = 0; i < result.Length; i++)
						result[i] = jq.children().eq(i).text();
					return result;
				}
			#endif
			return ((GridRowData)rowsIfNotRendered[row]).cellTexts;
		}
		
		public int NumRows {
			get {
				return numRows;
			}
		}
		
		private void AddRowHtml(StringBuilder sb, string[] cellTexts, bool even, bool selected, object data) {
			sb.Append("<tr" + (!Utils.IsNull(data) ? (" __data=\"" + Utils.HtmlEncode(Utils.Json(data)) + "\"") : "") + " class=\"" + (even ? EvenRowClass : OddRowClass) + (selected ? " ui-state-highlight" : "") + "\">");
			for (int c = 0; c < NumColumns; c++)
				sb.Append("<td " + (string.IsNullOrEmpty(colClasses[c]) ? "" : (" class=\"" + colClasses[c] + "\"")) + "><div style=\"width: " + Utils.ToStringInvariantInt(colWidths[c]) + "px\"><div>" + (c < cellTexts.Length && !string.IsNullOrEmpty(cellTexts[c]) ? Utils.HtmlEncode(cellTexts[c]) : "&nbsp;") + "</div></div></td>");
			sb.Append("</tr>");
		}
		
		private string InnerHtml {
			get {
				StringBuilder sb = new StringBuilder();
				// position: relative on the header div solves an IE6 rendering bug.
				sb.Append("<div class=\"" + HeaderDivClass + "\" style=\"position: relative; width: " + (this.width - 2 * BorderSize) + "px\"><table cellpadding=\"0\" cellspacing=\"0\" class=\"" + HeaderTableClass + "\"><thead><tr>");
				for (int c = 0; c < NumColumns; c++)
					sb.Append("<th " + (string.IsNullOrEmpty(colClasses[c]) ? "" : (" class=\"" + colClasses[c] + "\"")) + "><div style=\"width: " + Utils.ToStringInvariantInt(colWidths[c]) + "px\"><div>" + (!string.IsNullOrEmpty(colTitles[c]) ? Utils.HtmlEncode(colTitles[c]) : "&nbsp;") + "</div></div></th>");
				sb.Append("<th class=\"" + SpacerThClass + "\"><div>&nbsp;</div></th></tr></thead></table></div><div class=\"" + ValuesDivClass + "\" style=\"width: " + (this.width - 2 * BorderSize) + "px\"><table cellpadding=\"0\" cellspacing=\"0\" class=\"" + ValuesTableClass + "\"><tbody>");
				int index = 0;
				foreach (GridRowData r in rowsIfNotRendered) {
					AddRowHtml(sb, r.cellTexts, (index % 2) == 0, index == selectedRowIndex, r.data);
					index++;
				}
				sb.Append("</tbody></table></div>");
				return sb.ToString();
			}
		}
		
		protected virtual void BeforeGetHtml() {
		}

		public string Html {
			get {
				if (string.IsNullOrEmpty(id))
					throw new Exception("Must set ID before render");
				BeforeGetHtml();
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

			Attach();
		}
		
		private JQueryEventHandlerDelegate rowClickHandler;

		public DOMElement GetElement() { return isAttached ? Document.GetElementById(id) : null; }
		
		private jQuery SelectedRow { get { return valuesTbody.children().eq(selectedRowIndex); } }

		public int SelectedRowIndex {
			get {
				return selectedRowIndex;
			}
			set {
				if (selectedRowIndex == value)
					return;
				
				if (!RaiseSelectionChanging(value))
					return;
					
				if (selectedRowIndex != -1) {
					jQuery row = SelectedRow;
					row.removeClass("ui-state-highlight");
					if (enableDragDrop)
						row.draggable("destroy");
				}
				selectedRowIndex = value;
				if (selectedRowIndex != -1) {
					EnsureVisible(selectedRowIndex);
					jQuery row = SelectedRow;
					row.addClass("ui-state-highlight");
					if (enableDragDrop && enabled)
						MakeDraggable(row);
				}
				OnSelectionChanged(EventArgs.Empty);
			}
		}
		
		public void EnsureVisible(int rowIndex) {
			jQuery row = valuesTbody.children("tr:eq(" + Utils.ToStringInvariantInt(rowIndex) + ")");
			jQuery valuesDiv = gridElement.children("div:eq(1)");
			DOMElement d = valuesDiv.get(0);
			double offsetTop = row.offset().top - valuesDiv.offset().top, scrollTop = valuesDiv.scrollTop(), rowHeight = row.height(), tblHeight = d.ClientHeight;

			if (offsetTop < 0) {
				valuesDiv.scrollTop(Math.Round(scrollTop + offsetTop));
			}
			else if (offsetTop + rowHeight > tblHeight) {
				valuesDiv.scrollTop(Math.Round(scrollTop + offsetTop + rowHeight - tblHeight));
			}
		}
		
		private void MakeDraggable(jQuery row) {
			row.draggable(new Dictionary("helper", "clone",
				                         "appendTo", gridElement.children("div:eq(1)").children("table").children("tbody"),
				                         "scroll", false,
			                             "containment", "parent",
			                             "start", Utils.Wrap(new UnwrappedDraggableEventHandlerDelegate(delegate(DOMElement d, JQueryEvent evt, DraggableEventObject ui) { JQueryProxy.jQuery(d).addClass(CurrentDraggingRowClass); })),
			                             "stop", Utils.Wrap(new UnwrappedDraggableEventHandlerDelegate(delegate(DOMElement d, JQueryEvent evt, DraggableEventObject ui) { JQueryProxy.jQuery(d).removeClass(CurrentDraggingRowClass); }))
			                        ));
		}
		
		private void AttachToValuesTbody() {
			valuesTbody.children().each(new EachCallback(delegate(int i, DOMElement e) {
				string data = (string)e.GetAttribute("__data");
				Type.SetField(e, "__data", string.IsNullOrEmpty(data) ? null : jQuery.evalJSON(data));
				return true;
			}));
			valuesTbody.children().click(rowClickHandler);
			if (selectedRowIndex >= 0 && enableDragDrop && enabled)
				MakeDraggable(SelectedRow);

			if (enableDragDrop && enabled)
				EnableDroppableRows(valuesTbody.children());
		}
		
		private void EnableDroppableRows(jQuery rows) {
			rows.droppable(new Dictionary("tolerance", "pointer",
			                              "drop", Utils.Wrap(new UnwrappedDroppableEventHandlerDelegate(Row_Drop)),
			                              "greedy", true,
			                              "hoverClass", RowHoverClass
			              ));
		}

		private void EnableDroppableValueDiv() {
			gridElement.children("div:eq(1)").droppable(new Dictionary("tolerance", "pointer", "greedy", true, "drop", new DroppableEventHandlerDelegate(ValuesDiv_Drop)));
		}

		private void Row_Drop(DOMElement targetElem, JQueryEvent evt, DroppableEventObject ui) {
			int newIndex = ((TableRowElement)targetElem).RowIndex;
			newIndex = (newIndex > selectedRowIndex ? newIndex - 1 : newIndex); // If dragging down we have to pretend that the original row does not exist.
			if (newIndex == selectedRowIndex)
				return;

			GridDragDropCompletingEventArgs e = new GridDragDropCompletingEventArgs(selectedRowIndex, newIndex);
			OnDragDropCompleting(e);
			if (e.Cancel)
				return;

			DOMElement draggedElem = ui.draggable.get(0);
			DOMElement valuesTbodyEl = valuesTbody.get(0);
			valuesTbodyEl.RemoveChild(draggedElem);
			valuesTbodyEl.InsertBefore(draggedElem, targetElem);
			selectedRowIndex = newIndex;
			gridElement.focus();

			OnDragDropCompleted(new GridDragDropCompletedEventArgs(selectedRowIndex, newIndex));
		}

		private void ValuesDiv_Drop(JQueryEvent evt, DroppableEventObject ui) {
			if (selectedRowIndex == NumRows - 1)
				return;
			GridDragDropCompletingEventArgs e = new GridDragDropCompletingEventArgs(selectedRowIndex, NumRows - 1);
			OnDragDropCompleting(e);
			if (e.Cancel)
				return;

			DOMElement draggedElem = ui.draggable.get(0);
			DOMElement valuesTbodyEl = valuesTbody.get(0);
			valuesTbodyEl.RemoveChild(draggedElem);
			valuesTbodyEl.AppendChild(draggedElem);
			selectedRowIndex = NumRows - 1;
			gridElement.focus();

			OnDragDropCompleted(new GridDragDropCompletedEventArgs(selectedRowIndex, NumRows - 1));
		}
		
		private void AttachInner() {
			headerTr    = gridElement.children("div:eq(0)").children("table").children("thead").children("tr:first-child");
			valuesTbody = gridElement.children("div:eq(1)").children("table").children("tbody");

			AttachToValuesTbody();
			
			headerTr.children(":not(:last-child)").children().resizable(new Dictionary("handles", "e",
			                                                                           "stop", Utils.Wrap(new UnwrappedResizableEventHandlerDelegate(
			                                                                                   delegate(DOMElement d, JQueryEvent evt, ResizableEventObject ui) {
			                                                                                       int index = headerTr.children().index(d.ParentNode);
			                                                                                       SetColWidth(index, Math.Round(ui.size.width));
			                                                                                   }))
			                                                           ));
			if (jQuery.browser.msie && Utils.ParseDouble(jQuery.browser.version) < 8)
				headerTr.find(".ui-resizable-e").height(headerHeight);

			jQuery valuesDiv = gridElement.children("div:eq(1)");
			valuesDiv.height(height - 2 * BorderSize - (colHeadersVisible ? headerHeight : 0));
			valuesDiv.scroll(delegate {
				gridElement.children("div:eq(0)").scrollLeft(Math.Round(gridElement.children("div:eq(1)").scrollLeft()));
			});
		}
		
		public void Attach() {
			if (Utils.IsNull(id) || isAttached)
				throw new Exception("Must set ID and can only attach once");
			isAttached = true;
			gridElement = JQueryProxy.jQuery(GetElement());
		
			rowClickHandler = (JQueryEventHandlerDelegate)Utils.Wrap(new UnwrappedJQueryEventHandlerDelegate(delegate(DOMElement e, JQueryEvent evt) {
				if (!enabled)
					return;
			
				int rowIndex = ((TableRowElement)e).RowIndex;

				GridCellClickedEventArgs ea = new GridCellClickedEventArgs();
				ea.Row = rowIndex;
				ea.PreventRowSelect = false;
				
				// find the cell which was clicked
				for (DOMElement current = evt.target; current != e; current = current.ParentNode) {
					if (current.TagName.ToLowerCase() == "td") {
						ea.Col = (int)Type.GetField(current, "cellIndex"); // missing property from Script#
						OnCellClicked(ea);
						break;
					}
				}
				
				if (!ea.PreventRowSelect)
					SelectedRowIndex = rowIndex;
			}));

			headerHeight = Math.Round(gridElement.children("div:eq(0)").outerHeight());

			AttachInner();

			if (enableDragDrop && enabled)
				EnableDroppableValueDiv();

			UIUtils.AttachKeyPressHandler(gridElement.get(0), el_KeyDown);
            
            if (!colHeadersVisible)
				gridElement.children("div:eq(0)").css("display", "none");
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
					if (NumRows > 0 && selectedRowIndex  > 0)
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
