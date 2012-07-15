using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
#if SERVER
using System.Text;
using System.Linq;
using Newtonsoft.Json;
using Saltarelle.Ioc;
#endif
#if CLIENT
using System.Html;
using System.Text;
using jQueryApi;
using jQueryApi.UI.Interactions;
#endif

namespace Saltarelle.UI {
	public enum TreeNodeCheckState {
		no  = 0,
		yes = 1,
		indeterminate = 2
	}

	public delegate bool TreeNodeFindPredicate(TreeNode node);

	/// <summary>
	/// Represents a tree node. Instances of this class are manipulated using static methods on the <see cref="Tree"/> class.
	/// Instances are created by the <see cref="Tree.CreateTreeNode"/> method.
	/// </summary>
	[Record]
	public sealed class TreeNode
	{
		#if CLIENT
		[PreserveName]
		#else
		[JsonProperty]
		#endif
		internal int id;

		#if CLIENT
		[PreserveName]
		#else
		[JsonProperty]
		#endif
		internal bool expanded;

		#if CLIENT
		[PreserveName]
		#else
		[JsonProperty]
		#endif
		internal TreeNodeCheckState checkState;

		#if CLIENT
		[PreserveName]
		#else
		[JsonProperty]
		#endif
		internal string text;

		#if CLIENT
		[PreserveName]
		#else
		[JsonProperty]
		#endif
		internal string icon;

		#if CLIENT
		[PreserveName]
		#else
		[JsonProperty]
		#endif
		internal object data;

		#if CLIENT
		[PreserveName]
		#else
		[JsonProperty]
		#endif
		internal List<TreeNode> children;

		#if CLIENT
		[PreserveName]
		#else
		[JsonIgnore]
		#endif
		public Tree treeIfRoot;
		
		#if CLIENT
		[PreserveName]
		#else
		[JsonIgnore]
		#endif
		public TreeNode parent;

		internal TreeNode() {
			this.id         = Tree.nextNodeId++;
			this.text       = null;
			this.data       = null;
			this.icon       = Tree.DefaultIcon;
			this.children   = new List<TreeNode>();
			this.expanded   = false;
			this.checkState = TreeNodeCheckState.no;
			this.treeIfRoot = null;
			this.parent     = null;
		}
	}

#if CLIENT
	public class TreeSelectionChangingEventArgs : EventArgs {
		public bool Cancel;
		public TreeNode NewSelection;
	}
	public delegate void TreeSelectionChangingEventHandler(object sender, TreeSelectionChangingEventArgs e);
	
	public class TreeNodeEventArgs : EventArgs {
		public TreeNode Node;
		public TreeNodeEventArgs(TreeNode node) {
			this.Node = node;
		}
	}
	public delegate void TreeNodeEventHandler(object sender, TreeNodeEventArgs e);

	public class TreeKeyPressEventArgs : EventArgs {
		public int KeyCode;
		public bool PreventDefault;
	}
	public delegate void TreeKeyPressEventHandlerDelegate(object sender, TreeKeyPressEventArgs e);

	public class TreeDragDropCompletingEventArgs : CancelEventArgs {
		public TreeNode DraggedNode;
		public TreeNode NewParent;
		public int PositionWithinNewParent;

		public TreeDragDropCompletingEventArgs(TreeNode draggedNode, TreeNode newParent, int positionWithinNewParent) {
			this.DraggedNode             = draggedNode;
			this.NewParent               = newParent;
			this.PositionWithinNewParent = positionWithinNewParent;
			this.Cancel                  = false;
		}
	}
	public delegate void TreeDragDropCompletingEventHandler(object sender, TreeDragDropCompletingEventArgs e);

	public class TreeDragDropCompletedEventArgs : EventArgs {
		public TreeNode DraggedNode;
		public TreeNode NewParent;
		public int PositionWithinNewParent;

		public TreeDragDropCompletedEventArgs(TreeNode draggedNode, TreeNode newParent, int positionWithinNewParent) {
			this.DraggedNode             = draggedNode;
			this.NewParent               = newParent;
			this.PositionWithinNewParent = positionWithinNewParent;
		}
	}
	public delegate void TreeDragDropCompletedEventHandler(object sender, TreeDragDropCompletedEventArgs e);

	internal delegate TreeNode TreeNodeMapperDelegate(TreeNode arg);

	[Record]
	internal sealed class TreeDropTarget {
		public readonly TreeNode node;
		public readonly bool above;
		public TreeDropTarget(TreeNode node, bool above) {
			this.node  = node;
			this.above = above;
		}
	}
#endif

	public class Tree : IControl, IClientCreateControl, IResizableX, IResizableY {
		public const string DefaultIcon = "default-tree-icon";

		public const string NestedListClass      = "tree-nested-list";
		public const string ItemTextClass        = "tree-node-text";
		public const string ContainerClass       = "tree-node-container";
		public const string IconClass            = "tree-node-icon";
		public const string ExpandCollapseClass  = "expand-collapse-icon";
		public const string ExpandedSuffix       = "-expanded";
		public const string CollapsedSuffix      = "-collapsed";
		public const string LeafSuffix           = "-leaf";
		public const string SelectedNodeClass    = "ui-state-highlight";
		public const string CheckBoxClass        = "checkbox";
		public const string NodeClass            = "tree-node";
		public const string DropIntoClass        = "tree-node-dropinto";
		public const string DropAboveClass       = "tree-node-dropabove";
		public const string NodeIdPrefix         = "tn$";
		public const int    HorzBorderSize = 2;
		public const int    VertBorderSize = 2;

		public const string TreeClasses       = "ui-widget-content Tree";
		public const string DisabledTreeClass = "TreeDisabled";

