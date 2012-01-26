param($installPath, $toolsPath, $package, $project)

$isClient = $project.Name.EndsWith(".Client")

if ($isClient) {
	# Remove serverside reference added by us
	$project.Object.References.Item("Saltarelle.UI").Remove()
}
else {
	# Remove clientside references added by us
	$project.Object.References.Item("Saltarelle.UI.Client").Remove()
}
