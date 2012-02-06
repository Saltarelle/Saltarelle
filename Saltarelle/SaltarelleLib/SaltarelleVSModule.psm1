Function New-SaltarellePair([Parameter(Mandatory=$true)][string]$Name, [string]$Path) {
	$solution = Get-Interface $DTE.Solution ([EnvDTE80.Solution2])
	$baseDir = Join-Path (Join-Path (Split-Path $solution.FullName) $Path) $Name
	if (Test-Path $baseDir) {
		Write-Error "Directory $baseDir already exists"
		return
	}
	md "$baseDir" > $null
	
	$template = $solution.GetProjectTemplate("ClassLibrary.zip", "CSharp")
	
	$folderProject = $solution.AddSolutionFolder($Name)
	$folder = Get-Interface $folderProject.Object ([EnvDTE80.SolutionFolder])
	$folder.AddFromTemplate($template, "$baseDir\$Name.Client", "$Name.Client", $false)
	$folder.AddFromTemplate($template, "$baseDir\$Name.Server", "$Name.Server", $false)

	Install-Package -ProjectName "$Name.Client" -Id "SaltarelleCore"
	Install-Package -ProjectName "$Name.Server" -Id "SaltarelleCore"
}
