<div>
	<field name="person" type="DemoWeb.Person"/>
	<control id="Textbox" type="DemoWeb.TextInput" Value='code:"Some value"'/>
	<button id="AlertButton" type="button">Show value</button>
	<div id="ValueDisplayer">&amp;nbsp;</div>
	<img title="CopyrightNotice"/>
	<copyright>Erik Källén</copyright>
	<br/>
	<?x "This expression will be printed as code" ?>
	<br/>
	<?c int i = 0; ?>
	<?x "Before: " + i.ToString() ?>
	<br/>
	<?c i++; ?>
	<?x "After: " + i.ToString() ?>
	<br/>
	<for stmt="int a = 0; a &lt; 10; a++">
		<div>Iteration <?x a?>.</div>
	</for>
</div>
