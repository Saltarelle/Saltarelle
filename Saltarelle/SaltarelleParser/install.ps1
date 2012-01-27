param($installPath, $toolsPath, $package, $project)

$isClient = $project.Name.EndsWith(".Client")

if ($isClient) {
	# Remove serverside reference added by us
	$project.Object.References.Item("SaltarelleParser").Remove()
}
else {
	# Remove clientside references added by us
	$project.Object.References.Item("SaltarelleParser.Client").Remove()
}
