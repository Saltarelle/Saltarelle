param($installPath, $toolsPath, $package, $project)

Add-Type -AssemblyName 'Microsoft.Build, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
$msbuild = [Microsoft.Build.Evaluation.ProjectCollection]::GlobalProjectCollection.GetLoadedProjects($project.FullName) | Select-Object -First 1

$canonicalName = ($project.Name -replace "\.Server$","")

Function MakeRelativePath($Origin, $Target) {
    $originUri = New-Object Uri('file://' + $Origin)
    $targetUri = New-Object Uri('file://' + $Target)
    $originUri.MakeRelativeUri($targetUri).ToString().Replace('/', [System.IO.Path]::DirectorySeparatorChar)
}

Function Add-File([string]$RelativePath, [string]$ItemType, [switch]$DeleteFileIfCreated) {
	$fileName = [System.IO.Path]::GetFileName($RelativePath)
	if (-not ($project.ProjectItems | ? { $_.Name -eq $fileName })) {
		$filePath = [System.IO.Path]::GetFullPath([System.IO.Path]::Combine([System.IO.Path]::GetDirectoryName($project.FileName), $RelativePath))
		if (-not (Test-Path "$filePath")) {
			New-Item $filePath -Type File > $null
			$added = $true
		}
		$item = $project.ProjectItems.AddFromFile($filePath)
		$item.Properties.Item("ItemType").Value = $ItemType

		if ($added -and $DeleteFileIfCreated) {
			rm $filePath > $null
		}
	}
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

# Add references to the files that the client assembly produces
Add-File "..\Client.dll" -ItemType "EmbeddedResource" -DeleteFileIfCreated
Add-File "..\Script.js" -ItemType "EmbeddedResource" -DeleteFileIfCreated
Add-File "Module.less" -ItemType "EmbeddedResource"

# Add the SERVER define constant
$project.ConfigurationManager | % { Add-DefineConstant -Configuration $_ -Constant "SERVER" }

# Update the assembly name to be the canonical name (unless it has already been changed)
$assemblyName = $msbuild.Properties | ? { $_.Name -eq "AssemblyName" } | % { $_.UnevaluatedValue } | Select-Object -First 1
if ($assemblyName -eq $project.Name) {
	$msbuild.SetProperty("AssemblyName", $canonicalName)
}

# Add a reference from this project to the corresponding client project (if the client project exists)
$clientProject = $project.Collection | ? { $_.Name -eq "$canonicalName.Client" }
if ($clientProject) {
	Add-OrderingDependency -From $project -To $clientProject
}

$project.Save()