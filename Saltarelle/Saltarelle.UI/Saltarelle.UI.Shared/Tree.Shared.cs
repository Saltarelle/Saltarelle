using System;
#if SERVER
using System.Text;
using System.Collections;
using System.Linq;
#endif
#if CLIENT
using System.DHTML;
#endif

namespace Saltarelle.UI {
	[Record]
	internal sealed class TreeNode {
		internal int  id;
		internal bool expanded;
		internal bool isChecked;
		internal string text;
		internal object data;
		
		internal TreeNode(int id, string text, object data) {
			this.id = id;
			this.text = text;
			this.data = data;
			this.children = new ArrayList();
			this.expanded = false;
			this.isChecked = false;
		}
		
		/// <summary>
		/// Only relevant until the tree has been rendered
		/// </summary>
		internal ArrayList children;
	}
	
	public delegate bool TreeSearchDelegate(object value);
	
#if CLIENT
	public class TreeSelectionChangingEventArgs : EventArgs {
		public bool Cancel;
	}
	public delegate void TreeSelectionChangingEventHandler(object sender, TreeSelectionChangingEventArgs e);
	
	public class TreeNodeEventArgs : EventArgs {
		public int NodeId;
		public TreeNodeEventArgs(int nodeId) {
			this.NodeId = nodeId;
		}
	}
	public delegate void TreeNodeEventHandler(object sender, TreeNodeEventArgs e);

	public class TreeKeyPressEventArgs : EventArgs {
		public int KeyCode;
		public bool PreventDefault;
	}
	public delegate void TreeKeyPressEventHandlerDelegate(object sender, TreeKeyPressEventArgs e);

	public class TreeDragDropCompletingEventArgs : CancelEventArgs {
		public int DraggedNodeId;
		public int NewParentId;
		public int PositionWithinNewParent;
		public TreeDragDropCompletingEventArgs(int draggedNodeId, int newParentId, int positionWithinNewParent) {
			this.DraggedNodeId           = draggedNodeId;
			this.NewParentId             = newParentId;
			this.PositionWithinNewParent = positionWithinNewParent;
			this.Cancel                  = false;
		}
	}
	public delegate void TreeDragDropCompletingEventHandler(object sender, TreeDragDropCompletingEventArgs e);

	public class TreeDragDropCompletedEventArgs : EventArgs {
		public int DraggedNodeId;
		public int NewParentId;
		public int PositionWithinNewParent;
		public TreeDragDropCompletedEventArgs(int draggedNodeId, int newParentId, int positionWithinNewParent) {
			this.DraggedNodeId           = draggedNodeId;
			this.NewParentId             = newParentId;
			this.PositionWithinNewParent = positionWithinNewParent;
		}
	}
	public delegate void TreeDragDropCompletedEventHandler(object sender, TreeDragDropCompletedEventArgs e);
#endif
	
	public class Tree : IControl, IClientCreateControl, IResizableX, IResizableY {
		public const string ClassName = "Tree";
		
		public const string NestedListClass      = "tree-nested-list";
		public const string ItemTextClass        = "tree-node-text";
		public const string NodeClass            = "tree-node";
		public const string IconClass            = "tree-node-icon";
		public const string ExpandCollapseClass  = "expand-collapse-icon";
		public const string LeafSuffix           = "-leaf";
		public const string ExpandedSuffix       = "-expanded";
		public const string CollapsedSuffix      = "-collapsed";
		public const string SpacerClass          = "tree-spacer";
		public const string NodeDropHoverClass   = "ui-state-highlight";
		public const string SpacerDropHoverClass = "tree-spacer-drophover";
		public const string SelectedNodeClass    = "ui-state-highlight";
		public const int    HorzBorderSize = 2;
		public const int    VertBorderSize = 2;
		
		/// <summary>
		/// Set to null after render.
		/// </summary>
		private TreeNode invisibleRoot;
		private string   id;
		private int      width;
		private int      height;
		private Position position;
		private int      nextNodeId;
		private bool     enableDragDrop;
		private bool     hasChecks;
		private string   blankImageUrl;
		#if CLIENT
			private bool isAttached;
			private jQuery treeElement;
			private bool rebuilding;
			
			private JQueryEventHandlerDelegate checkboxClickHandler;
		#endif
		
		#region Tree manipulation API
		
		public void BeginRebuild() {
			#if CLIENT
				if (isAttached) {
					treeElement.empty();
					selectedJQ = null;
					invisibleRoot = new TreeNode(0, null, null);
					rebuilding = true;
					nodeHash = new Dictionary();
					return;
				}
			#endif
			invisibleRoot.children.Clear();
		}

		public void EndRebuild() {
			#if CLIENT
				rebuilding = false;
				if (isAttached) {
					treeElement.html(InnerHtml);
					BringToLife(treeElement, true);
				}
			#endif
		}

