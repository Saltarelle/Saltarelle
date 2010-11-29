<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="Saltarelle" %>
<%@ Import Namespace="Saltarelle.Mvc" %>
<%@ Import Namespace="DemoWeb" %>
<%--
<script runat="server">
private Lesson5Control control;

protected override void OnLoad(EventArgs e) {
	control = new Lesson5Control() { Id = "control" };
	GlobalServices.GetService<IScriptManagerService>().RegisterTopLevelControl(control);
}
</script>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>Lesson 5</title>
	<link rel="Stylesheet" href="/Content/themes/base/ui.all.css" type="text/css"/>
	<link rel="Stylesheet" href="/Content/site.css" type="text/css"/>
	<link rel="Stylesheet" href="/Stylesheet" type="text/css"/>
<% Html.Scripts(); %>
</head>

<body style="margin: 20px">
	<%= control.Html %>
</body>
</html>
--%>