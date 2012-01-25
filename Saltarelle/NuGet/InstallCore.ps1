param($installPath, $toolsPath, $package, $project)

$isClient = $project.Name.EndsWith(".Client")

Function MakeRelativePath($Origin, $Target) {
    $originUri = New-Object Uri('file://' + $Origin)
    $targetUri = New-Object Uri('file://' + $Target)
    $originUri.MakeRelativeUri($targetUri).ToString().Replace('/', [System.IO.Path]::DirectorySeparatorChar)
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
}
