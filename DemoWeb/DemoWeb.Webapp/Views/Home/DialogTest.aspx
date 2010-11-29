<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="Saltarelle" %>
<%@ Import Namespace="Saltarelle.UI" %>
<%@ Import Namespace="Saltarelle.Mvc" %>
<%@ Import Namespace="DemoWeb" %>
<%@ Import Namespace="System.Globalization" %>

<script runat="server">
private DialogBase[] controls;

private const int NumDialogs = 12;
private string buttonsHtml;

protected override void OnLoad(EventArgs e) {
	base.OnLoad(e);
	
	var sb = new StringBuilder();
	
	for (int i = 0; i < NumDialogs; i++)
		sb.AppendFormat(CultureInfo.InvariantCulture, "<button style=\"display: block\" class=\"Open\" __dlg=\"{0}\" type=\"button\">Open dialog {0}</button>", i);
	buttonsHtml = sb.ToString();
	
	controls = new DialogBase[NumDialogs];
	for (int i = 0; i < NumDialogs; i++) {
		var f = new DialogFrame() { Id = "dlg" + i.ToString(CultureInfo.InvariantCulture) };
		f.SetInnerFragments(new[] { "<div style=\"width: 200px; height: 400px\">Some rather long content that is not a button" + buttonsHtml + "<button style=\"display: block\" class=\"Close\" __dlg=\"" + i.ToString(CultureInfo.InvariantCulture) + "\">Close</button></div>" });
		f.Title      = ((i % 2) == 0 ? "Dialog " + i.ToString(CultureInfo.InvariantCulture) : null);
		f.Modality   = (i < 4 ? DialogModalityEnum.Modal : (i < 8 ? DialogModalityEnum.Modeless : DialogModalityEnum.HideOnFocusOut));
		f.HasPadding = (i < 6);
		if (i > 0)
			f.Position = PositionHelper.LeftTop((i % 6) * 200, (i / 6) * 200);
		controls[i]  = f;
	}
}
</script>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>Dialog Tests</title>
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

function logEvent(dlg, msg) {
	$('<div/>').text(dlg.get_id() + ': ' + msg).appendTo($('#events'));
}

function openedEventHandler(s, e) {
	logEvent(s, 'Opened');
}

function closedEventHandler(s, e) {
	logEvent(s, 'Closed');
}

function openingEventHandler(s, e) {
	logEvent(s, 'Opening');
}

function closingEventHandler(s, e) {
	logEvent(s, 'Closing');
}

function init() {
	var cfg = <%= Utils.InitScript(controls.Select(d => d.ConfigObject)) %>;
	controls = new Array(cfg.length);
	for (var i = 0; i < cfg.length; i++) {
		var c = new Saltarelle.UI.DialogFrame(cfg[i]);
		c.add_opened(openedEventHandler);
		c.add_closed(closedEventHandler);
		c.add_opening(openingEventHandler);
		c.add_closing(closingEventHandler);
		controls[i] = c;
	}

	$('button.Open').click(function(evt) {
		controls[evt.currentTarget.getAttribute('__dlg')].open();
	});

	$('button.Close').click(function(evt) {
		controls[evt.currentTarget.getAttribute('__dlg')].close();
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
	<%
	for (int i = 0; i < 20; i++) {
		Response.Write("<select style=\"position: absolute; left: " + ((i * 20) + 20).ToString() + "px; top: " + ((i * 20) + 20).ToString() + "px\"><option>Text</option></select>");
	}

	for (int i = 0; i < controls.Length; i++) {
		Response.Write(controls[i].Html);
	}
	%>
	<table>
		<tr>
			<td style="width: 600px">
				<%= buttonsHtml %>
			</td>
			<td>
				<div>
					<input type="text" style="width: 300px" id="scriptInput"/><button type="button" id="Button1">Eval</button>
				</div>
				<button type="button" id="clearEventsButton">Clear</button>
				<div style="width: 400px; height: 600px" id="events">
				</div>
			</td>
		</tr>
	</table>
</body>
</html>
