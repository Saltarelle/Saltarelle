<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="Saltarelle" %>
<%@ Import Namespace="Saltarelle.UI" %>
<%@ Import Namespace="Saltarelle.Mvc" %>
<%@ Import Namespace="DemoWeb" %>
<%@ Import Namespace="System.Globalization" %>
<script runat="server">
private Tree[] controls;

private const int NumControls = 4;

private int nodeData = 1;

private Saltarelle.UI.TreeNode CreateTreeNode(int numChildLevels) {
	var n = Tree.CreateTreeNode();
	Tree.SetTreeNodeText(n, "Node " + nodeData.ToString(CultureInfo.InvariantCulture));
	Tree.SetTreeNodeData(n, nodeData);
	Tree.SetTreeNodeExpanded(n, (nodeData % 3) == 1, false);
	nodeData++;
	if (numChildLevels > 0) {
		for (int i = 0; i < 4; i++) {
			Tree.AddTreeNodeChild(CreateTreeNode(numChildLevels - 1), n);
		}
	}
	return n;
}

protected override void OnLoad(EventArgs e) {
	controls = new Tree[NumControls];
	for (int i = 0; i < controls.Length; i++) {
		controls[i] = new Tree();
		controls[i].Id = "control" + i.ToString(CultureInfo.InvariantCulture);
		for (int j = 0; j <= i; j++)
			Tree.AddTreeNodeChild(CreateTreeNode(4), controls[i].InvisibleRoot);
	}

	controls[0].SelectedNode = Tree.FollowTreeNodePath(controls[0].InvisibleRoot, new[] { 0, 3, 1 });
	controls[1].SelectedNode = Tree.FollowTreeNodePath(controls[1].InvisibleRoot, new[] { 1, 3, 1, 1 });
	controls[2].SelectedNode = Tree.FollowTreeNodePath(controls[2].InvisibleRoot, new[] { 0 });
	
	controls[1].HasChecks          = true;
	controls[1].AutoCheckHierarchy = true;
	controls[2].Enabled            = false;
	controls[3].EnableDragDrop     = true;
}
</script>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>Tree Tests</title>
	<link rel="Stylesheet" href="/Content/themes/base/ui.all.css" type="text/css"/>
	<link rel="Stylesheet" href="/Content/site.css" type="text/css"/>
	<link rel="Stylesheet" href="/Stylesheet" type="text/css"/>
<% Html.Scripts(); %>
<style type="text/css">
.MessageLog span {
	display: block;
}
</style>
<script type="text/javascript" language="javascript">
function follow(treeIndex, path) {
	return Saltarelle.UI.Tree.followTreeNodePath(controls[treeIndex].get_invisibleRoot(), path);
}

function tn(text, data) {
	var n = Saltarelle.UI.Tree.createTreeNode();
	Saltarelle.UI.Tree.setTreeNodeText(n, text);
	Saltarelle.UI.Tree.setTreeNodeData(n, data);
	return n;
}

function logEvent(tree, msg) {
	$('<div/>').text(tree.get_id() + ': ' + msg).appendTo($('#events'));
}

function getData(node) {
	return (node !== null ? Saltarelle.UI.Tree.getTreeNodeData(node) : 'null');
}

function selectionChangingEventHandler(s, e) {
	logEvent(s, 'SelectionChanging, old = ' + getData(s.get_selectedNode()) + ', new = ' + getData(e.newSelection));
}

function selectionChangedEventHandler(s, e) {
	logEvent(s, 'SelectionChanged, new = ' + getData(s.get_selectedNode()));
}

function dragDropCompletingEventHandler(s, e) {
	logEvent(s, 'DragDropCompleting, item = ' + getData(e.draggedNode) + ', new parent = ' + getData(e.newParent) + ', index = ' + e.positionWithinNewParent);
}

function dragDropCompletedEventHandler(s, e) {
	logEvent(s, 'DragDropCompleted, item = ' + getData(e.draggedNode) + ', new parent = ' + getData(e.newParent) + ', index = ' + e.positionWithinNewParent);
}

function keyPressEventHandler(s, e) {
	logEvent(s, 'KeyPress, code = ' + e.keyCode);
}

function nodeCheckedEventHandler(s, e) {
	logEvent(s, 'NodeChecked, node = ' + getData(e.node) + ', checked = ' + Enum.toString(Saltarelle.UI.TreeNodeCheckState, Saltarelle.UI.Tree.getTreeNodeCheckState(e.node)));
}

var controls;
function init() {
	var cfg = <%= Utils.InitScript(controls.Select(c => c.ConfigObject)) %>;
	controls = new Array(cfg.length);

	for (var i = 0; i < controls.length; i++) {
		var c = new Saltarelle.UI.Tree(cfg[i]);
		c.add_selectionChanging(selectionChangingEventHandler);
		c.add_selectionChanged(selectionChangedEventHandler);
		c.add_keyPress(keyPressEventHandler);
		c.add_dragDropCompleting(dragDropCompletingEventHandler);
		c.add_dragDropCompleted(dragDropCompletedEventHandler);
		c.add_nodeChecked(nodeCheckedEventHandler);
		controls[i] = c;
	}
	
	$('#evalScriptButton').click(function() {
		eval($('#scriptInput').val());
	});
	
	$('#clearEventsButton').click(function() {
		$('#events').html('');
	});
}
</script>
</head>

<body style="margin: 20px">
	<table>
		<tr>
			<td>
				<%= controls[0].Html %>
			</td>
			<td>
				<%= controls[1].Html %>
			</td>
			<td rowspan="2">
				<div>
					<input type="text" style="width: 300px" id="scriptInput"/><button type="button" id="evalScriptButton">Eval</button>
				</div>
				<button type="button" id="clearEventsButton">Clear</button>
				<div style="width: 400px; height: 600px" id="events">
				</div>
			</td>
		</tr>
		<tr>
			<td>
				<%= controls[2].Html %>
			</td>
			<td>
				<%= controls[3].Html %>
			</td>
		</tr>
	</table>
</body>
</html>