		internal static int nextNodeId = 0;
		private TreeNode invisibleRoot;

		public TreeNode InvisibleRoot { get { return invisibleRoot; } }

		private string   id;
		private Position position = PositionHelper.NotPositioned;
		private int      width;
		private int      height;
		private int      tabIndex;
		private bool     hasChecks;
		private bool     enabled;
		private TreeNode selectedNode;
		private bool     enableDragDrop;
		private bool     autoCheckHierarchy;

        private ISaltarelleUIService uiService;

        #if SERVER
        [ClientInject]
        #endif
        public ISaltarelleUIService UIService {
            get { return uiService; }
            set { uiService = value; }
        }

		#if CLIENT
			// Drag-drop fields
			private bool           isAttached;
			private int            itemHeight;
			private TreeDropTarget currentDropTarget;

			private jQueryEventHandler dragFeedbackHandler;

			public event TreeSelectionChangingEventHandler SelectionChanging;
			public event TreeNodeEventHandler NodeChecked;
			public event EventHandler SelectionChanged;
			public event TreeKeyPressEventHandlerDelegate KeyPress;
			public event TreeDragDropCompletingEventHandler DragDropCompleting;
			public event TreeDragDropCompletedEventHandler DragDropCompleted;
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
					if (isAttached)
						GetElement().TabIndex = enabled ? value : -1;
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
						jQuery.FromElement(GetElement()).Width(value - HorzBorderSize);
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
					if (isAttached) {
						jQuery.FromElement(GetElement()).Height(value - VertBorderSize);
					}
				#endif
				height = value;
			}
		}

		public bool HasChecks {
			get { return hasChecks; }
			set {
				hasChecks = value;
				#if CLIENT
					if (isAttached)
						jQuery.FromElement(GetElement()).Html(InnerHtml);
				#endif
			}
		}
		
		public bool Enabled {
			get { return enabled; }
			set {
				#if CLIENT
					if (isAttached && value != enabled) {
						Element elem = GetElement();
						elem.TabIndex = enabled ? tabIndex : -1;
						elem.ClassName = TreeClasses + (value ? "" : (" " + DisabledTreeClass));
						if (value && enableDragDrop) {
							if (selectedNode != null)
								MakeDraggable(selectedNode, true);
							EnableDroppable(true);
						}
						else {
							if (selectedNode != null)
								MakeDraggable(selectedNode, false);
							EnableDroppable(true);
						}
					}
				#endif
				enabled = value;
			}
		}
		
		public bool EnableDragDrop {
			get { return enableDragDrop; }
			set {
				#if CLIENT
					if (isAttached && value != enableDragDrop) {
						if (enabled && value) {
							if (selectedNode != null)
								MakeDraggable(selectedNode, true);
							EnableDroppable(true);
						}
						else {
							if (selectedNode != null)
								MakeDraggable(selectedNode, false);
							EnableDroppable(true);
						}
					}
				#endif
				enableDragDrop = value;
			}
		}
		
		public bool AutoCheckHierarchy {
			get { return autoCheckHierarchy; }
			set { autoCheckHierarchy = value; }
		}
		
		public TreeNode SelectedNode {
			get {
				return selectedNode;
			}
			set {
				if (value != null) {
					Tree t = GetTree(value);
					if (t != this)
						throw new Exception("Node is not in tree");
				}
				SetSelection(value, true, true);
			}
		}
		
		private bool SetSelection(TreeNode newSelection, bool raiseSelectionChanging, bool raiseSelectionChanged) {
			if (newSelection != null)
				EnsureExpandedTo(newSelection);

			#if CLIENT
				if (raiseSelectionChanging) {
					if (!RaiseSelectionChanging(newSelection))
						return false;
				}

				if (isAttached) {
					if (selectedNode != null) {
						// Remove the previous selection
						var jq = jQuery.FromElement(GetNodeElement(selectedNode).Children[0]);
						Element d = jq.Children("." + ItemTextClass).GetElement(0);
						d.ClassName = ItemTextClass;
						if (enableDragDrop)
							MakeDraggable(selectedNode, false);
					}

					if (newSelection != null) {
						var jq = jQuery.FromElement(GetNodeElement(newSelection).Children[0]);
						Element d = jq.Children("." + ItemTextClass).GetElement(0);
						d.ClassName = ItemTextClass + " " + SelectedNodeClass;
						if (enableDragDrop)
							MakeDraggable(newSelection, true);
						EnsureVisible(newSelection);
					}
				}
			#endif

			selectedNode = newSelection;

			#if CLIENT
				if (raiseSelectionChanged)
					OnSelectionChanged(EventArgs.Empty);
			#endif
			return true;
		}
		
		private void AppendNestedListHtml(List<TreeNode> children, StringBuilder sb) {
			sb.Append("<div class=\"" + NestedListClass + "\"" + ">");
			for (int i = 0; i < children.Count; i++)
				AppendNodeHtml((TreeNode)children[i], sb);
			sb.Append("</div>");
		}

		private void AppendNodeHtml(TreeNode n, StringBuilder sb) {
			bool hasChildren = n.children != null && n.children.Count > 0;
			string suffix = (hasChildren ? (n.expanded ? ExpandedSuffix : CollapsedSuffix) : LeafSuffix);
            string blankImageUrl = uiService.BlankImageUrl;

			sb.Append("<div class=\"" + ContainerClass + " " + ContainerClass + suffix + "\" id=\"" + NodeIdPrefix + Utils.ToStringInvariantInt(n.id) + "\">"
			        +     "<div class=\"" + NodeClass + " " + NodeClass + suffix + "\">"
			        +         "<span class=\"" + ExpandCollapseClass + " " + ExpandCollapseClass + suffix + "\"><img src=\"" + blankImageUrl + "\" alt=\"\"/></span>"
			        +         "<span class=\"" + IconClass + " " + IconClass + suffix + " " + n.icon + " " + n.icon + suffix + "\"><img src=\"" + blankImageUrl + "\" alt=\"\"/></span>");
			if (hasChecks)
				sb.Append(    "<input type=\"checkbox\"" + (n.checkState == TreeNodeCheckState.yes ? " checked=\"checked\"" : "") + (n.checkState == TreeNodeCheckState.yes ? " defaultChecked=\"defaultChecked\"" : "") + (n.checkState == TreeNodeCheckState.indeterminate ? " indeterminate=\"indeterminate\"" : "") + "class=\"" + CheckBoxClass + "\" tabindex=\"-1\"/>");
			sb.Append(        "<span class=\"" + ItemTextClass + (n == selectedNode ? " " + SelectedNodeClass : "") + "\">" + Utils.HtmlEncode(n.text) + "</span>");
			sb.Append(    "</div>");
			if (hasChildren && n.expanded)
				AppendNestedListHtml(n.children, sb);
			sb.Append("</div>");
		}

		protected virtual string InnerHtml {
			get {
				StringBuilder sb = new StringBuilder();
				foreach (TreeNode c in invisibleRoot.children)
					AppendNodeHtml(c, sb);
				return sb.ToString();
			}
		}

		public virtual string Html {
			get {
				if (string.IsNullOrEmpty(id))
					throw new Exception("Must set ID before render");
			
				string style = PositionHelper.CreateStyle(position, width - HorzBorderSize, height - VertBorderSize);
				return "<div tabindex=\"" + Utils.ToStringInvariantInt(enabled ? tabIndex : -1) + "\" id=\"" + id + "\" class=\"" + TreeClasses + (enabled ? "" : " " + DisabledTreeClass) + "\" style=\"" + style + "\">"
				     +     InnerHtml
				     + "</div>";
			}
		}

		protected virtual void InitDefault() {
			invisibleRoot = new TreeNode();
			invisibleRoot.treeIfRoot = this;
			invisibleRoot.expanded   = true;
			selectedNode  = null;
			enabled       = true;
			width         = 300;
			height        = 300;
			tabIndex      = 0;
		}

