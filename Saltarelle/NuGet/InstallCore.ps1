param($installPath, $toolsPath, $package, $project)

$isClient = $project.Name.EndsWith(".Client")

Function MakeRelativePath($Origin, $Target) {
    $originUri = New-Object Uri('file://' + $Origin)
    $targetUri = New-Object Uri('file://' + $Target)
    $originUri.MakeRelativeUri($targetUri).ToString().Replace('/', [System.IO.Path]::DirectorySeparatorChar)
}

Function AddEmbeddedResource([string]$RelativePath, [switch]$DeleteFileIfAdded) {
	$fileName = [System.IO.Path]::GetFileName($RelativePath)
	if (-not ($project.ProjectItems | ? { $_.Name -eq $fileName })) {
		$filePath = [System.IO.Path]::GetFullPath([System.IO.Path]::Combine([System.IO.Path]::GetDirectoryName($project.FileName), $RelativePath))
		if (-not (Test-Path "$filePath")) {
			New-Item $filePath -Type File > $null
			$added = $true
		}
		$item = $project.ProjectItems.AddFromFile($filePath)
		$item.Properties.Item("ItemType").Value = "EmbeddedResource"

		if ($added -and $DeleteFileIfAdded) {
			rm $filePath > $null
		}
	}
}

if ($isClient) {
	$project.Object.References.Item("SaltarelleLib").Remove()
	$project.Object.References.Item("Newtonsoft.Json").Remove()

    Add-Type -AssemblyName 'Microsoft.Build, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
    $msbuild = [Microsoft.Build.Evaluation.ProjectCollection]::GlobalProjectCollection.GetLoadedProjects($project.FullName) | Select-Object -First 1

	$msbuild.Xml.Imports | ? { $_.Project.EndsWith("nStuff.ScriptSharp.targets") } | % { $msbuild.Xml.RemoveChild($_) }
	$msbuild.Xml.Imports | ? { $_.Project.EndsWith("Microsoft.CSharp.targets") } | % { $msbuild.Xml.RemoveChild($_) }
	$msbuild.Xml.AddImport("`$(SolutionDir)$(MakeRelativePath -Origin $project.DTE.Solution.FullName -Target ([System.IO.Path]::Combine($toolsPath, ""nStuff.ScriptSharp.targets"")))")
	
	$msbuild.Xml.Imports | ? { $_.Project.EndsWith("Saltarelle.targets") } | % { $msbuild.Xml.RemoveChild($_) }
	$msbuild.Xml.AddImport("`$(SolutionDir)$(MakeRelativePath -Origin $project.DTE.Solution.FullName -Target ([System.IO.Path]::Combine($toolsPath, ""Saltarelle.targets"")))")
}
else {
	$project.Object.References.Item("SaltarelleLib.Client").Remove()
	$project.Object.References.Item("sscorlib").Remove()

	AddEmbeddedResource "..\Client.dll" -DeleteFileIfAdded
	AddEmbeddedResource "..\Script.js" -DeleteFileIfAdded
	AddEmbeddedResource "..\Script.min.js" -DeleteFileIfAdded
	AddEmbeddedResource "Module.less"
}
