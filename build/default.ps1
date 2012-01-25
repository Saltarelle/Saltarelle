Framework "4.0x86"

properties {
	$base_dir = Resolve-Path ".."
	$buildtools_dir = Resolve-Path "."
	$out_dir = "$(Resolve-Path "".."")\build_out"
	$configuration = "Debug With Installer"
	$release_tag_pattern = "release-(.*)"
}

Task default -Depends Build

Task Clean {
	if (Test-Path $out_dir) {
		rm -Recurse -Force "$out_dir" >$null
	}
	md "$out_dir" >$null
}

Task Build -Depends Clean, Generate-VersionInfo {
	Exec { msbuild "$base_dir\Saltarelle\Saltarelle.sln" /p:"Configuration=$configuration" }
}

Task Publish -Depends Publish-Zip, Publish-Nupkg {
}

Task Publish-Zip -Depends Determine-Version, Build, Run-Tests {
#	$out_zip = "$out_dir\Publish\Engine-$script:version.zip"
#
#	If (Test-Path "$out_zip") {
#		rm "$out_zip"
#	}
#
#	Exec { & "$buildtools_dir\7z.exe" a -y -bd -r -tzip "$out_zip" "$out_dir\Engine\" "$out_dir\Tools\" }
}

Task Publish-Nupkg -Depends Determine-Version, Build, Run-Tests {
#@"
#<package xmlns="http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd">
#	<metadata>
#		<id>Engine</id>
#		<version>$script:version</version>
#		<title>Data Collection Engine</title>
#		<description>Data Collection Engine</description>
#		<authors>Erik Källén</authors>
#	</metadata>
#	<files>
#		<file src="$out_dir\Engine\**\*.*" target="lib\net35"/>
#		<file src="$out_dir\Tools\**\*.*" target="tools"/>
#	</files>
#</package>
#"@ >"$out_dir\Engine.nuspec"
#
#	& "$buildtools_dir\nuget.exe" pack "$out_dir\Engine.nuspec" -OutputDirectory "$out_dir\Publish"
#	rm "$out_dir\Engine.nuspec"
}

Task Run-Tests {
#	$test_assemblies_file = "$base_dir\Project\TestAssemblies.txt"
#
#	if (Test-Path "$test_assemblies_file") {
#		$testasms = @(Get-Content "$test_assemblies_file")
#	}
#	else {
#		$testasms = @()
#	}
#	
#	if ($testasms.Count -ne 0) {
#		$runner = (dir "$base_dir\Project\packages" -Recurse -Filter nunit-console.exe | Select -ExpandProperty FullName)
#		Exec { & "$runner" $testasms -nologo -xml "$out_dir\TestResults.xml" }
#	}
}

Task Configure -Depends Generate-VersionInfo {
}

Task Determine-Version {
	$script:version = 
		git log --decorate=full --simplify-by-decoration --pretty=oneline |             # Get the log
		Select-String '\(' |                                                            # Only include entries with names
		% { ($_ -replace "^[^(]*\(([^)]*)\).*$","`$1" -replace " ", "").Split(',') } |  # Select only the names, one line per name, delete spaces
		Select-String "^tag:$release_tag_pattern`$" |                                   # Only tags of interest
		% { $_ -replace "^tag:$release_tag_pattern`$", "`$1" } |                        # Extract the version
		Select-Object -First 1                                                          # Take the first (most recent) one

	if ($script:version -eq $null) {
		$script:version = "1.0"
	}
	
	$v = New-Object System.Version($script:version)
	$script:ExecutablesAssemblyVersion = "$($v.Major).$($v.Minor).0.0"
	$script:ProductVersion = "$($v.Major).$($v.Minor).0"

	"Version: $script:version"
	"ExecutablesAssemblyVersion: $script:ExecutablesAssemblyVersion"
	"ProductVersion: $script:ProductVersion"
}

Task Generate-VersionInfo -Depends Determine-Version {
@"
[assembly: System.Reflection.AssemblyVersion("$script:version")]
[assembly: System.Reflection.AssemblyFileVersion("$script:version")]
"@ | Out-File "$base_dir\Saltarelle\SaltarelleVersion.cs" -Encoding "UTF8"

@"
[assembly: System.Reflection.AssemblyVersion("$script:ExecutablesAssemblyVersion")]
[assembly: System.Reflection.AssemblyFileVersion("$script:ExecutablesAssemblyVersion")]
"@ | Out-File "$base_dir\Saltarelle\Executables\ExecutablesVersion.cs" -Encoding "UTF8"

@"
<?xml version="1.0" encoding="utf-8"?>
<Include>
	<?define ProductVersion="$script:ProductVersion"?>
	<?define ExecutablesAssemblyVersion="$script:ExecutablesAssemblyVersion"?>
</Include>
"@ | Out-File "$base_dir\Saltarelle\Installer\Version.wxi" -Encoding UTF8
}
