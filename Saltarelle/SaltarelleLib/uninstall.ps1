param($installPath, $toolsPath, $package, $project)

Add-Type -AssemblyName 'Microsoft.Build, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
$msbuild = [Microsoft.Build.Evaluation.ProjectCollection]::GlobalProjectCollection.GetLoadedProjects($project.FullName) | Select-Object -First 1

$msbuild.Xml.Imports | ? { $_.Project.EndsWith("nStuff.ScriptSharp.targets") } | % { $msbuild.Xml.RemoveChild($_) }
$msbuild.Xml.Imports | ? { $_.Project.EndsWith("Saltarelle.targets") } | % { $msbuild.Xml.RemoveChild($_) }