		public int AddNode(int parentId, string text, object data, bool expandParent) {
			int nodeId = nextNodeId++;
			#if CLIENT
				if (isAttached && !rebuilding) {
					jQuery list;
					if (parentId == 0) {
						list = treeElement;
					}
					else {
						jQuery parentJQ = FindNodeJQ(parentId);
						list = parentJQ.children("." + NestedListClass);
						if (list.size() == 0) {
							list = JQueryProxy.jQuery("<div class=\"" + NestedListClass + "\"></div>").appendTo(parentJQ);
							parentJQ.removeClass(NodeClass + LeafSuffix);
							parentJQ.addClass(NodeClass + (expandParent ? ExpandedSuffix : CollapsedSuffix));
							ExpandNodeJQ(parentJQ, expandParent);
						}
						else {
							if (expandParent)
								ExpandNodeJQ(parentJQ, true);
						}
					}
					JQueryProxy.jQuery(SpacerHtml).appendTo(list);
					jQuery newNode = JQueryProxy.jQuery(GetNodeHtml(new TreeNode(nodeId, text, data))).appendTo(list);
					BringToLife(newNode, false);
					SetSelectedJQ(newNode);

					return nodeId;
				}
			#endif
			TreeNode parent = FindNode(parentId, invisibleRoot);
			if (Utils.IsNull(parent.children))
				parent.children = new ArrayList();
			parent.children.Add(new TreeNode(nodeId, text, data));
			parent.expanded = parent.expanded | expandParent;
			return nodeId;
		}
		
		public void RemoveNode(int id) {
			#if CLIENT
				if (isAttached && !rebuilding) {
					if (id == SelectedId) {
						jQuery newSel = Utils.Next(selectedJQ, "." + NodeClass);
						if (newSel.size() == 0) {
							newSel = Utils.Prev(selectedJQ, "." + NodeClass);
							if (newSel.size() == 0)
								newSel = Utils.Parent(selectedJQ, "." + NodeClass);
						}
						SetSelectedJQ(newSel);
					}
					jQuery q = FindNodeJQ(id);
					if (q.size() > 0) {
						jQuery parentJQ = Utils.Parent(q, "." + NodeClass + ",." + ClassName);
						q.prev().remove();
						q.remove();
						if (parentJQ.isInExpression("." + NodeClass))
							ChildRemoved(parentJQ);
					}
					nodeHash.Remove(id.ToString());
					return;
				}
			#endif

			TreeNode parent = FindParent(id, invisibleRoot);
			for (int i = 0; i < Utils.ArrayLength(parent.children); i++) {
				if (((TreeNode)parent.children[i]).id == id) {
					parent.children.RemoveAt(i);
					break;
				}
			}
		}
		
		public int[] GetChildNodeIds(int parentId) {
			#if CLIENT
				ArrayList l = new ArrayList();
				if (isAttached && !rebuilding) {
					(parentId == 0 ? treeElement : FindNodeJQ(parentId).children("." + NestedListClass)).children("." + NodeClass)
					     .each(delegate(int index, DOMElement elem) {
					         l.Add(Utils.ParseInt((string)elem.GetAttribute("__nodeid")));
						     return true;
						 });
				}
				else {
					ArrayList children = FindNode(parentId, invisibleRoot).children;
					if (!Utils.IsNull(children)) {
						foreach (TreeNode tn in children)
							l.Add(tn.id);
					}
				}
				return (int[])(object)l;
			#endif
			#if SERVER
				return FindNode(parentId, invisibleRoot).children.Cast<TreeNode>().Select(n => n.id).ToArray();
			#endif
		}
		
		public int[] RootNodeIds {
			get { return GetChildNodeIds(0); }
		}
		
		public string GetNodeText(int id) {
			#if CLIENT
				if (isAttached && !rebuilding)
					return FindNodeJQ(id).children("." + ItemTextClass).text();
			#endif
			return FindNode(id, invisibleRoot).text;
		}

		public void SetNodeText(int id, string text) {
			#if CLIENT
				if (isAttached && !rebuilding) {
					FindNodeJQ(id).children("." + ItemTextClass).text(text);
					return;
				}
			#endif
			FindNode(id, invisibleRoot).text = text;
		}
		
		public object GetNodeData(int id) {
			#if CLIENT
				if (isAttached && !rebuilding)
					return Type.GetField(FindNodeJQ(id).get(0), "__data");
			#endif
			return FindNode(id, invisibleRoot).data;
		}

		public void SetNodeData(int id, object data) {
			#if CLIENT
				if (isAttached && !rebuilding) {
					Type.SetField(FindNodeJQ(id).get(0), "__data", data);
					return;
				}
			#endif
			FindNode(id, invisibleRoot).data = data;
		}

