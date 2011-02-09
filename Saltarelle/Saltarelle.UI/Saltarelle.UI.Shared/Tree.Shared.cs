using System;
#if SERVER
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
#endif
#if CLIENT
using System.DHTML;
#endif

namespace Saltarelle.UI {
	public enum TreeNodeCheckState {
		no  = 0,
		yes = 1,
		indeterminate = 2
	}

	public delegate bool TreeNodeFindPredicate(ITreeNode node);

	/// <summary>
	/// Represents a tree node. Instances of this class are manipulated using static methods on the <see cref="Tree"/> class.
	/// Instances are created by the <see cref="Tree.CreateTreeNode"/> method.
	/// The reasons for this interface are:
	/// 1) Script# does not support methods on [Record] types, and
	/// 2) If we make the TreeNode class (whose members we don't want others to modify) public and its fields internal, Script# will minimize the field names, which is bad for JSON. [PreserveName] does not seem to work.
	/// </summary>
	public interface ITreeNode {
	}

	[Record]
	internal sealed class TreeNode
	#if SERVER
	: ITreeNode
	#endif
	{
		public int id;
		public bool expanded;
		public TreeNodeCheckState checkState;
		public string text;
		public string icon;
		public object data;
		public ArrayList children;

		#if SERVER
		[JsonIgnore]
		#endif
		public Tree treeIfRoot;
		
		#if SERVER
		[JsonIgnore]
		#endif
		public TreeNode parent;

		internal TreeNode() {
			this.id         = Tree.nextNodeId++;
			this.text       = null;
			this.data       = null;
			this.icon       = Tree.DefaultIcon;
			this.children   = new ArrayList();
			this.expanded   = false;
			this.checkState = TreeNodeCheckState.no;
			this.treeIfRoot = null;
			this.parent     = null;
		}
	}

#if CLIENT
	public class TreeSelectionChangingEventArgs : EventArgs {
		public bool Cancel;
		public ITreeNode NewSelection;
	}
	public delegate void TreeSelectionChangingEventHandler(object sender, TreeSelectionChangingEventArgs e);
	
	public class TreeNodeEventArgs : EventArgs {
		public ITreeNode Node;
		public TreeNodeEventArgs(ITreeNode node) {
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
		public ITreeNode DraggedNode;
		public ITreeNode NewParent;
		public int PositionWithinNewParent;

		public TreeDragDropCompletingEventArgs(ITreeNode draggedNode, ITreeNode newParent, int positionWithinNewParent) {
			this.DraggedNode             = draggedNode;
			this.NewParent               = newParent;
			this.PositionWithinNewParent = positionWithinNewParent;
			this.Cancel                  = false;
		}
	}
	public delegate void TreeDragDropCompletingEventHandler(object sender, TreeDragDropCompletingEventArgs e);

	public class TreeDragDropCompletedEventArgs : EventArgs {
		public ITreeNode DraggedNode;
		public ITreeNode NewParent;
		public int PositionWithinNewParent;

		public TreeDragDropCompletedEventArgs(ITreeNode draggedNode, ITreeNode newParent, int positionWithinNewParent) {
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

	#if SERVER
	[RequiresClientService(typeof(ISaltarelleUIService))]
	#endif
	public class Tree : IControl, IClientCreateControl, IResizableX, IResizableY {
		private static TreeNode N(ITreeNode i) { return (TreeNode)(object)i; }
		private static ITreeNode I(TreeNode n) { return (ITreeNode)(object)n; }

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

		public ITreeNode InvisibleRoot { get { return I(invisibleRoot); } }

		private string   id;
		private Position position = PositionHelper.NotPositioned;
		private int      width;
		private int      height;
		private int      tabIndex;
		private string   blankImageUrl;
		private bool     hasChecks;
		private bool     enabled;
		private TreeNode selectedNode;
		private bool     enableDragDrop;
		private bool     autoCheckHierarchy;

		#if CLIENT
			// Drag-drop fields
			private bool           isAttached;
			private int            itemHeight;
			private TreeDropTarget currentDropTarget;

			private JQueryEventHandlerDelegate dragFeedbackHandler;

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
						JQueryProxy.jQuery(GetElement()).width(value - HorzBorderSize);
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
						JQueryProxy.jQuery(GetElement()).height(value - VertBorderSize);
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
						JQueryProxy.jQuery(GetElement()).html(InnerHtml);
				#endif
			}
		}
		
