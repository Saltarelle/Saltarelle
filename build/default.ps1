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

Task Publish -Depends Determine-Version, Build, Run-Tests {
	$authors = "Erik Källén"

	If (Test-Path "$out_dir\zip") {
		rm -Recurse -Force "$out_dir\Publish"
	}
	md "$out_dir\Publish"
	
@"
<package xmlns="http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd">
	<metadata>
		<id>SaltarelleCore</id>
		<version>$script:version</version>
		<title>Saltarelle Core Library</title>
		<description>Saltarelle Core Library</description>
		<authors>$authors</authors>
		<dependencies>
			<dependency id="Newtonsoft.Json" version="4.0.7" />
		</dependencies>
	</metadata>
	<files>
		<file src="$base_dir\Saltarelle\SaltarelleLib\SaltarelleLib.Client\bin\SaltarelleLib.Client.dll" target="lib"/>
		<file src="$base_dir\Saltarelle\SaltarelleLib\SaltarelleLib.Client\bin\SaltarelleLib.Client.xml" target="lib"/>
		<file src="$base_dir\Saltarelle\SaltarelleLib\SaltarelleLib.Server\bin\SaltarelleLib.dll" target="lib"/>
		<file src="$base_dir\Saltarelle\SaltarelleLib\SaltarelleLib.Server\bin\SaltarelleLib.xml" target="lib"/>
		<file src="$base_dir\Saltarelle\SaltarelleLib\SaltarelleLib.Server\bin\SaltarelleLib.pdb" target="lib"/>
		<file src="$base_dir\Saltarelle\VSIntegrationInstaller\bin\SaltarelleVSIntegration.msi" target="tools\SaltarelleVSIntegration-$script:ProductVersion.msi"/>
		<file src="$base_dir\Saltarelle\Executables\SalgenTask\bin\Saltarelle.SalgenTask.dll" target="tools"/>
		<file src="$base_dir\Saltarelle\Executables\SalgenTask\bin\Saltarelle.targets" target="tools"/>
		<file src="$base_dir\Saltarelle\Executables\Salgen.exe\bin\salgen.exe" target="tools"/>
		<file src="$base_dir\Saltarelle\packages-manual\ScriptSharp\sscorlib.dll" target="lib"/>
		<file src="$base_dir\Saltarelle\packages-manual\ScriptSharp\nStuff.ScriptSharp.dll" target="tools"/>
		<file src="$base_dir\Saltarelle\packages-manual\ScriptSharp\nStuff.ScriptSharp.targets" target="tools"/>
		<file src="$base_dir\Saltarelle\packages-manual\ScriptSharp\ssc.exe" target="tools"/>
		<file src="$base_dir\Saltarelle\packages-manual\ScriptSharp\sspp.exe" target="tools"/>
		<file src="$base_dir\Saltarelle\SaltarelleLib\install.ps1" target="tools"/>
		<file src="$base_dir\Saltarelle\SaltarelleLib\init.ps1" target="tools"/>
		<file src="$base_dir\Saltarelle\SaltarelleLib\SaltarelleVSModule.psm1" target="tools"/>
		<file src="$base_dir\Saltarelle\SaltarelleLib\uninstall.ps1" target="tools"/>
	</files>
</package>
"@ >"$out_dir\SaltarelleCore.nuspec"

	Exec { & "$buildtools_dir\nuget.exe" pack "$out_dir\SaltarelleCore.nuspec" -OutputDirectory "$out_dir\Publish" }
	rm "$out_dir\SaltarelleCore.nuspec" > $null

@"
<package xmlns="http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd">
	<metadata>
		<id>SaltarelleParser</id>
		<version>$script:version</version>
		<title>Saltarelle Parser</title>
		<description>Saltarelle Parser</description>
		<authors>$authors</authors>
		<dependencies>
			<dependency id="SaltarelleCore" version="[$script:version]" />
		</dependencies>
	</metadata>
	<files>
		<file src="$base_dir\Saltarelle\SaltarelleParser\SaltarelleParser.Client\bin\SaltarelleParser.Client.dll" target="lib"/>
		<file src="$base_dir\Saltarelle\SaltarelleParser\SaltarelleParser.Client\bin\SaltarelleParser.Client.xml" target="lib"/>
		<file src="$base_dir\Saltarelle\SaltarelleParser\SaltarelleParser.Server\bin\SaltarelleParser.dll" target="lib"/>
		<file src="$base_dir\Saltarelle\SaltarelleParser\SaltarelleParser.Server\bin\SaltarelleParser.xml" target="lib"/>
		<file src="$base_dir\Saltarelle\SaltarelleParser\SaltarelleParser.Server\bin\SaltarelleParser.pdb" target="lib"/>
		<file src="$base_dir\Saltarelle\SaltarelleParser\install.ps1" target="tools"/>
	</files>
</package>
"@ >"$out_dir\SaltarelleParser.nuspec"

	Exec { & "$buildtools_dir\nuget.exe" pack "$out_dir\SaltarelleParser.nuspec" -OutputDirectory "$out_dir\Publish" }
	rm "$out_dir\SaltarelleParser.nuspec" > $null

@"
<package xmlns="http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd">
	<metadata>
		<id>SaltarelleUI</id>
		<version>$script:version</version>
		<title>Saltarelle UI</title>
		<description>Saltarelle UI</description>
		<authors>$authors</authors>
		<dependencies>
			<dependency id="SaltarelleCore" version="[$script:version]" />
		</dependencies>
	</metadata>
	<files>
		<file src="$base_dir\Saltarelle\Saltarelle.UI\Saltarelle.UI.Client\bin\Saltarelle.UI.Client.dll" target="lib"/>
		<file src="$base_dir\Saltarelle\Saltarelle.UI\Saltarelle.UI.Client\bin\Saltarelle.UI.Client.xml" target="lib"/>
		<file src="$base_dir\Saltarelle\Saltarelle.UI\Saltarelle.UI.Server\bin\Saltarelle.UI.dll" target="lib"/>
		<file src="$base_dir\Saltarelle\Saltarelle.UI\Saltarelle.UI.Server\bin\Saltarelle.UI.xml" target="lib"/>
		<file src="$base_dir\Saltarelle\Saltarelle.UI\Saltarelle.UI.Server\bin\Saltarelle.UI.pdb" target="lib"/>
		<file src="$base_dir\Saltarelle\Saltarelle.UI\install.ps1" target="tools"/>
	</files>
</package>
"@ >"$out_dir\Saltarelle.UI.nuspec"

	Exec { & "$buildtools_dir\nuget.exe" pack "$out_dir\Saltarelle.UI.nuspec" -OutputDirectory "$out_dir\Publish" }
	rm "$out_dir\Saltarelle.UI.nuspec" > $null

@"
<package xmlns="http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd">
	<metadata>
		<id>SaltarelleMvc</id>
		<version>$script:version</version>
		<title>Saltarelle Mvc Interop</title>
		<description>Saltarelle Mvc Interop</description>
		<authors>$authors</authors>
		<dependencies>
			<dependency id="dotless" version="1.2.2.0" />
			<dependency id="Mono.Cecil" version="0.9.5.2" />
			<dependency id="SaltarelleCore" version="[$script:version]" />
		</dependencies>
	</metadata>
	<files>
		<file src="$base_dir\Saltarelle\Saltarelle.Mvc\bin\Saltarelle.Mvc.dll" target="lib"/>
		<file src="$base_dir\Saltarelle\Saltarelle.Mvc\bin\Saltarelle.Mvc.xml" target="lib"/>
		<file src="$base_dir\Saltarelle\Saltarelle.Mvc\bin\Saltarelle.Mvc.pdb" target="lib"/>
	</files>
</package>
"@ >"$out_dir\SaltarelleMvc.nuspec"

	Exec { & "$buildtools_dir\nuget.exe" pack "$out_dir\SaltarelleMvc.nuspec" -OutputDirectory "$out_dir\Publish" }
	rm "$out_dir\SaltarelleMvc.nuspec" > $null
}