		public bool IsNodeChecked(int id) {
			#if CLIENT
				if (isAttached && !rebuilding)
					return FindNodeJQ(id).find("input:checkbox").isInExpression(":checked");
			#endif
			return FindNode(id, invisibleRoot).isChecked;
		}

		public void ExpandNode(int id, bool expanded) {
			#if CLIENT
				if (isAttached && !rebuilding) {
					ExpandNodeJQ(FindNodeJQ(id), expanded);
					return;
				}
			#endif
			FindNode(id, invisibleRoot).expanded = expanded;
		}
		
		public void CheckNode(int id, bool isChecked) {
			if (!hasChecks)
				throw new Exception("The tree does not have any checkboxes");
			#if CLIENT
				if (isAttached && !rebuilding) {
					FindNodeJQ(id).find("input:checkbox").each(delegate(int _, DOMElement e) {
						((CheckBoxElement)e).Checked = isChecked;
						return true;
					});
				}
				else
					FindNode(id, invisibleRoot).isChecked = isChecked;
				OnNodeChecked(new TreeNodeEventArgs(id));
			#else
				FindNode(id, invisibleRoot).isChecked = isChecked;
			#endif
		}
		
		public int GetParentNodeId(int nodeId) {
			#if CLIENT
				if (isAttached && !rebuilding) {
					jQuery q = FindNodeJQ(nodeId);
					if (q.size() > 0) {
						jQuery parentJQ = Utils.Parent(q, "." + NodeClass + ",." + ClassName);
						if (parentJQ.isInExpression("." + NodeClass))
							return Utils.ParseInt((string)parentJQ.attr("__nodeid"));
					}
					return 0;
				}
			#endif

			return FindParent(nodeId, invisibleRoot).id;
		}

		/// <summary>
		/// Collapse a node and all its children
		/// </summary>
		/// <param name="id">Node to collapse, or 0 for entire tree</param>
		public void CollapseNodeRecursive(int id) {
			if (id != 0)
				ExpandNode(id, false);
			foreach (int child in GetChildNodeIds(id))
				CollapseNodeRecursive(child);
		}
		
		public int[] FindNodes(int root, TreeSearchDelegate filter) {
			int[] arr = new int[0];
			foreach (int id in GetChildNodeIds(root)) {
				arr = (int[])Utils.ArrayAppendRange(arr, FindNodes(id, filter));
				if (filter(GetNodeData(id)))
					arr = (int[])Utils.ArrayAppend(arr, id);
			}
			return arr;
		}

		#endregion
		
		public bool EnableDragDrop {
			get { return enableDragDrop; }
			set {
				enableDragDrop = value;
				#if CLIENT
					if (!Utils.IsNull(selectedJQ)) {
						if (value)
							MakeDraggable(selectedJQ);
						else
							selectedJQ.draggable("destroy");
					}
				#endif
			}
			
		}
		
		public bool HasChecks {
			get { return hasChecks; }
			set {
				#if CLIENT
				if (isAttached) {
					if (value && !hasChecks) {
						// add checkboxes
						JQueryProxy.jQuery("<input type=\"checkbox\" class=\"checkbox\" tabindex=\"-1\"/>").insertBefore(treeElement.find("." + ItemTextClass));
						treeElement.find("input:checkbox").click(checkboxClickHandler);
					}
					else if (!value && hasChecks) {
						// remove checkboxes
						treeElement.find("input:checkbox").remove();
					}
				}
				#endif
				hasChecks = value;
			}
		}
		
		private TreeNode FindNode(int nodeIdToFind, TreeNode nodeToSearch) {
			if (nodeIdToFind == nodeToSearch.id)
				return nodeToSearch;
			if (!Utils.IsNull(nodeToSearch.children)) {
				foreach (TreeNode n in nodeToSearch.children) {
					TreeNode x = FindNode(nodeIdToFind, n);
					if (!Utils.IsNull(x))
						return x;
				}
			}
			return null;
		}
		
		private TreeNode FindParent(int nodeIdToFind, TreeNode parentToSearch) {
			foreach (TreeNode n in parentToSearch.children) {
				if (n.id == nodeIdToFind)
					return parentToSearch;
				if (!Utils.IsNull(n.children)) {
					TreeNode x = FindParent(nodeIdToFind, n);
					if (!Utils.IsNull(x))
						return x;
				}
			}
			return null;
		}

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
			get { return width; }
			set {
				width = value;
				#if CLIENT
					if (isAttached)
						treeElement.width(value - HorzBorderSize);
				#endif
			}
		}

		public int Height {
			get { return height; }
			set {
				height = value;
				#if CLIENT
					if (isAttached)
						treeElement.height(value - 2 * VertBorderSize);
				#endif
			}
		}

