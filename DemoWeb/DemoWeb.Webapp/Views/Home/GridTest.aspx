<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="Saltarelle" %>
<%@ Import Namespace="Saltarelle.UI" %>
<%@ Import Namespace="Saltarelle.Mvc" %>
<%@ Import Namespace="DemoWeb" %>
<%@ Import Namespace="System.Globalization" %>

<script runat="server">
private Grid[] controls;

private const int NumGrids = 3;
private const int NumRows = 100;

protected override void OnLoad(EventArgs e) {
	base.OnLoad(e);
	
	controls = new Grid[NumGrids];
	for (int i = 0; i < NumGrids; i++) {
		controls[i] = new Grid() { Id = "grid" + i.ToString(CultureInfo.InvariantCulture), ColTitles = new string[] { "Co1 1", "Co1 2", "Co1 3", "Co1 4" }, Width = 420, Height = 150 };
		for (int j = 0; j < NumRows; j++) {
			string prefix = "Cell " + j.ToString(CultureInfo.InvariantCulture) + ", ";
			controls[i].AddItem(new[] { prefix + "1", prefix + "2", prefix + "3", prefix + "4" }, j);
		}
	}
	
	controls[1].ColHeadersVisible = false;
	controls[2].EnableDragDrop    = true;
}
</script>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>Grid Tests</title>
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
var controls;

function logEvent(grid, msg) {
	$('<div/>').text(grid.get_id() + ': ' + msg).appendTo($('#events'));
}

function keyPressEventHandler(s, e) {
	logEvent(s, 'KeyPress, code = ' + e.keyCode);
}

function selectionChangingEventHandler(s, e) {
	logEvent(s, 'SelectionChanging, old = ' + s.getData(e.oldSelectionIndex) + ', new = ' + s.getData(e.newSelectionIndex));
}

function selectionChangedEventHandler(s, e) {
	logEvent(s, 'SelectionChanged, new = ' + s.getData(s.get_selectedRowIndex()));
}

function cellClickedEventHandler(s, e) {
	logEvent(s, 'CellClicked, (' + s.getData(e.row) + ', ' + e.col + ')');
}

function dragDropCompletingEventHandler(s, e) {
	logEvent(s, 'DragDropCompleting, item = ' + e.itemIndex + ', drop index = ' + e.dropIndex + ', old item data = ' + s.getData(e.itemIndex));
}

function dragDropCompletedEventHandler(s, e) {
	logEvent(s, 'DragDropCompleted, item = ' + e.itemIndex + ', drop index = ' + e.dropIndex + ', new item data = ' + s.getData(e.dropIndex));
}

function init() {
	var cfg = <%= Utils.InitScript(controls.Select(g => g.ConfigObject)) %>;
	controls = new Array(cfg.length);
	for (var i = 0; i < cfg.length; i++) {
		var c = new Saltarelle.UI.Grid(cfg[i]);
		c.add_keyPress(keyPressEventHandler);
		c.add_selectionChanging(selectionChangingEventHandler);
		c.add_cellClicked(cellClickedEventHandler);
		c.add_selectionChanged(selectionChangedEventHandler);
		c.add_dragDropCompleting(dragDropCompletingEventHandler);
		c.add_dragDropCompleted(dragDropCompletedEventHandler);
		controls[i] = c;
	}
	
	$('#verifyDataButton').click(function() {
		for (var i = 0; i < controls.length; i++) {
			var numRows = controls[i].get_numRows();
			for (var j = 0; j < numRows; j++) {
				var data = controls[i].getData(j), cellTexts = controls[i].getTexts(j);
				var match = /^Cell ([0-9]+)\,.*/.exec(cellTexts[0]);
				if (parseInt(match[1]) != data) {
					alert('Expected ' + match[1] + ', got ' + data);
					break;
				}
			}
		}
	});	

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
				<button type="button" id="verifyDataButton">Verify Data</button><br />
				<div style="width: 500px; height: 600px" id="events">
				</div>
			</td>
		</tr>
		<tr>
			<td>
				<%= controls[2].Html %>
			</td>
			<td>
				&nbsp;
			</td>
		</tr>
	</table>
</body>
</html>
