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

Add-Type -AssemblyName 'Microsoft.Build, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
$msbuild = [Microsoft.Build.Evaluation.ProjectCollection]::GlobalProjectCollection.GetLoadedProjects($project.FullName) | Select-Object -First 1
$rootNamespace = $msbuild.Properties | ? { $_.Name -eq "RootNamespace" } | % { $_.UnevaluatedValue } | Select-Object -First 1
if ($rootNamespace -eq $project.Name) {
	$msbuild.SetProperty("RootNamespace", ($project.Name -replace "\.(Client|Server)$",""))
}

if ($isClient) {
	$project.Object.References.Item("SaltarelleLib").Remove()
	$project.Object.References.Item("Newtonsoft.Json").Remove()

	$project.Object.References | ? { $_.Name.StartsWith("System.") } | % { $_.Remove() }
	$project.Object.References | ? { $_.Name -eq "System" } | % { $_.Remove() }
	$project.Object.References | ? { $_.Name.StartsWith("Microsoft.") } | % { $_.Remove() }

	$msbuild.Xml.Imports | ? { $_.Project.EndsWith("nStuff.ScriptSharp.targets") } | % { $msbuild.Xml.RemoveChild($_) }
	$msbuild.Xml.Imports | ? { $_.Project.EndsWith("Microsoft.CSharp.targets") } | % { $msbuild.Xml.RemoveChild($_) }
	$msbuild.Xml.AddImport("`$(SolutionDir)$(MakeRelativePath -Origin $project.DTE.Solution.FullName -Target ([System.IO.Path]::Combine($toolsPath, ""nStuff.ScriptSharp.targets"")))")
	
	$msbuild.Xml.Imports | ? { $_.Project.EndsWith("Saltarelle.targets") } | % { $msbuild.Xml.RemoveChild($_) }
	$msbuild.Xml.AddImport("`$(SolutionDir)$(MakeRelativePath -Origin $project.DTE.Solution.FullName -Target ([System.IO.Path]::Combine($toolsPath, ""Saltarelle.targets"")))")
	
	$msbuild.SetProperty("NoStdLib", "True")
	$msbuild.SetProperty("TemplateFile", "Properties\Script.jst")
	
	$propertiesDir = [System.IO.Path]::Combine([System.IO.Path]::GetDirectoryName($project.FileName), "Properties")
	
	if (-not (Test-Path $propertiesDir)) {
		md $propertiesDir
	}
	"#include[as-is] ""%code%""" | Out-File ([System.IO.Path]::Combine($propertiesDir, "Script.jst")) -Encoding UTF8
	AddFile "Properties\Script.jst" -ItemType "Content"
}
else {
	$project.Object.References.Item("SaltarelleLib.Client").Remove()
	$project.Object.References.Item("sscorlib").Remove()

	AddFile "..\Client.dll" -ItemType "EmbeddedResource" -DeleteFileIfAdded
	AddFile "..\Script.js" -ItemType "EmbeddedResource" -DeleteFileIfAdded
	AddFile "..\Script.min.js" -ItemType "EmbeddedResource" -DeleteFileIfAdded
	AddFile "Module.less" -ItemType "EmbeddedResource"
}