		private string GetNodeHtml(TreeNode n) {
			bool hasChildren = !Utils.IsNull(n.children) && Utils.ArrayLength(n.children) > 0;
			string suffix = (hasChildren ? (n.expanded ? ExpandedSuffix : CollapsedSuffix) : LeafSuffix);
			StringBuilder sb = new StringBuilder();
			sb.Append("<div class=\"" + NodeClass + " " + NodeClass + suffix + "\" __nodeid=\"" + Utils.ToStringInvariantInt(n.id) + "\" __data=\"" + Utils.HtmlEncode(Utils.Json(n.data)) + "\">");
			sb.Append("<span class=\"" + ExpandCollapseClass + " " + ExpandCollapseClass + suffix + "\"><img src=\"" + blankImageUrl + "\" alt=\"\"/></span>");
			sb.Append("<span class=\"" + IconClass + " " + IconClass + suffix  + "\"><img src=\"" + blankImageUrl + "\" alt=\"\"/></span>");
			if (hasChecks)
				sb.Append("<input type=\"checkbox\"" + (n.isChecked ? " checked=\"checked\"" : "") + "class=\"checkbox\" tabindex=\"-1\"/>");
			sb.Append("<span class=\"" + ItemTextClass + "\">" + Utils.HtmlEncode(n.text) + "</span>");
			if (hasChildren) {
				sb.Append("<div class=\"" + NestedListClass + "\"" + (n.expanded ? "" : " style=\"display: none\"") + ">");
				foreach (TreeNode c in n.children) {
                    sb.Append(SpacerHtml);
                    sb.Append(GetNodeHtml(c));
				}
				sb.Append("</div>");
			}
			sb.Append("</div>");
			return sb.ToString();
		}
		
		private string SpacerHtml {get { return "<div class=\"" + SpacerClass + "\"><img src=\"" + blankImageUrl + "\" alt=\"\"/></div>"; } }

		private string InnerHtml {
			get {
				StringBuilder sb = new StringBuilder();
				foreach (TreeNode c in invisibleRoot.children) {
                    sb.Append(SpacerHtml);
                    sb.Append(GetNodeHtml(c));
				}
				return sb.ToString();
			}
		}

		public string Html {
			get {
				if (string.IsNullOrEmpty(id))
					throw new Exception("Must set ID before render");
			
				string style = PositionHelper.CreateStyle(position, width - HorzBorderSize, height - VertBorderSize);
				return "<div tabindex=\"0\" id=\"" + id + "\" class=\"ui-widget-content " + ClassName + "\" style=\"" + style + "\">"
				     + InnerHtml
				     + "</div>";
			}
		}
		
		private void InitDefault() {
			width         = 300;
			height        = 300;
			position      = PositionHelper.NotPositioned;
			invisibleRoot = new TreeNode(0, null, null);
			nextNodeId    = 1;
			blankImageUrl = ((ISaltarelleUIService)GlobalServices.Provider.GetService(typeof(ISaltarelleUIService))).BlankImageUrl;
		}
#if SERVER
		public Tree() {
			GlobalServices.Provider.GetService<IScriptManagerService>().RegisterType(GetType());
			GlobalServices.Provider.LoadService<ISaltarelleUIService>();
			InitDefault();
		}
		
		public object ConfigObject {
			get { return new { id, width, height, nextNodeId, enableDragDrop, hasChecks }; }
		}
#endif

#if CLIENT
		[AlternateSignature]
		public extern Tree();
		public Tree(object config) {
			nodeHash = new Dictionary();
			checkboxClickHandler = (JQueryEventHandlerDelegate)Utils.Wrap(new UnwrappedJQueryEventHandlerDelegate(checkbox_Click));
			if (!Script.IsUndefined(config)) {
				InitConfig(Dictionary.GetDictionary(config));
			}
			else
				InitDefault();
		}
		
		private void InitConfig(Dictionary config) {
			id             = (string)config["id"];
			width          = (int)config["width"];
			height         = (int)config["height"];
			nextNodeId     = (int)config["nextNodeId"];
			enableDragDrop = (bool)config["enableDragDrop"];
			hasChecks      = (bool)config["hasChecks"];
			blankImageUrl  = ((ISaltarelleUIService)GlobalServices.Provider.GetService(typeof(ISaltarelleUIService))).BlankImageUrl;
			invisibleRoot  = null;

			Attach();
		}

		public event TreeSelectionChangingEventHandler SelectionChanging;
		public event TreeNodeEventHandler NodeChecked;
		public event EventHandler SelectionChanged;
		public event TreeKeyPressEventHandlerDelegate KeyPress;
		public event TreeDragDropCompletingEventHandler DragDropCompleting;
		public event TreeDragDropCompletedEventHandler DragDropCompleted;

