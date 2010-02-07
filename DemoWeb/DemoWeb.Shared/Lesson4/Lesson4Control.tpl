<div>
	<table>
		<tr>
			<td style="padding-right: 20px">
				<control type="Saltarelle.UI.Tree" id="DepartmentsTree" Width="int:300" Height="int:300"/>
			</td>
			<td>
				<control type="Saltarelle.UI.Grid" id="EmployeesGrid" ColTitles="str[]:First Name|Last Name|" ColWidths="int[]:200|200|50" ColClasses="str[]:||ActionCol" Width="int:500" Height="int:200"/>
			</td>
		</tr>
	</table>
	<control type="Saltarelle.UI.DialogFrame" id="EditEmployeeDialog" ModalityInt="int:1" Title="str:Employee Details">
		<table>
			<tr>
				<td style="text-align: right; padding-bottom: 4px; padding-right: 4px; white-space: nowrap">First Name:</td>
				<td style="padding-bottom: 4px"><input type="text" id="FirstNameInput" style="width: 200px"/></td>
			</tr>
			<tr>
				<td style="text-align: right; padding-bottom: 4px; padding-right: 4px; white-space: nowrap">Last Name:</td>
				<td style="padding-bottom: 4px"><input type="text" id="LastNameInput" style="width: 200px"/></td>
			</tr>
			<tr>
				<td style="text-align: right; padding-bottom: 4px; padding-right: 4px; white-space: nowrap">Title:</td>
				<td style="padding-bottom: 4px"><input type="text" id="TitleInput" style="width: 40px"/></td>
			</tr>
			<tr>
				<td style="text-align: right; padding-bottom: 4px; padding-right: 4px; white-space: nowrap">Email Address:</td>
				<td style="padding-bottom: 4px"><input type="text" id="EmailInput" style="width: 250px"/></td>
			</tr>
			<tr>
				<td colspan="2" style="text-align: center">
					<button type="button" style="width: 80px" id="EditEmployeeOKButton">OK</button>
					<button type="button" style="width: 80px" id="EditEmployeeCancelButton">Cancel</button>
				</td>
			</tr>
		</table>
	</control>
</div>