#if SERVER
		public Tree() {
			InitDefault();
		}

		protected virtual void AddItemsToConfigObject(Dictionary<string, object> config) {
			config["id"]                 = id;
			config["width"]              = width;
			config["height"]             = height;
			config["tabIndex"]           = tabIndex;
			config["hasChecks"]          = hasChecks;
			config["enabled"]            = enabled;
			config["invisibleRoot"]      = invisibleRoot;
			config["enableDragDrop"]     = enableDragDrop;
			config["autoCheckHierarchy"] = autoCheckHierarchy;
			config["nextNodeId"]         = nextNodeId;	// Yes, it is static, but the worst thing that can happen is that we assign the member more than once during startup.
			config["selectionPath"]      = (selectedNode != null ? GetTreeNodePath(selectedNode, invisibleRoot) : null);
		}

		public object ConfigObject {
			get {
				var config = new Dictionary<string, object>();
				AddItemsToConfigObject(config);
				return config;
			}
		}
#endif
		
#if CLIENT
		[AlternateSignature]
		public Tree() {}
		public Tree(object config) {
			dragFeedbackHandler = new jQueryEventHandler(Element_DragFeedback);
			if (!Script.IsUndefined(config)) {
				InitConfig(JsDictionary.GetDictionary(config));
			}
			else
				InitDefault();
		}
		
		protected virtual void InitConfig(JsDictionary config) {
			id                 = (string)config["id"];
			width              = (int)config["width"];
			height             = (int)config["height"];
			tabIndex           = (int)config["tabIndex"];
			hasChecks          = (bool)config["hasChecks"];
			enabled            = (bool)config["enabled"];
			invisibleRoot      = (TreeNode)config["invisibleRoot"];
			enableDragDrop     = (bool)config["enableDragDrop"];
			autoCheckHierarchy = (bool)config["autoCheckHierarchy"];
			nextNodeId         = (int)config["nextNodeId"];

			FixTreeAfterDeserialize(invisibleRoot);
			invisibleRoot.treeIfRoot = this;
			invisibleRoot.parent     = null;
			
			int[] selectionPath = (int[])config["selectionPath"];
			selectedNode = selectionPath != null ? FollowTreeNodePath(invisibleRoot, selectionPath) : null;

			Attach();
		}
		
		private Element GetNodeElement(TreeNode node) { return Document.GetElementById(NodeIdPrefix + Utils.ToStringInvariantInt(node.id)); }
		
		public Element GetElement() { return isAttached ? Document.GetElementById(id) : null; }

		public void Attach() {
			if (id == null || isAttached)
				throw new Exception("Must set ID and can only attach once");
			isAttached = true;
			Element elem = GetElement();

			UIUtils.AttachKeyPressHandler(elem, Element_KeyPress);
			jQuery.FromElement(elem).Click(Element_Click);
			if (selectedNode != null)
				EnsureVisible(selectedNode);
			
			if (enableDragDrop && enabled) {
				if (selectedNode != null)
					MakeDraggable(selectedNode, true);
				EnableDroppable(true);
			}
		}
		
		private void EnsureVisible(TreeNode n) {
			Element treeEl = GetElement(), nodeEl = GetNodeElement(n);
			var treeJq = jQuery.FromElement(treeEl);
			var nodeJq = jQuery.FromElement(nodeEl);
			int offsetTop = nodeJq.GetOffset().Top - treeJq.GetOffset().Top, scrollTop = treeJq.GetScrollTop(), nHeight = nodeJq.Children(":eq(0)").GetOuterHeight(), treeHeight = treeEl.ClientHeight;

			if (offsetTop < 0) {
				treeJq.ScrollTop(scrollTop + offsetTop);
			}
			else if (offsetTop + nHeight > treeHeight) {
				treeJq.ScrollTop(scrollTop + offsetTop + nHeight - treeHeight);
			}
		}
		
		private void MakeDraggable(TreeNode node, bool enable) {
			var el = jQuery.FromElement(GetNodeElement(node).Children[0]).Children("." + ItemTextClass);
			if (enable) {
				el.Draggable(new DraggableOptions { Helper      = "clone",
				                                    AppendTo    = GetElement(),
				                                    Scroll      = true,
				                                    Containment = "parent" });
			}
			else
				((DraggableObject)el).Destroy();
		}

		private void Element_DragFeedback(jQueryEvent evt) {
			Element elem = GetElement();

			int elemTop = jQuery.FromElement(elem).GetOffset().Top;
			int offset = evt.PageY - elemTop + elem.ScrollTop;

			int itemIndex  = Math.Truncate(offset / (float)itemHeight) + 1;	// Add one because of the invisible root. Need truncate because Script# doesn't do integer division correctly.
			int posRelItem = offset % itemHeight;
			bool dropAbove;
			if (posRelItem < itemHeight / 4) {
				dropAbove = true;	// Upper quarter - drop above the item
			}
			else if (posRelItem < itemHeight * 3 / 4) {
				dropAbove = false;	// Middle two quarters - drop into the item
			}
			else {
				// Lower quarter - drop above the next item.
				dropAbove = true;
				itemIndex++;
			}

			TreeNode newTarget = null;
			if (itemIndex >= 1) {
				// The itemIndex variable now contains the number of visible items before the currently dragged over node.
				TreeNodeMapperDelegate countDown = null;
				countDown = delegate(TreeNode n) {
					if (itemIndex == 0)
						return n;
					itemIndex--;
					if (n.expanded) {
						for (int i = 0; i < n.children.Count; i++) {
							TreeNode x = countDown(n.children[i]);
							if (x != null)
								return x;
						}
					}
					return null;
				};
				newTarget = countDown(invisibleRoot);
			}
			else
				newTarget = null;

			if (newTarget != null) {
				// Determine if we try to drop into the current dragging node or a child of it.
				for (TreeNode n = newTarget; n != null; n = n.parent) {
					if (n == selectedNode) {
						newTarget = null;
						break;
					}
				}
			}

			ChangeDropTarget(newTarget != null ? new TreeDropTarget(newTarget, dropAbove) : null);
		}

		private void Element_Drop(jQueryEvent evt, DropEvent ui) {
			if (currentDropTarget == null) {
				DragEnded();
				return;
			}
			
			TreeNode dropParent, draggedNode = selectedNode;
			int dropIndex;
			if (currentDropTarget.above) {
				// Drop above the active node.
				dropParent = currentDropTarget.node.parent;
				dropIndex  = GetTreeNodeChildIndex(currentDropTarget.node);
			}
			else {
				// Drop into the active node.
				dropParent = currentDropTarget.node;
				dropIndex  = dropParent.children.Count;
			}

			TreeDragDropCompletingEventArgs completingArgs = new TreeDragDropCompletingEventArgs(draggedNode, dropParent, dropIndex);
			OnDragDropCompleting(completingArgs);
			if (completingArgs.Cancel) {
				DragEnded();
				return;
			}

			SetSelection(null, false, false);	// Temporarily remove the selection.
			
			// Remove the node from its current position.
			Element draggedElem = RemoveTreeNodeDOM(draggedNode);
			draggedNode.parent.children.RemoveAt(GetTreeNodeChildIndex(draggedNode));
			if (draggedNode.parent.children.Count == 0)
				UpdateExpansionClasses(GetNodeElement(draggedNode.parent), draggedNode.parent.icon, false, draggedNode.parent.expanded);	// Need to fix the classes to say that this node is now a leaf.

			// Add the node to its new position.
			if (dropParent.treeIfRoot == null) {
				DoSetTreeNodeExpanded(dropParent, true, true);
				Element dropParentN = GetNodeElement(dropParent);
				if (dropIndex == dropParentN.Children[1].Children.Length)
					dropParentN.Children[1].AppendChild(draggedElem);
				else
					dropParentN.Children[1].InsertBefore(draggedElem, dropParentN.Children[1].Children[dropIndex]);
			}
			else {
				Element elem = GetElement();
				if (dropIndex == elem.Children.Length)
					elem.AppendChild(draggedElem);
				else
					elem.InsertBefore(draggedElem, elem.Children[dropIndex]);
			}

			draggedNode.parent = dropParent;
			dropParent.children.Insert(dropIndex, draggedNode);
			
			// Restore the selection to the new node.
			SetSelection(draggedNode, false, false);

			var completedArgs = new TreeDragDropCompletedEventArgs(draggedNode, dropParent, dropIndex);
			OnDragDropCompleted(completedArgs);

			DragEnded();
		}

		private void ChangeDropTarget(TreeDropTarget newTarget) {
			if ((newTarget == null && currentDropTarget == null) || (newTarget != null && currentDropTarget != null && newTarget.node == currentDropTarget.node && newTarget.above == currentDropTarget.above))
				return;
			if (currentDropTarget != null)
				jQuery.FromElement(GetNodeElement(currentDropTarget.node).Children[0]).RemoveClass(DropIntoClass).RemoveClass(DropAboveClass);
			if (newTarget != null)
				jQuery.FromElement(GetNodeElement(newTarget.node).Children[0]).AddClass(newTarget.above ? DropAboveClass : DropIntoClass);
			currentDropTarget = newTarget;
		}
		
		private void DragEnded() {
			ChangeDropTarget(null);
			jQuery.Document.Unbind("mousemove", dragFeedbackHandler);
		}

		private void EnableDroppable(bool enable) {
			var el = jQuery.FromElement(GetElement());
			if (enable) {
				el.Droppable(new DroppableOptions { Tolerance = "pointer",
                                                    Greedy    = true,
                                                    OnOver    = (_1, _2) => {
				                                                          itemHeight = (invisibleRoot.children.Count > 0 ? jQuery.FromElement(GetNodeElement(invisibleRoot.children[0]).Children[0]).GetOuterHeight() : 1);
				                                                          currentDropTarget = null;
				                                                          jQuery.Document.MouseMove(dragFeedbackHandler);
				                                                      },
				                                    OnOut     = (_1, _2) => DragEnded(),
                                                    OnDrop    = Element_Drop });
			}
			else
				((DroppableObject)el).Destroy();
		}
		
		private TreeNode FindTreeNode(Element nodeElem) {
			// Ugly (but works). We need to find the node, which is done by investigating the ID and walking the tree from the root down.
			// It works, though, and it performs well even on IE6.
			string idStr = nodeElem.ID;
			int id = Utils.ParseInt(Utils.Substring(idStr, NodeIdPrefix.Length, idStr.Length - NodeIdPrefix.Length));
			var result = FindTreeNodes(invisibleRoot, n => n.id == id);
			return result.Count > 0 ? result[0] : null;
		}
		
		private void Element_Click(jQueryEvent evt) {
			if (!enabled)
				return;

			Element elem = GetElement();
			for (Element target = evt.Target; target != elem; target = target.ParentNode) {
				string cls = " " + target.ClassName + " ";
				if (target.TagName.ToLowerCase() == "input") {
					TreeNode n = FindTreeNode(target.ParentNode.ParentNode);
					n.checkState = ((CheckBoxElement)target).Checked ? TreeNodeCheckState.yes : TreeNodeCheckState.no;
					((CheckBoxElement)target).DefaultChecked = ((CheckBoxElement)target).Checked;
					if (autoCheckHierarchy)
						ApplyCheckHierarchy(n);
					OnNodeChecked(new TreeNodeEventArgs(n));
					return;
				}
				if (cls.IndexOf(" " + ItemTextClass + " ") != -1) {
					// When clicking on the text, select the node.
					SetSelection(FindTreeNode(target.ParentNode.ParentNode), true, true);
					return;
				}
				else if (cls.IndexOf(" " + ExpandCollapseClass + " ") != -1) {
					// Expand/collapse the node.
					TreeNode n = FindTreeNode(target.ParentNode.ParentNode);
					DoSetTreeNodeExpanded(n, !n.expanded, false);
					return;
				}
			}
		}
		
		private void Element_KeyPress(jQueryEvent e) {
			if (!RaiseKeyPress(e.Which)) {
				e.PreventDefault();
				return;
			}

			switch (e.Which) {
				case 32: {
					// Space - used to toggle checkmark if there is one.
					if (hasChecks) {
						if (selectedNode != null) {
							SetTreeNodeCheckState(selectedNode, selectedNode.checkState == TreeNodeCheckState.yes ? TreeNodeCheckState.no : TreeNodeCheckState.yes);
						}
						e.PreventDefault();
					}
					break;
				}
			
				case 37: {
					// key left - if current node exists and is expanded: collapse it, otherwise navigate to its parent
					if (selectedNode != null) {
						if (selectedNode.children.Count > 0 && selectedNode.expanded)
							DoSetTreeNodeExpanded(selectedNode, false, false);
						else if (selectedNode.parent != invisibleRoot)
							SetSelection(selectedNode.parent, true, true);
					}
					else if (invisibleRoot.children.Count > 0)
						SetSelection(invisibleRoot.children[0], true, true);
					e.PreventDefault();
					break;
				}

				case 38:
					// key up - navigate to the parent or to the most expanded node in the tree of the previous sibling
					if (selectedNode != null) {
						int index = GetTreeNodeChildIndex(selectedNode);
						if (index == 0) {
							if (selectedNode.parent != invisibleRoot)
								SetSelection(selectedNode.parent, true, true);
						}
						else {
							TreeNode n = (TreeNode)selectedNode.parent.children[index - 1];
							while (n.children.Count > 0 && n.expanded)
								n = (TreeNode)n.children[n.children.Count - 1];
							SetSelection(n, true, true);
						}
					}
					else if (invisibleRoot.children.Count > 0)
						SetSelection(invisibleRoot.children[0], true, true);

					e.PreventDefault();
					break;
					
				case 39:
					// key right - if current node has children: expand if collapsed, navigate to first child if expanded
					if (selectedNode != null) {
						if (selectedNode.children.Count > 0) {
							if (selectedNode.expanded) {
								SetSelection((TreeNode)selectedNode.children[0], true, true);
							}
							else {
								DoSetTreeNodeExpanded(selectedNode, true, false);
							}
						}
					}
					else if (invisibleRoot.children.Count > 0)
						SetSelection((TreeNode)invisibleRoot.children[0], true, true);

					e.PreventDefault();
					break;
					
				case 40: {
					// key down - navigate to the first child if the selected node is expanded, otherwise navigate to the next sibling of the closest node which has a next sibling.
					if (selectedNode != null) {
						if (selectedNode.children.Count > 0 && selectedNode.expanded) {
							SetSelection((TreeNode)selectedNode.children[0], true, true);
						}
						else {
							TreeNode n = selectedNode;
							for (;;) {
								if (n.parent == null)
									break;	// Obviously we are already at the last position.
								int index = GetTreeNodeChildIndex(n);
								if (index < n.parent.children.Count - 1) {
									SetSelection((TreeNode)n.parent.children[index + 1], true, true);
									break;
								}
								n = n.parent;
							}
						}
					}
					else if (invisibleRoot.children.Count > 0)
						SetSelection(invisibleRoot.children[0], true, true);

					e.PreventDefault();
					break;
				}
			}
		}
		
		/// <summary>
		/// Removes a node from the tree, which must be attached. Does not fix the node's parent's child list. Returns the removed element, or null if there was no physical node (eg. if the node has never been expanded to).
		/// </summary>
		private Element RemoveTreeNodeDOM(TreeNode node) {
			if (selectedNode != null && (selectedNode == node || TreeNodeIsChildOf(selectedNode, node))) {
				TreeNode newSelection;

				int childIndex = GetTreeNodeChildIndex(node);
				if (childIndex < node.parent.children.Count - 1)
					newSelection = (TreeNode)node.parent.children[childIndex + 1];
				else if (node.parent.children.Count > 1)
					newSelection = (TreeNode)node.parent.children[node.parent.children.Count - 2];	// - 2 because our caller will remove the last node.
				else if (node.parent.treeIfRoot == null)
					newSelection = node.parent;
				else
					newSelection = null;

				SetSelection(newSelection, false, true);
			}
		
			Element elem = GetNodeElement(node);
			if (elem == null)
				return null;
			Element list = elem.ParentNode;
			elem.ParentNode.RemoveChild(elem);
			if (node.parent.children.Count == 1 && node.parent.treeIfRoot == null)	// In case we are removing the last child, also remove the child list.
				list.ParentNode.RemoveChild(list);
			return elem;
		}
		
		private void SetNodeTextDOM(TreeNode node, string text) {
			Element elem = GetNodeElement(node);
			if (elem != null)
				jQuery.FromElement(elem.Children[0]).Children("." + ItemTextClass).Text(text);
		}

		private void SetNodeIconDOM(TreeNode node, string text) {
			Element elem = GetNodeElement(node);
			if (elem != null) {
				string suffix = (node.children.Count > 0 ? (node.expanded ? ExpandedSuffix : CollapsedSuffix) : LeafSuffix);
				Element iconEl = jQuery.FromElement(elem.Children[0]).Children("." + IconClass).GetElement(0);
				iconEl.ClassName = (IconClass + " " + IconClass + suffix + " " + node.icon + " " + node.icon + suffix);
			}
		}
		
		/// <summary>
		/// Inserts a node into the tree, which must be attached. The parent's child list should not include the new node, and the list is fixed by this method. Returns the inserted element, or null if there was no physical node (eg. if the parent has never been expanded to).
		/// </summary>
		private Element InsertTreeNodeDOM(TreeNode parent, TreeNode toInsert, int position) {
			Element result = null;
			if (parent.treeIfRoot != null) {
				// Modifying the root.
				Element elem = GetElement();
				StringBuilder sb = new StringBuilder();
				AppendNodeHtml(toInsert, sb);
				result = jQuery.FromHtml(sb.ToString()).GetElement(0);
				if (position == parent.children.Count)
					elem.AppendChild(result);
				else
					elem.InsertBefore(result, elem.Children[position]);
			}
			else {
				// Not inserting at the root.
				Element parentEl = GetNodeElement(parent);
				if (parentEl == null)
					return null;

				if (parentEl.Children.Length > 1) {
					// We need to insert the new element into the parent's child list.
					Element listEl = parentEl.Children[1];
					StringBuilder sb = new StringBuilder();
					AppendNodeHtml((TreeNode)toInsert, sb);
					result = jQuery.FromHtml(sb.ToString()).GetElement(0);
					if (position == parent.children.Count)
						listEl.AppendChild(result);
					else
						listEl.InsertBefore(result, listEl.Children[position]);
				}

				if (parent.children.Count == 0)
					UpdateExpansionClasses(parentEl, parent.icon, true, parent.expanded);	// This was the first child we added.
			}

			return result;
		}
		
		private void SetTreeNodeCheckStateDOM(TreeNode node, TreeNodeCheckState checkState) {
			Element nodeElem = GetNodeElement(node);
			if (nodeElem != null) {
				CheckBoxElement cb = (CheckBoxElement)jQuery.FromElement(nodeElem.Children[0]).Children("input").GetElement(0);
				cb.Indeterminate = (checkState == TreeNodeCheckState.indeterminate);
				cb.Checked = (checkState == TreeNodeCheckState.yes);
				cb.DefaultChecked = cb.Checked;
			}
		}
		
		private void UpdateExpansionClasses(Element nodeElem, string icon, bool hasChildren, bool expanded) {
			string suffix = (hasChildren ? (expanded ? ExpandedSuffix : CollapsedSuffix) : LeafSuffix);
			nodeElem.ClassName = ContainerClass + " " + ContainerClass + suffix;
			nodeElem.Children[0].ClassName = NodeClass + " " + NodeClass + suffix;
			nodeElem.Children[0].Children[0].ClassName = ExpandCollapseClass + " " + ExpandCollapseClass + suffix;
			nodeElem.Children[0].Children[1].ClassName = IconClass + " " + IconClass + suffix + " " + icon + " " + icon + suffix;
		}
		
		private void DoSetTreeNodeExpanded(TreeNode node, bool expanded, bool doItEvenIfNoChildren) {
			node.expanded = expanded;
			if (doItEvenIfNoChildren || node.children.Count > 0) {
				Element elem = GetNodeElement(node);
				if (elem != null) {
					if (elem.Children.Length > 1) {
						// The list exists - update its display state.
						elem.Children[1].Style.Display = expanded ? "" : "none";
					}
					else {
						if (expanded) {
							// Expanding and the list does not exist - add it
							StringBuilder sb = new StringBuilder();
							AppendNestedListHtml(node.children, sb);
							jQuery.FromHtml(sb.ToString()).AppendTo(jQuery.FromElement(elem));
						}
					}

					UpdateExpansionClasses(elem, node.icon, node.children.Count > 0, expanded);
				}
			}

			if (selectedNode != null && !expanded && TreeNodeIsChildOf(selectedNode, node)) {
				SetSelection(node, false, true);
			}
		}
		
		public void Focus() {
			if (isAttached) {
				try {
					GetElement().Focus();
				}
				catch (Exception) {
				}
			}
		}

		#region Event raisers

		private bool RaiseSelectionChanging(TreeNode newSelection) {
			var e = new TreeSelectionChangingEventArgs { Cancel = false, NewSelection = newSelection };
			OnSelectionChanging(e);
			return !e.Cancel;
		}
		
		private bool RaiseKeyPress(int keyCode) {
			var e = new TreeKeyPressEventArgs { KeyCode = keyCode };
			OnKeyPress(e);
			return !e.PreventDefault;
		}

		protected virtual void OnSelectionChanging(TreeSelectionChangingEventArgs e) {
			if (SelectionChanging != null)
				SelectionChanging(this, e);
		}
		
		protected virtual void OnSelectionChanged(EventArgs e) {
			if (SelectionChanged != null)
				SelectionChanged(this, e);
		}

		protected virtual void OnNodeChecked(TreeNodeEventArgs e) {
			if (NodeChecked != null)
				NodeChecked(this, e);
		}

		protected virtual void OnKeyPress(TreeKeyPressEventArgs e) {
			if (KeyPress != null)
				KeyPress(this, e);
		}
		
		protected virtual void OnDragDropCompleting(TreeDragDropCompletingEventArgs e) {
			if (DragDropCompleting != null)
				DragDropCompleting(this, e);
		}

		protected virtual void OnDragDropCompleted(TreeDragDropCompletedEventArgs e) {
			if (DragDropCompleted != null)
				DragDropCompleted(this, e);
		}
		#endregion