		private JQueryEventHandlerDelegate nodeClickHandler;
		private JQueryEventHandlerDelegate expandCollapseClickHandler;

		private jQuery selectedJQ;

		private Dictionary nodeHash;
		
		private jQuery FindNodeJQ(int nodeId) { return JQueryProxy.jQuery((DOMElement)nodeHash[nodeId.ToString()]); }
		
		public int SelectedId {
			get { return !Utils.IsNull(selectedJQ) && selectedJQ.size() > 0 ? Utils.ParseInt((string)selectedJQ.attr("__nodeid")) : 0; }
			set { SetSelectedJQ(FindNodeJQ(value)); }
		}
		public string SelectedText {
			get { return !Utils.IsNull(selectedJQ) && selectedJQ.size() > 0 ? selectedJQ.children("." + ItemTextClass).text() : null; }
		}
		public object SelectedData {
			get { return !Utils.IsNull(selectedJQ) && selectedJQ.size() > 0 ? Type.GetField(selectedJQ.get(0), "__data") : null; }
		}

		public void Focus() {
			if (isAttached)
				GetElement().Focus();
		}

		private void EnsureVisible(jQuery n) {
			DOMElement d = treeElement.get(0);
			double offsetTop = n.offset().top - treeElement.offset().top, scrollTop = treeElement.scrollTop(), nHeight = n.children("." + ItemTextClass).outerHeight(), treeHeight = d.ClientHeight;

			if (offsetTop < 0) {
				treeElement.scrollTop(Math.Round(scrollTop + offsetTop));
			}
			else if (offsetTop + nHeight > treeHeight) {
				treeElement.scrollTop(Math.Round(scrollTop + offsetTop + nHeight - treeHeight));
			}
		}
		
		private void MakeDraggable(jQuery node) {
			node.children("." + ItemTextClass).draggable(new Dictionary("helper", "clone",
				                                                        "appendTo", treeElement,
			                                                            "containment", "parent",
			                                                            "start", Utils.Wrap(new UnwrappedDraggableEventHandlerDelegate(NodeContent_DragStart)),
			                                                            "stop", Utils.Wrap(new UnwrappedDraggableEventHandlerDelegate(NodeContent_DragStop))
			                                              ));
		}

		private bool SetSelectedJQ(jQuery value) {
			if (value.size() == 0)
				value = null;
			if (SelectedId == (!Utils.IsNull(value) ? Utils.ParseInt((string)value.attr("__nodeid")) : 0))
				return true;
			
			if (!RaiseSelectionChanging())
				return false;
		
			if (!Utils.IsNull(selectedJQ)) {
				selectedJQ.children("." + ItemTextClass).removeClass(SelectedNodeClass)
				                                          .draggable("destroy");
			}
			
			selectedJQ = value;

			if (!Utils.IsNull(value)) {
				EnsureExpandedTo(selectedJQ);
				EnsureVisible(selectedJQ);
				selectedJQ.children("." + ItemTextClass).addClass(SelectedNodeClass);
				if (enableDragDrop)
					MakeDraggable(selectedJQ);
			}

			OnSelectionChanged(EventArgs.Empty);
			
			return true;
		}
		
		private void EnsureExpandedTo(jQuery node) {
			for (;;) {
				node = Utils.Parent(node, "." + ClassName + ",." + NodeClass);
				if (node.isInExpression("." + ClassName))
					break;
				ExpandNodeJQ(node, true);
			}
		}

