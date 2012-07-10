param($installPath, $toolsPath, $package, $project)

Add-Type -AssemblyName 'Microsoft.Build, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
$msbuild = [Microsoft.Build.Evaluation.ProjectCollection]::GlobalProjectCollection.GetLoadedProjects($project.FullName) | Select-Object -First 1

$canonicalName = ($project.Name -replace "\.Client$","")

Function MakeRelativePath($Origin, $Target) {
    $originUri = New-Object Uri('file://' + $Origin)
    $targetUri = New-Object Uri('file://' + $Target)
    $originUri.MakeRelativeUri($targetUri).ToString().Replace('/', [System.IO.Path]::DirectorySeparatorChar)
}

Function Add-DefineConstant($Configuration, [string]$Constant) {
	$defineConstants = $Configuration.Properties.Item("DefineConstants").Value
	if ($defineConstants) {
		if (-not (";$defineConstants;" | Select-String ";$Constant;")) {
			$Configuration.Properties.Item("DefineConstants").Value = "$defineConstants;$Constant"
		}
	}
	else {
		$Configuration.Properties.Item("DefineConstants").Value = $Constant
	}
}

Function Add-OrderingDependency($From, $To, [switch]$Save) {
	$msb = [Microsoft.Build.Evaluation.ProjectCollection]::GlobalProjectCollection.GetLoadedProjects($From.FullName) | Select-Object -First 1
	$relPath = MakeRelativePath -Origin $From.FullName -Target $To.FullName

	If (-not ($msb.Items | ? { $_.Key -eq "ProjectReference" } | % { $_.Value } | ? { $_.UnevaluatedInclude -eq $relPath })) {
		$ref = $msb.AddItem("ProjectReference", $relPath) | Select-Object -First 1
		$ref.SetMetadataValue("ReferenceOutputAssembly", "false") > $null
		$ref.SetMetadataValue("Name", "$($To.Name) (ordering only)") > $null

		if ($Save) {
			$msb.Save()
		}
	}
}

# Exclude trailing .Client/.Server from the root namespace (unless it has already been changed)
$rootNamespace = $msbuild.Properties | ? { $_.Name -eq "RootNamespace" } | % { $_.UnevaluatedValue } | Select-Object -First 1
if ($rootNamespace -eq $project.Name) {
	$msbuild.SetProperty("RootNamespace", $canonicalName)
}

# Import Saltarelle.targets
$msbuild.Xml.Imports | ? { $_.Project.EndsWith("Saltarelle.targets") } | % { $msbuild.Xml.RemoveChild($_) }
$msbuild.Xml.AddImport("`$(SolutionDir)$(MakeRelativePath -Origin $project.DTE.Solution.FullName -Target ([System.IO.Path]::Combine($toolsPath, ""Saltarelle.targets"")))")

# Add the CLIENT define constant
$project.ConfigurationManager | % { Add-DefineConstant -Configuration $_ -Constant "CLIENT" }

# Add a reference from the server project to this project (if the server project exists)
$serverProject = $project.Collection | ? { $_.Name -eq "$canonicalName.Server" }
if ($serverProject) {
	Add-OrderingDependency -From $serverProject -To $project -Save
}

$project.Save()
