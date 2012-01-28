param($installPath, $toolsPath, $package, $project)

$isClient = $project.Name.EndsWith(".Client")

if ($isClient) {
	# Remove serverside reference added by us
	$project.Object.References | ? { $_.Name -eq "Saltarelle.UI" } | % { $_.Remove() }
}
else {
	# Remove clientside references added by us
	$project.Object.References | ? { $_.Name -eq "Saltarelle.UI.Client" } | % { $_.Remove() }
}