		private bool ExpandNodeJQ(jQuery node, bool expanded) {
			bool leaf = node.isInExpression("." + NodeClass + LeafSuffix);
			
			if (!leaf && !expanded && !Utils.IsNull(selectedJQ) && selectedJQ.size() > 0) {
				// if we are collapsing the node containing the current selected node, we have to make sure it's OK and also select this node.
				jQuery n = selectedJQ;
				for (;;) {
					n = Utils.Parent(n, "." + ClassName + ",." + NodeClass);
					if (n.isInExpression("." + ClassName))
						break;
					if (n.get(0) == node.get(0)) {
						if (!SetSelectedJQ(node)) // try to change the selection
							return false;
						break;
					}
				}
			}

			jQuery expandCollapseIcon = node.children("." + ExpandCollapseClass), nodeIcon = node.children("." + IconClass);

			if (!leaf && expanded) {
				node.addClass(NodeClass + ExpandedSuffix);
				expandCollapseIcon.addClass(ExpandCollapseClass + ExpandedSuffix);
				nodeIcon.addClass(IconClass + ExpandedSuffix);
			}
			else {
				node.removeClass(NodeClass + ExpandedSuffix);
				expandCollapseIcon.removeClass(ExpandCollapseClass + ExpandedSuffix);
				nodeIcon.removeClass(IconClass + ExpandedSuffix);
			}

			if (!leaf && !expanded) {
				node.addClass(NodeClass + CollapsedSuffix);
				expandCollapseIcon.addClass(ExpandCollapseClass + CollapsedSuffix);
				nodeIcon.addClass(IconClass + CollapsedSuffix);
			}
			else {
				node.removeClass(NodeClass + CollapsedSuffix);
				expandCollapseIcon.removeClass(ExpandCollapseClass + CollapsedSuffix);
				nodeIcon.removeClass(IconClass + CollapsedSuffix);
			}

			if (leaf) {
				node.addClass(NodeClass + LeafSuffix);
				expandCollapseIcon.addClass(ExpandCollapseClass + LeafSuffix);
				nodeIcon.addClass(IconClass + LeafSuffix);
			}
			else {
				node.removeClass(NodeClass + LeafSuffix);
				expandCollapseIcon.removeClass(ExpandCollapseClass + LeafSuffix);
				nodeIcon.removeClass(IconClass + LeafSuffix);
			}
			
			node.children("." + NestedListClass).css("display", expanded ? "" : "none");
			
			return true;
		}
		
		private void ToggleNodeExpandedJQ(jQuery node) {
			if (node.isInExpression("." + NodeClass + ExpandedSuffix))
				ExpandNodeJQ(node, false);
			else if (node.isInExpression("." + NodeClass + CollapsedSuffix))
				ExpandNodeJQ(node, true);
		}

		public DOMElement GetElement() { return isAttached ? Document.GetElementById(id) : null; }

		private void BringToLife(jQuery el, bool isBatchAttach) {
			(isBatchAttach ? el.find("." + NodeClass) : el).each(delegate(int index, DOMElement elem) {
				Type.SetField(elem, "__data", jQuery.evalJSON((string)elem.GetAttribute("__data")));
				nodeHash[(string)elem.GetAttribute("__nodeid")] = elem;
				return true;
			});

			el.find("." + ExpandCollapseClass).click(expandCollapseClickHandler);
			
			el.find("." + ItemTextClass).click(nodeClickHandler)
			                              .droppable(new Dictionary("addClasses", false,
			                                                        "hoverClass", NodeDropHoverClass,
			                                                        "tolerance", "pointer",
			                                                        "drop", Utils.Wrap(new UnwrappedDroppableEventHandlerDelegate(Node_Drop))
			                               ))
			                              .addClass("ui-droppable");
			                              
			if (hasChecks)
				el.find("input:checkbox").click(checkboxClickHandler);

            (isBatchAttach ? el.find("." + SpacerClass) : el.prev()).droppable(new Dictionary("addClasses", false,
                                                                                               "hoverClass", SpacerDropHoverClass,
                                                                                               "tolerance", "pointer",
                                                                                               "drop", Utils.Wrap(new UnwrappedDroppableEventHandlerDelegate(Spacer_Drop))
                                                                      ))
                                                                     .addClass("ui-droppable");
		}

		private void NodeContent_DragStart(DOMElement _this, JQueryEvent e, DraggableEventObject ui) {
			SetSelectedJQ(Utils.Parent(JQueryProxy.jQuery(_this), "." + NodeClass));
			selectedJQ.find(".ui-droppable").droppable("disable");
			treeElement.focus();
		}
		
		private void ChildRemoved(jQuery parentNode) {
			jQuery q = parentNode.children("." + NestedListClass + ":empty");
			if (q.size() > 0) {
				q.remove();
				parentNode.removeClass(NodeClass + ExpandedSuffix);
				parentNode.removeClass(NodeClass + CollapsedSuffix);
				parentNode.addClass(NodeClass + LeafSuffix);
				ExpandNodeJQ(parentNode, false);
			}
		}
		