#endif

		#region TreeNode manipulators

		private static void FixTreeAfterDeserializeInt(TreeNode n) {
			for (int i = 0; i < n.children.Count; i++) {
				FixTreeAfterDeserializeInt(n.children[i]);
				((TreeNode)n.children[i]).parent = n;
			}
			n.treeIfRoot = null;
		}

		public static void FixTreeAfterDeserialize(TreeNode rootNode) {
			FixTreeAfterDeserializeInt(rootNode);
		}

		private static Tree GetTree(TreeNode n) {
			while (n.parent != null)
				n = n.parent;
			return n.treeIfRoot;
		}
		
		private static int GetTreeNodeChildIndex(TreeNode n) {
			TreeNode parent = n.parent;
			for (int i = 0; i < parent.children.Count; i++) {
				if (parent.children[i] == n)
					return i;
			}
			throw new Exception("Bad parent");
		}

		/// <summary>
		/// Determines is a node is a child of another. Returns false if the nodes are the same.
		/// </summary>
		public static bool TreeNodeIsChildOf(TreeNode potentialChild, TreeNode potentialParent) {
			for (var c = potentialChild.parent; ; c = c.parent) {
				if (c == potentialParent)
					return true;
				else if (c == null)
					return false;
			}
		}

		private List<int> GetTreeNodePath(TreeNode child, TreeNode parent) {
			var path = new List<int>();

			for (TreeNode n = child; n != parent; n = n.parent) {
				path.Insert(0, GetTreeNodeChildIndex(n));
				if (n.parent == null)
					throw new Exception("Nodes are not related");
			}

			return path;
		}
		
		public static TreeNode FollowTreeNodePath(TreeNode parent, int[] path) {
			TreeNode n = parent;
			for (int i = 0; i < path.Length; i++) {
				if (path[i] < 0 || path[i] >= n.children.Count)
					throw new Exception("Invalid path");
				n = n.children[path[i]];
			}
			return n;
		}

		public static TreeNode CreateTreeNode() {
			return new TreeNode();
		}

		public static void EnsureExpandedTo(TreeNode node) {
			for (var n = node.parent; n != null; n = n.parent)
				SetTreeNodeExpanded(n, true, false);
		}

		public static void SetTreeNodeText(TreeNode node, string text) {
			if (node.treeIfRoot != null)
				throw new Exception("Cannot change tree root node text");
			#if CLIENT
				Tree tree = GetTree(node);
				if (tree != null && tree.isAttached)
					tree.SetNodeTextDOM(node, text);
			#endif
			node.text = text;
		}

		public static void SetTreeNodeData(TreeNode node, object data) {
			node.data = data;
		}
		
		public static void SetTreeNodeIcon(TreeNode node, string icon) {
			if (node.treeIfRoot != null)
				throw new Exception("Cannot change tree root node text");
			#if CLIENT
				Tree tree = GetTree(node);
				if (tree != null && tree.isAttached)
					tree.SetNodeIconDOM(node, icon);
			#endif
			node.icon = icon;
		}

		public static void InsertTreeNodeChild(TreeNode toInsert, TreeNode parent, int position) {
			if (position < 0 || position > parent.children.Count) throw new Exception("Bad position");
			if (toInsert.parent != null) throw new Exception("Inserted node is not root.");
			Tree it = GetTree(toInsert), pt = GetTree(parent);
			if (it != null) throw new Exception("Node is already in a tree");
			#if CLIENT
				if (pt != null && pt.isAttached)
					pt.InsertTreeNodeDOM(parent, toInsert, position);
			#endif
			parent.children.Insert(position, toInsert);
			toInsert.parent = parent;
		}

		public static void AddTreeNodeChild(TreeNode toAdd, TreeNode parent) {
			InsertTreeNodeChild(toAdd, parent, parent.children.Count);
		}
		
		public static void RemoveTreeNode(TreeNode node) {
			if (node.parent == null)
				throw new Exception("Node is root");
			#if CLIENT
				Tree tree = GetTree(node);
				if (tree != null) {
					tree.RemoveTreeNodeDOM(node);
				}
			#endif
			node.parent.children.RemoveAt(GetTreeNodeChildIndex(node));
			node.parent = null;
		}
		
		private void ApplyCheckHierarchyToChildren(TreeNode node) {
			if (node.checkState != TreeNodeCheckState.indeterminate) {
				for (int i = 0; i < node.children.Count; i++) {
					TreeNode c   = node.children[i];
					c.checkState = node.checkState;
					ApplyCheckHierarchyToChildren(c);
					#if CLIENT
						if (isAttached)
							SetTreeNodeCheckStateDOM(c, c.checkState);
						OnNodeChecked(new TreeNodeEventArgs(c));
					#endif
				}
			}
		}
		
		private TreeNodeCheckState FindCheckStateFromChildren(TreeNode node) {
			bool hasChecked = false, hasUnchecked = false, hasIndeterminate = false;
			for (int i = 0; i < node.children.Count; i++) {
				TreeNode c = (TreeNode)node.children[i];
				switch (c.checkState) {
					case TreeNodeCheckState.yes:           hasChecked       = true; break;
					case TreeNodeCheckState.no:            hasUnchecked     = true; break;
					case TreeNodeCheckState.indeterminate: hasIndeterminate = true; break;
					default: throw new Exception("Invalid checkstate");
				}
			}
			if (hasIndeterminate || (hasChecked && hasUnchecked))
				return TreeNodeCheckState.indeterminate;
			else if (hasChecked)
				return TreeNodeCheckState.yes;
			else
				return TreeNodeCheckState.no;
		}
		
		private void ApplyCheckHierarchyToParents(TreeNode node) {
			for (TreeNode n = node.parent; n != null && n.treeIfRoot == null; n = n.parent) {
				n.checkState = FindCheckStateFromChildren(n);
				#if CLIENT
					if (isAttached)
						SetTreeNodeCheckStateDOM(n, n.checkState);
					OnNodeChecked(new TreeNodeEventArgs(n));
				#endif
			}
		}

		private void ApplyCheckHierarchy(TreeNode n) {
			ApplyCheckHierarchyToChildren(n);
			ApplyCheckHierarchyToParents(n);
		}

		public static void SetTreeNodeCheckState(TreeNode node, TreeNodeCheckState check) {
			Tree tree = GetTree(node);
			#if CLIENT
				if (tree != null && tree.isAttached && tree.hasChecks)
					tree.SetTreeNodeCheckStateDOM(node, check);
			#endif

			node.checkState = check;
			if (tree != null && tree.autoCheckHierarchy)
				tree.ApplyCheckHierarchy(node);

			#if CLIENT
				if (tree != null)
					tree.OnNodeChecked(new TreeNodeEventArgs(node));
			#endif
		}

		public static void SetTreeNodeExpanded(TreeNode node, bool expanded, bool applyToAllChildren) {
			if (node.children.Count > 0) {
				if (applyToAllChildren) {
					for (int i = 0; i < node.children.Count; i++)
						SetTreeNodeExpanded(node.children[i], expanded, true);
				}

				if (node.treeIfRoot == null) {
					#if CLIENT
						// Don't do this for the invisible root (it is always expanded).
						Tree tree = GetTree(node);
						if (tree != null && tree.isAttached)
							tree.DoSetTreeNodeExpanded(node, expanded, false);
						else
							node.expanded = expanded;
					#else
						node.expanded = expanded;
					#endif
				}
			}
			else {
				if (node.treeIfRoot == null)
					node.expanded = expanded;
			}
		}
		
		public static bool IsTreeNodeExpanded(TreeNode node) {
			return node.expanded;
		}
		
		public static TreeNodeCheckState GetTreeNodeCheckState(TreeNode node) {
			return node.checkState;
		}
		
		public static string GetTreeNodeText(TreeNode node) {
			return node.text;
		}
		
		public static string GetTreeNodeIcon(TreeNode node) {
			return node.icon;
		}
		
		public static object GetTreeNodeData(TreeNode node) {
			return node.data;
		}
		
		public static List<TreeNode> GetTreeNodeChildren(TreeNode node) {
			#if SERVER
				return node.children.ToList();
			#endif
			#if CLIENT
				return node.children.Clone();
			#endif
		}

		public static bool HasChildren(TreeNode node) {
			return node.children.Count > 0;
		}

		public static TreeNode GetTreeNodeParent(TreeNode node) {
			return node.parent;
		}
		
		private static void FindTreeNodesRecursive(TreeNode n, TreeNodeFindPredicate predicate, List<TreeNode> arr) {
			if (predicate(n))
				arr.Add(n);
			for (int i = 0; i < n.children.Count; i++)
				FindTreeNodesRecursive(n.children[i], predicate, arr);
		}
		
		public static List<TreeNode> FindTreeNodes(TreeNode root, TreeNodeFindPredicate predicate) {
			List<TreeNode> result = new List<TreeNode>();
			FindTreeNodesRecursive(root, predicate, result);
			return result;
		}
		
		#endregion
	}
}
