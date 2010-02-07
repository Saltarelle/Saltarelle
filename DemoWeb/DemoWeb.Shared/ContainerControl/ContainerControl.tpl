<?xml version="1.0" encoding="utf-8"?>
<div>
	<control type="DemoWeb.SimpleControl" id="Nested" Person="person: Källén, Erik"/>
	<control type="Saltarelle.UI.Grid" id="grid" ColWidths="int[]:150|100" Width="int:280" Height="int:200" ColTitles="str[]:Column 1|Column2"/>
	<control type="Saltarelle.UI.Tree" id="tree" Width="int:280" Height="int:200"/>
	<control type="Saltarelle.UI.DialogFrame" id="dialog" ModalityInt="int:2">
		<div style="width: 200px; height: 200px">
			Dialog content
		</div>
	</control>
	<control type="Saltarelle.UI.GroupBox" id="group" Title="str:Group title">
		<button type="button" id="showDialogButton">Open Dialog</button>
		<control type="Saltarelle.UI.Label" Text="str:Some Text"/>
	</control>
	<control type="Saltarelle.UI.TabControl" TabCaptions="str[]:Tab 1|Tab 2|Tab 3" RightAlignTabs="bool:true" SelectedTab="int:1">
		<div style="width: 200px; height: 200px">
			Content 1
		</div>
		<div style="width: 200px; height: 200px">
			Content 2
		</div>
		<div style="width: 200px; height: 200px">
			Content 3
		</div>
	</control>
</div>