		private void Node_Drop(DOMElement _this, JQueryEvent e, DroppableEventObject ui) {
			jQuery draggedNode = Utils.Parent(ui.draggable, "." + NodeClass);
			jQuery dropNode = Utils.Parent(JQueryProxy.jQuery(_this), "." + NodeClass), list = dropNode.children("." + NestedListClass);
			int draggedNodeId = Utils.ParseInt((string)draggedNode.attr("__nodeid"));
			int dropParentId  = Utils.ParseInt((string)dropNode.attr("__nodeid"));
			int dropIndex     = dropNode.children("." + NestedListClass).children("." + NodeClass).size() - (Utils.Parent(draggedNode, "." + NodeClass + ",." + ClassName).get(0) == dropNode.get(0) ? 1 : 0);

			TreeDragDropCompletingEventArgs completingArgs = new TreeDragDropCompletingEventArgs(draggedNodeId, dropParentId, dropIndex);
			OnDragDropCompleting(completingArgs);
			if (completingArgs.Cancel)
				return;
		
			if (list.size() == 0) {
				list = JQueryProxy.jQuery("<div class=\"" + NestedListClass + "\"></div>").appendTo(dropNode);
				dropNode.removeClass(NodeClass + LeafSuffix);
				dropNode.addClass(NodeClass + ExpandedSuffix);
			}
			ExpandNodeJQ(dropNode, true);
			jQuery oldParent = Utils.Parent(draggedNode, "." + NodeClass + ",." + ClassName);
			draggedNode.prev().appendTo(list);
			draggedNode.appendTo(list);
			if (oldParent.isInExpression("." + NodeClass))
				ChildRemoved(oldParent);

			OnDragDropCompleted(new TreeDragDropCompletedEventArgs(draggedNodeId, dropParentId, dropIndex));
		}

		private void Spacer_Drop(DOMElement _this, JQueryEvent e, DroppableEventObject ui) {
			jQuery spacer = JQueryProxy.jQuery(_this), draggedNode = Utils.Parent(ui.draggable, "." + NodeClass);
			if (spacer.get(0) != draggedNode.prev().get(0)) {
				jQuery oldParent = Utils.Parent(draggedNode, "." + NodeClass + ",." + ClassName);
				jQuery newParent = Utils.Parent(spacer, "." + NodeClass + ",." + ClassName);
				int draggedNodeId = Utils.ParseInt((string)draggedNode.attr("__nodeid"));
				int dropParentId  = newParent.isInExpression("." + NodeClass) ? Utils.ParseInt((string)newParent.attr("__nodeid")) : 0;
				int dropIndex     = (newParent.isInExpression("." + ClassName) ? newParent : newParent.children("." + NestedListClass)).children("." + SpacerClass).index(spacer.get(0));
				if (oldParent.get(0) == newParent.get(0) && (oldParent.isInExpression("." + ClassName) ? oldParent : oldParent.children("." + NestedListClass)).children("." + NodeClass).index(draggedNode.get(0)) < dropIndex)
					dropIndex--;	// In case we're draggin down within the same parent, we shouldn't count the dragged node.

				// don't do anything if dropping just before where it was. This would cause problems since spacer would be inserted before itself.
				TreeDragDropCompletingEventArgs completingArgs = new TreeDragDropCompletingEventArgs(draggedNodeId, dropParentId, dropIndex);
				OnDragDropCompleting(completingArgs);
				if (completingArgs.Cancel)
					return;
				
				spacer.before(draggedNode.prev());
				spacer.before(draggedNode);
				if (oldParent.isInExpression("." + NodeClass))
					ChildRemoved(oldParent);

				OnDragDropCompleted(new TreeDragDropCompletedEventArgs(draggedNodeId, dropParentId, dropIndex));
			}
		}
		
		private void NodeContent_DragStop(DOMElement _this, JQueryEvent e, DraggableEventObject ui) {
			Utils.Parent(JQueryProxy.jQuery(_this), "." + NodeClass).find(".ui-droppable").droppable("enable");
		}
		
		private void checkbox_Click(DOMElement _this, JQueryEvent e) {
			jQuery node = Utils.Parent(JQueryProxy.jQuery(_this), "." + NodeClass);
			int nodeId = Utils.ParseInt((string)node.attr("__nodeid"));
			OnNodeChecked(new TreeNodeEventArgs(nodeId));
		}

		public void Attach() {
			if (Utils.IsNull(id) || isAttached)
				throw new Exception("Must set ID and can only attach once");
			isAttached = true;
			treeElement = JQueryProxy.jQuery(GetElement());

			treeElement.bind("selectstart", null, new BasicCallback(delegate { return false; }));
			
			expandCollapseClickHandler = (JQueryEventHandlerDelegate)Utils.Wrap(new UnwrappedJQueryEventHandlerDelegate(delegate(DOMElement _this, JQueryEvent e) {
				ToggleNodeExpandedJQ(Utils.Parent(JQueryProxy.jQuery(_this), "." + NodeClass));
				e.stopPropagation();
			}));

			nodeClickHandler = (JQueryEventHandlerDelegate)Utils.Wrap(new UnwrappedJQueryEventHandlerDelegate(delegate(DOMElement _this, JQueryEvent e) {
				treeElement.focus();
				SetSelectedJQ(Utils.Parent(JQueryProxy.jQuery(_this), "." + NodeClass));
				e.stopPropagation();
			}));

			BringToLife(treeElement, true);

			jQuery q = treeElement.children("." + NodeClass + ":first");
			if (q.size() > 0)
				SetSelectedJQ(q);

			UIUtils.AttachKeyPressHandler(treeElement.get(0), el_KeyDown);
		}
		
