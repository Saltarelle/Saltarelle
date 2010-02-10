<div>
<div>
Enter some markup:<br/>
<textarea id="DynamicMarkupInput" rows="10" cols="80">
&lt;div style="background-color: red"&gt;
	&lt;control type="Saltarelle.UI.Label" Text="str:Label text"/&gt;
	&lt;div&gt;
		Some text
	&lt;/div&gt;
&lt;/div&gt;
</textarea>
<br/>
<button type="button" id="InsertDynamicControlButton">Insert Control</button>
</div>
<div id="DynamicControlContainer">&amp;nbsp;</div>
<div>
Number of rows: <input type="text" id="NumRowsInput" value="0" style="width: 50px"/>
<button type="button" id="AjaxButton">Create grid using Ajax</button>
</div>
<div id="AjaxControlContainer">&amp;nbsp;</div>
</div>