		public bool Enabled {
			get { return enabled; }
			set {
				#if CLIENT
					if (isAttached && value != enabled) {
						DOMElement elem = GetElement();
						elem.TabIndex = enabled ? tabIndex : -1;
						elem.ClassName = TreeClasses + (value ? "" : (" " + DisabledTreeClass));
						if (value && enableDragDrop) {
							if (!Utils.IsNull(selectedNode))
								MakeDraggable(selectedNode, true);
							EnableDroppable(true);
						}
						else {
							if (!Utils.IsNull(selectedNode))
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
							if (!Utils.IsNull(selectedNode))
								MakeDraggable(selectedNode, true);
							EnableDroppable(true);
						}
						else {
							if (!Utils.IsNull(selectedNode))
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
		
		public ITreeNode SelectedNode {
			get {
				return I(selectedNode);
			}
			set {
				TreeNode n = N(value);
				if (!Utils.IsNull(value)) {
					Tree t = GetTree(n);
					if (t != this)
						throw new Exception("Node is not in tree");
				}
				SetSelection(n, true, true);
			}
		}
		
		private bool SetSelection(TreeNode newSelection, bool raiseSelectionChanging, bool raiseSelectionChanged) {
			if (!Utils.IsNull(newSelection))
				EnsureExpandedTo(I(newSelection));

			#if CLIENT
				if (raiseSelectionChanging) {
					if (!RaiseSelectionChanging(newSelection))
						return false;
				}

				if (isAttached) {
					if (!Utils.IsNull(selectedNode)) {
						// Remove the previous selection
						jQuery jq = JQueryProxy.jQuery(GetNodeElement(selectedNode).Children[0]);
						DOMElement d = jq.children("." + ItemTextClass).get(0);
						d.ClassName = ItemTextClass;
						if (enableDragDrop)
							MakeDraggable(selectedNode, false);
					}

					if (!Utils.IsNull(newSelection)) {
						jQuery jq = JQueryProxy.jQuery(GetNodeElement(newSelection).Children[0]);
						DOMElement d = jq.children("." + ItemTextClass).get(0);
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
		
		private void AppendNestedListHtml(ArrayList children, StringBuilder sb) {
			sb.Append("<div class=\"" + NestedListClass + "\"" + ">");
			for (int i = 0; i < Utils.ArrayLength(children); i++)
				AppendNodeHtml((TreeNode)children[i], sb);
			sb.Append("</div>");
		}

		private void AppendNodeHtml(TreeNode n, StringBuilder sb) {
			bool hasChildren = !Utils.IsNull(n.children) && Utils.ArrayLength(n.children) > 0;
			string suffix = (hasChildren ? (n.expanded ? ExpandedSuffix : CollapsedSuffix) : LeafSuffix);

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
			blankImageUrl = ((ISaltarelleUIService)GlobalServices.Provider.GetService(typeof(ISaltarelleUIService))).BlankImageUrl;
			selectedNode  = null;
			enabled       = true;
			width         = 300;
			height        = 300;
			tabIndex      = 0;
		}

#if SERVER
		public Tree() {
			GlobalServices.GetService<IScriptManagerService>().RegisterClientType(GetType());
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
			config["selectionPath"]      = (selectedNode != null ? GetTreeNodePath(I(selectedNode), I(invisibleRoot)) : null);
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
		public extern Tree();
		public Tree(object config) {
			dragFeedbackHandler = new JQueryEventHandlerDelegate(Element_DragFeedback);
			if (!Script.IsUndefined(config)) {
				InitConfig(Dictionary.GetDictionary(config));
			}
			else
				InitDefault();
		}
		
		protected virtual void InitConfig(Dictionary config) {
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
			blankImageUrl      = ((ISaltarelleUIService)GlobalServices.Provider.GetService(typeof(ISaltarelleUIService))).BlankImageUrl;

			FixTreeAfterDeserialize(I(invisibleRoot));
			invisibleRoot.treeIfRoot = this;
			invisibleRoot.parent     = null;
			
			int[] selectionPath = (int[])config["selectionPath"];
			selectedNode = N(!Utils.IsNull(selectionPath) ? FollowTreeNodePath(I(invisibleRoot), selectionPath) : null);

			Attach();
		}
		
		private DOMElement GetNodeElement(TreeNode node) { return Document.GetElementById(NodeIdPrefix + Utils.ToStringInvariantInt(node.id)); }
		
		public DOMElement GetElement() { return isAttached ? Document.GetElementById(id) : null; }

		public void Attach() {
			if (Utils.IsNull(id) || isAttached)
				throw new Exception("Must set ID and can only attach once");
			isAttached = true;
			DOMElement elem = GetElement();

			UIUtils.AttachKeyPressHandler(elem, Element_KeyPress);
			JQueryProxy.jQuery(elem).click(Element_Click);
			if (!Utils.IsNull(selectedNode))
				EnsureVisible(selectedNode);
			
			if (enableDragDrop && enabled) {
				if (!Utils.IsNull(selectedNode))
					MakeDraggable(selectedNode, true);
				EnableDroppable(true);
			}
		}
		
		private void EnsureVisible(TreeNode n) {
			DOMElement treeEl = GetElement(), nodeEl = GetNodeElement(n);
			jQuery treeJq = JQueryProxy.jQuery(treeEl), nodeJq = JQueryProxy.jQuery(nodeEl);
			double offsetTop = nodeJq.offset().top - treeJq.offset().top, scrollTop = treeJq.scrollTop(), nHeight = nodeJq.children(":eq(0)").outerHeight(), treeHeight = treeEl.ClientHeight;

			if (offsetTop < 0) {
				treeJq.scrollTop(Math.Round(scrollTop + offsetTop));
			}
			else if (offsetTop + nHeight > treeHeight) {
				treeJq.scrollTop(Math.Round(scrollTop + offsetTop + nHeight - treeHeight));
			}
		}
		
		private void MakeDraggable(TreeNode node, bool enable) {
			jQuery el = JQueryProxy.jQuery(GetNodeElement(node).Children[0]).children("." + ItemTextClass);
			if (enable) {
				el.draggable(new Dictionary("helper", "clone",
				                            "appendTo", JQueryProxy.jQuery(GetElement()),
				                            "scroll", true,
				                            "containment", "parent"));
			}
			else
				el.draggable("destroy");
		}

		private void Element_DragFeedback(JQueryEvent evt) {
			DOMElement elem = GetElement();

			int elemTop = (int)JQueryProxy.jQuery(elem).offset().top;
			int offset = evt.pageY - elemTop + elem.ScrollTop;

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
						for (int i = 0; i < n.children.Length; i++) {
							TreeNode x = countDown((TreeNode)n.children[i]);
							if (!Utils.IsNull(x))
								return x;
						}
					}
					return null;
				};
				newTarget = countDown(invisibleRoot);
			}
			else
				newTarget = null;

			if (!Utils.IsNull(newTarget)) {
				// Determine if we try to drop into the current dragging node or a child of it.
				for (TreeNode n = newTarget; !Utils.IsNull(n); n = n.parent) {
					if (n == selectedNode) {
						newTarget = null;
						break;
					}
				}
			}

			ChangeDropTarget(!Utils.IsNull(newTarget) ? new TreeDropTarget(newTarget, dropAbove) : null);
		}

		private void Element_Drop(JQueryEvent evt, DroppableEventObject ui) {
			if (Utils.IsNull(currentDropTarget)) {
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
				dropIndex  = dropParent.children.Length;
			}

			TreeDragDropCompletingEventArgs completingArgs = new TreeDragDropCompletingEventArgs(I(draggedNode), I(dropParent), dropIndex);
			OnDragDropCompleting(completingArgs);
			if (completingArgs.Cancel) {
				DragEnded();
				return;
			}

			SetSelection(null, false, false);	// Temporarily remove the selection.
			
			// Remove the node from its current position.
			DOMElement draggedElem = RemoveTreeNodeDOM(draggedNode);
			draggedNode.parent.children.RemoveAt(GetTreeNodeChildIndex(draggedNode));
			if (draggedNode.parent.children.Length == 0)
				UpdateExpansionClasses(GetNodeElement(draggedNode.parent), draggedNode.parent.icon, false, draggedNode.parent.expanded);	// Need to fix the classes to say that this node is now a leaf.

			// Add the node to its new position.
			if (Utils.IsNull(dropParent.treeIfRoot)) {
				DoSetTreeNodeExpanded(dropParent, true, true);
				DOMElement dropParentN = GetNodeElement(dropParent);
				if (dropIndex == dropParentN.Children[1].Children.Length)
					dropParentN.Children[1].AppendChild(draggedElem);
				else
					dropParentN.Children[1].InsertBefore(draggedElem, dropParentN.Children[1].Children[dropIndex]);
			}
			else {
				DOMElement elem = GetElement();
				if (dropIndex == elem.Children.Length)
					elem.AppendChild(draggedElem);
				else
					elem.InsertBefore(draggedElem, elem.Children[dropIndex]);
			}

			draggedNode.parent = dropParent;
			dropParent.children.Insert(dropIndex, draggedNode);
			
			// Restore the selection to the new node.
			SetSelection(draggedNode, false, false);

			TreeDragDropCompletedEventArgs completedArgs = new TreeDragDropCompletedEventArgs(I(draggedNode), I(dropParent), dropIndex);
			OnDragDropCompleted(completedArgs);

			DragEnded();
		}

		private void ChangeDropTarget(TreeDropTarget newTarget) {
			if ((Utils.IsNull(newTarget) && Utils.IsNull(currentDropTarget)) || (!Utils.IsNull(newTarget) && !Utils.IsNull(currentDropTarget) && newTarget.node == currentDropTarget.node && newTarget.above == currentDropTarget.above))
				return;
			if (!Utils.IsNull(currentDropTarget))
				JQueryProxy.jQuery(GetNodeElement(currentDropTarget.node).Children[0]).removeClass(DropIntoClass).removeClass(DropAboveClass);
			if (!Utils.IsNull(newTarget))
				JQueryProxy.jQuery(GetNodeElement(newTarget.node).Children[0]).addClass(newTarget.above ? DropAboveClass : DropIntoClass);
			currentDropTarget = newTarget;
		}
		
		private void DragEnded() {
			ChangeDropTarget(null);
			JQueryProxy.jQuery(Window.Document).unbind("mousemove", dragFeedbackHandler);
		}

		private void EnableDroppable(bool enable) {
			jQuery el = JQueryProxy.jQuery(GetElement());
			if (enable) {
				el.droppable(new Dictionary("tolerance", "pointer",
				                            "greedy",    true,
				                            "over",      (Callback)delegate() {
				                                             itemHeight = (invisibleRoot.children.Length > 0 ? (int)JQueryProxy.jQuery(GetNodeElement((TreeNode)invisibleRoot.children[0]).Children[0]).outerHeight() : 1);
				                                             currentDropTarget = null;
				                                             JQueryProxy.jQuery(Window.Document).mousemove(dragFeedbackHandler);
				                                         },
				                            "out",       (Callback)DragEnded, 
				                            "drop",      new DroppableEventHandlerDelegate(Element_Drop)));
			}
			else
				el.droppable("destroy");
		}
		
		private TreeNode FindTreeNode(DOMElement nodeElem) {
			// Ugly (but works). We need to find the node, which is done by investigating the ID and walking the tree from the root down.
			// It works, though, and it performs well even on IE6.
			string idStr = nodeElem.ID;
			int id = Utils.ParseInt(Utils.Substring(idStr, NodeIdPrefix.Length, idStr.Length - NodeIdPrefix.Length));
			ITreeNode[] result = FindTreeNodes(I(invisibleRoot), delegate(ITreeNode n) { return N(n).id == id; });
			return result.Length > 0 ? N(result[0]) : null;
		}
		
		private void Element_Click(JQueryEvent evt) {
			if (!enabled)
				return;

			DOMElement elem = GetElement();
			for (DOMElement target = evt.target; target != elem; target = target.ParentNode) {
				string cls = " " + target.ClassName + " ";
				if (target.TagName.ToLowerCase() == "input") {
					TreeNode n = FindTreeNode(target.ParentNode.ParentNode);
					n.checkState = ((CheckBoxElement)target).Checked ? TreeNodeCheckState.yes : TreeNodeCheckState.no;
					Type.SetField(target, "defaultChecked", ((CheckBoxElement)target).Checked);
					if (autoCheckHierarchy)
						ApplyCheckHierarchy(n);
					OnNodeChecked(new TreeNodeEventArgs(I(n)));
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
		
		private void Element_KeyPress(JQueryEvent e) {
			if (!RaiseKeyPress(e.keyCode)) {
				e.preventDefault();
				return;
			}

			switch (e.keyCode) {
				case 32: {
					// Space - used to toggle checkmark if there is one.
					if (hasChecks) {
						if (selectedNode != null) {
							CheckBoxElement cb = (CheckBoxElement)JQueryProxy.jQuery(GetNodeElement(selectedNode).Children[0]).find("input").get(0);
							SetTreeNodeCheckState(I(selectedNode), selectedNode.checkState == TreeNodeCheckState.yes ? TreeNodeCheckState.no : TreeNodeCheckState.yes);
						}
						e.preventDefault();
					}
					break;
				}
			
				case 37: {
					// key left - if current node exists and is expanded: collapse it, otherwise navigate to its parent
					if (!Utils.IsNull(selectedNode)) {
						if (selectedNode.children.Length > 0 && selectedNode.expanded)
							DoSetTreeNodeExpanded(selectedNode, false, false);
						else if (selectedNode.parent != invisibleRoot)
							SetSelection(selectedNode.parent, true, true);
					}
					else if (invisibleRoot.children.Length > 0)
						SetSelection((TreeNode)invisibleRoot.children[0], true, true);
					e.preventDefault();
					break;
				}

				case 38:
					// key up - navigate to the parent or to the most expanded node in the tree of the previous sibling
					if (!Utils.IsNull(selectedNode)) {
						int index = GetTreeNodeChildIndex(selectedNode);
						if (index == 0) {
							if (selectedNode.parent != invisibleRoot)
								SetSelection(selectedNode.parent, true, true);
						}
						else {
							TreeNode n = (TreeNode)selectedNode.parent.children[index - 1];
							while (n.children.Length > 0 && n.expanded)
								n = (TreeNode)n.children[n.children.Length - 1];
							SetSelection(n, true, true);
						}
					}
					else if (invisibleRoot.children.Length > 0)
						SetSelection((TreeNode)invisibleRoot.children[0], true, true);

					e.preventDefault();
					break;
					
				case 39:
					// key right - if current node has children: expand if collapsed, navigate to first child if expanded
					if (!Utils.IsNull(selectedNode)) {
						if (selectedNode.children.Length > 0) {
							if (selectedNode.expanded) {
								SetSelection((TreeNode)selectedNode.children[0], true, true);
							}
							else {
								DoSetTreeNodeExpanded(selectedNode, true, false);
							}
						}
					}
					else if (invisibleRoot.children.Length > 0)
						SetSelection((TreeNode)invisibleRoot.children[0], true, true);

					e.preventDefault();
					break;
					
				case 40: {
					// key down - navigate to the first child if the selected node is expanded, otherwise navigate to the next sibling of the closest node which has a next sibling.
					if (!Utils.IsNull(selectedNode)) {
						if (selectedNode.children.Length > 0 && selectedNode.expanded) {
							SetSelection((TreeNode)selectedNode.children[0], true, true);
						}
						else {
							TreeNode n = selectedNode;
							for (;;) {
								if (Utils.IsNull(n.parent))
									break;	// Obviously we are already at the last position.
								int index = GetTreeNodeChildIndex(n);
								if (index < n.parent.children.Length - 1) {
									SetSelection((TreeNode)n.parent.children[index + 1], true, true);
									break;
								}
								n = n.parent;
							}
						}
					}
					else if (invisibleRoot.children.Length > 0)
						SetSelection((TreeNode)invisibleRoot.children[0], true, true);

					e.preventDefault();
					break;
				}
			}
		}
		
		/// <summary>
		/// Removes a node from the tree, which must be attached. Does not fix the node's parent's child list. Returns the removed element, or null if there was no physical node (eg. if the node has never been expanded to).
		/// </summary>
		private DOMElement RemoveTreeNodeDOM(TreeNode node) {
			if (!Utils.IsNull(selectedNode) && (selectedNode == node || TreeNodeIsChildOf(I(selectedNode), I(node)))) {
				TreeNode newSelection;

				int childIndex = GetTreeNodeChildIndex(node);
				if (childIndex < node.parent.children.Length - 1)
					newSelection = (TreeNode)node.parent.children[childIndex + 1];
				else if (node.parent.children.Length > 1)
					newSelection = (TreeNode)node.parent.children[node.parent.children.Length - 2];	// - 2 because our caller will remove the last node.
				else if (Utils.IsNull(node.parent.treeIfRoot))
					newSelection = node.parent;
				else
					newSelection = null;

				SetSelection(newSelection, false, true);
			}
		
			DOMElement elem = GetNodeElement(node);
			if (Utils.IsNull(elem))
				return null;
			DOMElement list = elem.ParentNode;
			elem.ParentNode.RemoveChild(elem);
			if (node.parent.children.Length == 1 && Utils.IsNull(node.parent.treeIfRoot))	// In case we are removing the last child, also remove the child list.
				list.ParentNode.RemoveChild(list);
			return elem;
		}
		
		private void SetNodeTextDOM(TreeNode node, string text) {
			DOMElement elem = GetNodeElement(node);
			if (!Utils.IsNull(elem))
				JQueryProxy.jQuery(elem.Children[0]).children("." + ItemTextClass).text(text);
		}

		private void SetNodeIconDOM(TreeNode node, string text) {
			DOMElement elem = GetNodeElement(node);
			if (!Utils.IsNull(elem)) {
				string suffix = (node.children.Length > 0 ? (node.expanded ? ExpandedSuffix : CollapsedSuffix) : LeafSuffix);
				DOMElement iconEl = JQueryProxy.jQuery(elem.Children[0]).children("." + IconClass).get(0);
				iconEl.ClassName = (IconClass + " " + IconClass + suffix + " " + node.icon + " " + node.icon + suffix);
			}
		}
		
		/// <summary>
		/// Inserts a node into the tree, which must be attached. The parent's child list should not include the new node, and the list is fixed by this method. Returns the inserted element, or null if there was no physical node (eg. if the parent has never been expanded to).
		/// </summary>
		private DOMElement InsertTreeNodeDOM(TreeNode parent, TreeNode toInsert, int position) {
			DOMElement result = null;
			if (!Utils.IsNull(parent.treeIfRoot)) {
				// Modifying the root.
				DOMElement elem = GetElement();
				StringBuilder sb = new StringBuilder();
				AppendNodeHtml((TreeNode)toInsert, sb);
				result = JQueryProxy.jQuery(sb.ToString()).get(0);
				if (position == parent.children.Length)
					elem.AppendChild(result);
				else
					elem.InsertBefore(result, elem.Children[position]);
			}
			else {
				// Not inserting at the root.
				DOMElement parentEl = GetNodeElement(parent);
				if (Utils.IsNull(parentEl))
					return null;

				if (parentEl.Children.Length > 1) {
					// We need to insert the new element into the parent's child list.
					DOMElement listEl = parentEl.Children[1];
					StringBuilder sb = new StringBuilder();
					AppendNodeHtml((TreeNode)toInsert, sb);
					result = JQueryProxy.jQuery(sb.ToString()).get(0);
					if (position == parent.children.Length)
						listEl.AppendChild(result);
					else
						listEl.InsertBefore(result, listEl.Children[position]);
				}

				if (parent.children.Length == 0)
					UpdateExpansionClasses(parentEl, parent.icon, true, parent.expanded);	// This was the first child we added.
			}

			return result;
		}
		
		private void SetTreeNodeCheckStateDOM(TreeNode node, TreeNodeCheckState checkState) {
			DOMElement nodeElem = GetNodeElement(node);
			if (nodeElem != null) {
				CheckBoxElement cb = (CheckBoxElement)JQueryProxy.jQuery(nodeElem.Children[0]).children("input").get(0);
				Type.SetField(cb, "indeterminate", checkState == TreeNodeCheckState.indeterminate);
				cb.Checked = (checkState == TreeNodeCheckState.yes);
				Type.SetField(cb, "defaultChecked", cb.Checked);
			}
		}
		
		private void UpdateExpansionClasses(DOMElement nodeElem, string icon, bool hasChildren, bool expanded) {
			string suffix = (hasChildren ? (expanded ? ExpandedSuffix : CollapsedSuffix) : LeafSuffix);
			nodeElem.ClassName = ContainerClass + " " + ContainerClass + suffix;
			nodeElem.Children[0].ClassName = NodeClass + " " + NodeClass + suffix;
			nodeElem.Children[0].Children[0].ClassName = ExpandCollapseClass + " " + ExpandCollapseClass + suffix;
			nodeElem.Children[0].Children[1].ClassName = IconClass + " " + IconClass + suffix + " " + icon + " " + icon + suffix;
		}
		
		private void DoSetTreeNodeExpanded(TreeNode node, bool expanded, bool doItEvenIfNoChildren) {
			node.expanded = expanded;
			if (doItEvenIfNoChildren || node.children.Length > 0) {
				DOMElement elem = GetNodeElement(node);
				if (!Utils.IsNull(elem)) {
					if (elem.Children.Length > 1) {
						// The list exists - update its display state.
						elem.Children[1].Style.Display = expanded ? "" : "none";
					}
					else {
						if (expanded) {
							// Expanding and the list does not exist - add it
							StringBuilder sb = new StringBuilder();
							AppendNestedListHtml(node.children, sb);
							JQueryProxy.jQuery(sb.ToString()).appendTo(JQueryProxy.jQuery(elem));
						}
					}

					UpdateExpansionClasses(elem, node.icon, node.children.Length > 0, expanded);
				}
			}

			if (!Utils.IsNull(selectedNode) && !expanded && TreeNodeIsChildOf(I(selectedNode), I(node))) {
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
			TreeSelectionChangingEventArgs e = new TreeSelectionChangingEventArgs();
			e.Cancel = false;
			e.NewSelection = I(newSelection);
			OnSelectionChanging(e);
			return !e.Cancel;
		}
		
		private bool RaiseKeyPress(int keyCode) {
			TreeKeyPressEventArgs e = new TreeKeyPressEventArgs();
			e.KeyCode = keyCode;
			OnKeyPress(e);
			return !e.PreventDefault;
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
		#endregion
#endif

		#region TreeNode manipulators

		private static void FixTreeAfterDeserializeInt(TreeNode n) {
			for (int i = 0; i < Utils.ArrayLength(n.children); i++) {
				FixTreeAfterDeserializeInt((TreeNode)n.children[i]);
				((TreeNode)n.children[i]).parent = n;
			}
			n.treeIfRoot = null;
		}

		public static void FixTreeAfterDeserialize(ITreeNode rootNode) {
			FixTreeAfterDeserializeInt(N(rootNode));
		}

		private static Tree GetTree(TreeNode n) {
			while (!Utils.IsNull(n.parent))
				n = n.parent;
			return n.treeIfRoot;
		}
		
		private static int GetTreeNodeChildIndex(TreeNode n) {
			TreeNode parent = n.parent;
			for (int i = 0; i < Utils.ArrayLength(parent.children); i++) {
				if (parent.children[i] == n)
					return i;
			}
			throw new Exception("Bad parent");
		}

		/// <summary>
		/// Determines is a node is a child of another. Returns false if the nodes are the same.
		/// </summary>
		public static bool TreeNodeIsChildOf(ITreeNode potentialChild, ITreeNode potentialParent) {
			TreeNode p = N(potentialParent);
			for (TreeNode c = N(potentialChild).parent; ; c = c.parent) {
				if (c == p)
					return true;
				else if (Utils.IsNull(c))
					return false;
			}
		}

		private int[] GetTreeNodePath(ITreeNode child, ITreeNode parent) {
			#if SERVER
			List<int> path = new List<int>();
			#else
			ArrayList path = new ArrayList();
			#endif

			TreeNode parentN = N(parent);
			for (TreeNode n = N(child); n != parentN; n = n.parent) {
				path.Insert(0, GetTreeNodeChildIndex(n));
				if (Utils.IsNull(n.parent))
					throw new Exception("Nodes are not related");
			}
			
			#if SERVER
			return path.ToArray();
			#else
			return (int[])path;
			#endif
		}
		
		public static ITreeNode FollowTreeNodePath(ITreeNode parent, int[] path) {
			TreeNode n = N(parent);
			for (int i = 0; i < path.Length; i++) {
				if (path[i] < 0 || path[i] >= Utils.ArrayLength(n.children))
					throw new Exception("Invalid path");
				n = (TreeNode)n.children[path[i]];
			}
			return I(n);
		}

		public static ITreeNode CreateTreeNode() {
			return I(new TreeNode());
		}

		public static void EnsureExpandedTo(ITreeNode node) {
			TreeNode n = N(node);
			for (n = n.parent; !Utils.IsNull(n); n = n.parent)
				SetTreeNodeExpanded(I(n), true, false);
		}

		public static void SetTreeNodeText(ITreeNode node, string text) {
			TreeNode n = N(node);
			if (!Utils.IsNull(n.treeIfRoot))
				throw new Exception("Cannot change tree root node text");
			#if CLIENT
				Tree tree = GetTree(n);
				if (!Utils.IsNull(tree) && tree.isAttached)
					tree.SetNodeTextDOM(n, text);
			#endif
			n.text = text;
		}

		public static void SetTreeNodeData(ITreeNode node, object data) {
			TreeNode n = N(node);
			n.data = data;
		}
		
		public static void SetTreeNodeIcon(ITreeNode node, string icon) {
			TreeNode n = N(node);
			if (!Utils.IsNull(n.treeIfRoot))
				throw new Exception("Cannot change tree root node text");
			#if CLIENT
				Tree tree = GetTree(n);
				if (!Utils.IsNull(tree) && tree.isAttached)
					tree.SetNodeIconDOM(n, icon);
			#endif
			n.icon = icon;
		}

		public static void InsertTreeNodeChild(ITreeNode toInsert, ITreeNode parent, int position) {
			TreeNode toInsertN = N(toInsert), parentN = N(parent);

			if (position < 0 || position > Utils.ArrayLength(parentN.children)) throw new Exception("Bad position");
			if (!Utils.IsNull(toInsertN.parent)) throw new Exception("Inserted node is not root.");
			Tree it = GetTree(toInsertN), pt = GetTree(parentN);
			if (!Utils.IsNull(it)) throw new Exception("Node is already in a tree");
			#if CLIENT
				if (!Utils.IsNull(pt) && pt.isAttached)
					pt.InsertTreeNodeDOM(parentN, toInsertN, position);
			#endif
			parentN.children.Insert(position, toInsert);
			toInsertN.parent = parentN;
		}

		public static void AddTreeNodeChild(ITreeNode toAdd, ITreeNode parent) {
			InsertTreeNodeChild(toAdd, parent, Utils.ArrayLength(N(parent).children));
		}
		
		public static void RemoveTreeNode(ITreeNode node) {
			TreeNode n = N(node);
			if (Utils.IsNull(n.parent))
				throw new Exception("Node is root");
			#if CLIENT
				Tree tree = GetTree(n);
				if (!Utils.IsNull(tree)) {
					tree.RemoveTreeNodeDOM(n);
				}
			#endif
			n.parent.children.RemoveAt(GetTreeNodeChildIndex(n));
			n.parent = null;
		}
		
		private void ApplyCheckHierarchyToChildren(TreeNode node) {
			if (node.checkState != TreeNodeCheckState.indeterminate) {
				for (int i = 0; i < Utils.ArrayLength(node.children); i++) {
					TreeNode c   = (TreeNode)node.children[i];
					c.checkState = node.checkState;
					ApplyCheckHierarchyToChildren(c);
					#if CLIENT
						if (isAttached)
							SetTreeNodeCheckStateDOM(c, c.checkState);
						OnNodeChecked(new TreeNodeEventArgs(I(c)));
					#endif
				}
			}
		}
		
		private TreeNodeCheckState FindCheckStateFromChildren(TreeNode node) {
			bool hasChecked = false, hasUnchecked = false, hasIndeterminate = false;
			for (int i = 0; i < Utils.ArrayLength(node.children); i++) {
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
			for (TreeNode n = node.parent; !Utils.IsNull(n) && Utils.IsNull(n.treeIfRoot); n = n.parent) {
				n.checkState = FindCheckStateFromChildren(n);
				#if CLIENT
					if (isAttached)
						SetTreeNodeCheckStateDOM(n, n.checkState);
					OnNodeChecked(new TreeNodeEventArgs(I(n)));
				#endif
			}
		}

		private void ApplyCheckHierarchy(TreeNode n) {
			ApplyCheckHierarchyToChildren(n);
			ApplyCheckHierarchyToParents(n);
		}

		public static void SetTreeNodeCheckState(ITreeNode node, TreeNodeCheckState check) {
			TreeNode n = N(node);
			Tree tree = GetTree(n);
			#if CLIENT
				if (!Utils.IsNull(tree) && tree.isAttached && tree.hasChecks)
					tree.SetTreeNodeCheckStateDOM(n, check);
			#endif

			n.checkState = check;
			if (tree != null && tree.autoCheckHierarchy)
				tree.ApplyCheckHierarchy(n);

			#if CLIENT
				if (!Utils.IsNull(tree))
					tree.OnNodeChecked(new TreeNodeEventArgs(node));
			#endif
		}

		public static void SetTreeNodeExpanded(ITreeNode node, bool expanded, bool applyToAllChildren) {
			TreeNode n = N(node);
			if (Utils.ArrayLength(n.children) > 0) {
				if (applyToAllChildren) {
					for (int i = 0; i < Utils.ArrayLength(n.children); i++)
						SetTreeNodeExpanded(I((TreeNode)n.children[i]), expanded, true);
				}

				if (Utils.IsNull(n.treeIfRoot)) {
					#if CLIENT
						// Don't do this for the invisible root (it is always expanded).
						Tree tree = GetTree(n);
						if (!Utils.IsNull(tree) && tree.isAttached)
							tree.DoSetTreeNodeExpanded(n, expanded, false);
						else
							n.expanded = expanded;
					#else
						n.expanded = expanded;
					#endif
				}
			}
			else {
				if (Utils.IsNull(n.treeIfRoot))
					n.expanded = expanded;
			}
		}
		
		public static bool IsTreeNodeExpanded(ITreeNode node) {
			return N(node).expanded;
		}
		
		public static TreeNodeCheckState GetTreeNodeCheckState(ITreeNode node) {
			return N(node).checkState;
		}
		
		public static string GetTreeNodeText(ITreeNode node) {
			return N(node).text;
		}
		
		public static string GetTreeNodeIcon(ITreeNode node) {
			return N(node).icon;
		}
		
		public static object GetTreeNodeData(ITreeNode node) {
			return N(node).data;
		}
		
		public static ITreeNode[] GetTreeNodeChildren(ITreeNode node) {
			#if SERVER
				return (ITreeNode[])N(node).children.ToArray(typeof(ITreeNode));
			#endif
			#if CLIENT
				return (ITreeNode[])N(node).children.Clone();
			#endif
		}

		public static bool HasChildren(ITreeNode node) {
			return Utils.ArrayLength(N(node).children) > 0;
		}

		public static ITreeNode GetTreeNodeParent(ITreeNode node) {
			return I(N(node).parent);
		}
		
		private static void FindTreeNodesRecursive(TreeNode n, TreeNodeFindPredicate predicate, ArrayList arr) {
			if (predicate(I(n)))
				arr.Add(n);
			for (int i = 0; i < Utils.ArrayLength(n.children); i++)
				FindTreeNodesRecursive((TreeNode)n.children[i], predicate, arr);
		}
		
		public static ITreeNode[] FindTreeNodes(ITreeNode root, TreeNodeFindPredicate predicate) {
			ArrayList result = new ArrayList();
			FindTreeNodesRecursive(N(root), predicate, result);
			#if SERVER
				return (ITreeNode[])result.ToArray(typeof(ITreeNode));
			#endif
			#if CLIENT
				return (ITreeNode[])result;
			#endif
		}
		
		#endregion
	}
}
