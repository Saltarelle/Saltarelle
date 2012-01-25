param($installPath, $toolsPath, $package, $project)

$isClient = $project.Name.EndsWith(".Client")

if ($isClient) {
	$project.Object.References.Item("SaltarelleLib").Remove()
	$project.Object.References.Item("Newtonsoft.Json").Remove()
}
else {
	$project.Object.References.Item("SaltarelleLib.Client").Remove()
}