		private void el_KeyDown(JQueryEvent e) {
			TreeKeyPressEventArgs ev = new TreeKeyPressEventArgs();
			ev.KeyCode = e.keyCode;
			OnKeyPress(ev);
			if (ev.PreventDefault) {
				e.preventDefault();
				return;
			}

			switch (e.keyCode) {
				case 32: {
					// Space - used to toggle checkmark if there is one.
					if (hasChecks) {
						jQuery q = selectedJQ.children("input:checkbox");
						if (q.size() > 0) {
							((CheckBoxElement)q.get(0)).Checked = !((CheckBoxElement)q.get(0)).Checked;
							OnNodeChecked(new TreeNodeEventArgs(SelectedId));
							e.preventDefault();
						}
					}
					break;
				}
			
				case 37:
					// key left - if current node exists and is expanded: collapse it, otherwise navigate to its parent
					if (selectedJQ.isInExpression("." + NodeClass + ExpandedSuffix)) {
						ExpandNodeJQ(selectedJQ, false);
					}
					else {
						jQuery parentNode = Utils.Parent(selectedJQ, "." + ClassName + ",." + NodeClass);
						if (parentNode.isInExpression("." + NodeClass))
							SetSelectedJQ(parentNode);
					}
					e.preventDefault();
					break;

				case 38:
					// key up - navigate to previous sibling or parent
					jQuery prevSibling = Utils.Prev(selectedJQ, "." + NodeClass);
					if (prevSibling.size() > 0) {
						// navigate to the most expanded node
						while (prevSibling.isInExpression("." + NodeClass + ExpandedSuffix))
							prevSibling = prevSibling.children("." + NestedListClass).children("." + NodeClass + ":last");
						SetSelectedJQ(prevSibling);
					}
					else {
						jQuery parentNode = Utils.Parent(selectedJQ, "." + ClassName + ",." + NodeClass);
						if (parentNode.isInExpression("." + NodeClass))
							SetSelectedJQ(parentNode);
					}
					e.preventDefault();
					break;
					
				case 39:
					// key right - if current node has children: expand if collapsed, navigate to first child if expanded
					if (selectedJQ.isInExpression("." + NodeClass + CollapsedSuffix)) {
						ExpandNodeJQ(selectedJQ, true);
					}
					else if (selectedJQ.isInExpression("." + NodeClass + ExpandedSuffix)) {
						SetSelectedJQ(selectedJQ.children("." + NestedListClass).children("." + NodeClass + ":first"));
					}
					e.preventDefault();
					break;
					
				case 40:
					// key down
					if (selectedJQ.isInExpression("." + NodeClass + ExpandedSuffix)) {
						// navigate to the first child
						SetSelectedJQ(selectedJQ.children("." + NestedListClass).children("." + NodeClass + ":first"));
					}
					else {
						// navigate to the next sibling in the lowest level in the hierarachy where there is one.
						for (jQuery jq = selectedJQ; !jq.isInExpression("." + ClassName); jq = jq.parent()) {
							if (jq.isInExpression("." + NodeClass)) {
								jQuery nextSibling = Utils.Next(jq, "." + NodeClass);
								if (nextSibling.size() > 0) {
									SetSelectedJQ(nextSibling);
									break;
								}
							}
						}
					}
					e.preventDefault();
					break;
			}
		}
		
		private bool RaiseSelectionChanging() {
			TreeSelectionChangingEventArgs e = new TreeSelectionChangingEventArgs();
			OnSelectionChanging(e);
			return !e.Cancel;
		}

		protected virtual void OnSelectionChanging(TreeSelectionChangingEventArgs e) {
			if (!Utils.IsNull(SelectionChanging))
				SelectionChanging(this, e);
		}
		
		protected virtual void OnSelectionChanged(EventArgs e) {
			if (!Utils.IsNull(SelectionChanged))
				SelectionChanged(this, e);
		}

		protected virtual void OnNodeChecked(TreeNodeEventArgs e) {
			if (!Utils.IsNull(NodeChecked))
				NodeChecked(this, e);
		}

		protected virtual void OnKeyPress(TreeKeyPressEventArgs e) {
			if (!Utils.IsNull(KeyPress))
				KeyPress(this, e);
		}
		
		protected virtual void OnDragDropCompleting(TreeDragDropCompletingEventArgs e) {
			if (!Utils.IsNull(DragDropCompleting))
				DragDropCompleting(this, e);
		}

		protected virtual void OnDragDropCompleted(TreeDragDropCompletedEventArgs e) {
			if (!Utils.IsNull(DragDropCompleted))
				DragDropCompleted(this, e);
		}
#endif
	}
}
