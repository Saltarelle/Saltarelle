param($installPath, $toolsPath, $package, $project)

$project.Object.References | % { $_.Name }