Task Run-Tests {
	$test_assemblies_file = "$base_dir\Saltarelle\TestAssemblies.txt"

	if (Test-Path "$test_assemblies_file") {
		$testasms = @(Get-Content "$test_assemblies_file")
	}
	else {
		$testasms = @()
	}
	
	if ($testasms.Count -ne 0) {
		$runner = (dir "$base_dir\Saltarelle\packages" -Recurse -Filter nunit-console.exe | Select -ExpandProperty FullName)
		Exec { & "$runner" $testasms -nologo -xml "$out_dir\TestResults.xml" }
	}
}

Task Configure -Depends Generate-VersionInfo {
}

Task Determine-Version {
	$refcommit = % {
	(git log --decorate=full --simplify-by-decoration --pretty=oneline HEAD |           # Append items from the log
		Select-String '\(' |                                                            # Only include entries with names
		% { ($_ -replace "^[^(]*\(([^)]*)\).*$","`$1" -replace " ", "").Split(',') } |  # Select only the names, one line per name, delete spaces
		Select-String "^tag:$release_tag_pattern`$" |                                   # Only tags of interest
		% { $_ -replace "^tag:","" }                                                    # Remove the tag: prefix
	) } { git log --reverse --pretty=format:%H | Select-Object -First 1 } |             # Add the oldest commit as a fallback
	Select-Object -First 1
	
	If ($refcommit | Select-String "^$release_tag_pattern`$") {
		$ver = New-Object System.Version(($refcommit -replace "^$release_tag_pattern`$","`$1"))
		If ($ver.Build -eq -1) {
			$ver = New-Object System.Version($ver.Major, $ver.Minor, 0)
		}
	}
	else {
		$ver = New-Object System.Version("0.0.0")
	}
	
	$revision = ((git log "$refcommit..HEAD" --pretty=format:"%H") | Measure-Object).Count # Number of commits since our reference commit
	if ($revision -gt 0) {
		$ver = New-Object System.Version($ver.Major, $ver.Minor, $ver.Build, $revision)
	}
	
	$script:version = $ver.ToString()
	$script:ProductVersion = "$($ver.Major).$($ver.Minor).$($ver.Build)"

	"Version: $script:version"
	"ProductVersion: $script:ProductVersion"
}

Task Generate-VersionInfo -Depends Determine-Version {
@"
[assembly: System.Reflection.AssemblyVersion("$script:version")]
[assembly: System.Reflection.AssemblyFileVersion("$script:version")]
"@ | Out-File "$base_dir\Saltarelle\SaltarelleVersion.cs" -Encoding "UTF8"

@"
<?xml version="1.0" encoding="utf-8"?>
<Include>
	<?define ProductVersion="$script:ProductVersion"?>
	<?define AssemblyVersion="$script:version"?>
</Include>
"@ | Out-File "$base_dir\Saltarelle\VSIntegrationInstaller\Version.wxi" -Encoding UTF8
}
