param($installPath, $toolsPath, $package, $project)

$isClient = $project.Name.EndsWith(".Client")

Function MakeRelativePath($Origin, $Target) {
    $originUri = New-Object Uri('file://' + $Origin)
    $targetUri = New-Object Uri('file://' + $Target)
    $originUri.MakeRelativeUri($targetUri).ToString().Replace('/', [System.IO.Path]::DirectorySeparatorChar)
}

Function AddFile([string]$RelativePath, [string]$ItemType, [switch]$DeleteFileIfAdded) {
	$fileName = [System.IO.Path]::GetFileName($RelativePath)
	if (-not ($project.ProjectItems | ? { $_.Name -eq $fileName })) {
		$filePath = [System.IO.Path]::GetFullPath([System.IO.Path]::Combine([System.IO.Path]::GetDirectoryName($project.FileName), $RelativePath))
		if (-not (Test-Path "$filePath")) {
			New-Item $filePath -Type File > $null
			$added = $true
		}
		$item = $project.ProjectItems.AddFromFile($filePath)
		$item.Properties.Item("ItemType").Value = $ItemType

		if ($added -and $DeleteFileIfAdded) {
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

$canonicalName = ($project.Name -replace "\.(Client|Server)$","")

Add-Type -AssemblyName 'Microsoft.Build, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
$msbuild = [Microsoft.Build.Evaluation.ProjectCollection]::GlobalProjectCollection.GetLoadedProjects($project.FullName) | Select-Object -First 1

# Exclude trailing .Client/.Server from the root namespace (unless it has already been changed)
$rootNamespace = $msbuild.Properties | ? { $_.Name -eq "RootNamespace" } | % { $_.UnevaluatedValue } | Select-Object -First 1
if ($rootNamespace -eq $project.Name) {
	$msbuild.SetProperty("RootNamespace", $canonicalName)
}

if ($isClient) {
	# Remove serverside references added by us
	$project.Object.References.Item("SaltarelleLib").Remove()
	$project.Object.References.Item("Newtonsoft.Json").Remove()

	# Remove default assemblies System, System.*, Microsoft.*
	$project.Object.References | ? { $_.Name.StartsWith("System.") } | % { $_.Remove() }
	$project.Object.References | ? { $_.Name -eq "System" } | % { $_.Remove() }
	$project.Object.References | ? { $_.Name.StartsWith("Microsoft.") } | % { $_.Remove() }

	# Swap the import for Microsoft.CSharp.targets for nStuff.ScriptSharp.targets
	$msbuild.Xml.Imports | ? { $_.Project.EndsWith("nStuff.ScriptSharp.targets") } | % { $msbuild.Xml.RemoveChild($_) }
	$msbuild.Xml.Imports | ? { $_.Project.EndsWith("Microsoft.CSharp.targets") } | % { $msbuild.Xml.RemoveChild($_) }
	$msbuild.Xml.AddImport("`$(SolutionDir)$(MakeRelativePath -Origin $project.DTE.Solution.FullName -Target ([System.IO.Path]::Combine($toolsPath, ""nStuff.ScriptSharp.targets"")))")
	
	# Import Saltarelle.targets
	$msbuild.Xml.Imports | ? { $_.Project.EndsWith("Saltarelle.targets") } | % { $msbuild.Xml.RemoveChild($_) }
	$msbuild.Xml.AddImport("`$(SolutionDir)$(MakeRelativePath -Origin $project.DTE.Solution.FullName -Target ([System.IO.Path]::Combine($toolsPath, ""Saltarelle.targets"")))")
	
	# Set the NoStdLib and TemplateFile properties
	$msbuild.SetProperty("NoStdLib", "True")
	$msbuild.SetProperty("TemplateFile", "Properties\Script.jst")
	
	# Add a default script template (Script.jst) to the project
	$propertiesDir = [System.IO.Path]::Combine([System.IO.Path]::GetDirectoryName($project.FileName), "Properties")
	if (-not (Test-Path $propertiesDir)) {
		md $propertiesDir
	}
	"#include[as-is] ""%code%""" | Out-File ([System.IO.Path]::Combine($propertiesDir, "Script.jst")) -Encoding UTF8
	AddFile "Properties\Script.jst" -ItemType "Content"
	
	# Add the CLIENT define constant
	$project.ConfigurationManager | % { Add-DefineConstant -Configuration $_ -Constant "CLIENT" }
}
else {
	# Remove serverside references added by us
	$project.Object.References.Item("SaltarelleLib.Client").Remove()
	$project.Object.References.Item("sscorlib").Remove()

	# Add references to the files that the client assembly produces
	AddFile "..\Client.dll" -ItemType "EmbeddedResource" -DeleteFileIfAdded
	AddFile "..\Script.js" -ItemType "EmbeddedResource" -DeleteFileIfAdded
	AddFile "..\Script.min.js" -ItemType "EmbeddedResource" -DeleteFileIfAdded
	AddFile "Module.less" -ItemType "EmbeddedResource"

	# Add the SERVER define constant
	$project.ConfigurationManager | % { Add-DefineConstant -Configuration $_ -Constant "SERVER" }

	# Update the assembly name to be the canonical name (unless it has already been changed)
	$assemblyName = $msbuild.Properties | ? { $_.Name -eq "AssemblyName" } | % { $_.UnevaluatedValue } | Select-Object -First 1
	if ($assemblyName -eq $project.Name) {
		$msbuild.SetProperty("AssemblyName", $canonicalName)
	}
}
