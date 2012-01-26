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
	$out_zip = "$out_dir\Publish\Saltarelle-$script:version.zip"

	If (Test-Path "$out_zip") {
		rm "$out_zip"
	}
	
	If (Test-Path "$out_dir\zip") {
		rm -Recurse -Force "$out_dir\zip"
	}
	
	md "$out_dir\zip\Server"
	md "$out_dir\zip\Client"
	md "$out_dir\zip\Tools"
	
	copy "$base_dir\Saltarelle\SaltarelleLib\SaltarelleLib.Client\bin\SaltarelleLib.Client.dll" "$out_dir\zip\Client"
	copy "$base_dir\Saltarelle\SaltarelleLib\SaltarelleLib.Client\bin\SaltarelleLib.Client.xml" "$out_dir\zip\Client"
	copy "$base_dir\Saltarelle\SaltarelleParser\SaltarelleParser.Client\bin\SaltarelleParser.Client.dll" "$out_dir\zip\Client"
	copy "$base_dir\Saltarelle\SaltarelleParser\SaltarelleParser.Client\bin\SaltarelleParser.Client.xml" "$out_dir\zip\Client"
	copy "$base_dir\Saltarelle\Saltarelle.UI\Saltarelle.UI.Client\bin\Saltarelle.UI.Client.dll" "$out_dir\zip\Client"
	copy "$base_dir\Saltarelle\Saltarelle.UI\Saltarelle.UI.Client\bin\Saltarelle.UI.Client.xml" "$out_dir\zip\Client"

	copy "$base_dir\Saltarelle\SaltarelleLib\SaltarelleLib.Server\bin\SaltarelleLib.dll" "$out_dir\zip\Server"
	copy "$base_dir\Saltarelle\SaltarelleLib\SaltarelleLib.Server\bin\SaltarelleLib.xml" "$out_dir\zip\Server"
	copy "$base_dir\Saltarelle\SaltarelleLib\SaltarelleLib.Server\bin\SaltarelleLib.pdb" "$out_dir\zip\Server"
	copy "$base_dir\Saltarelle\SaltarelleParser\SaltarelleParser.Server\bin\SaltarelleParser.dll" "$out_dir\zip\Server"
	copy "$base_dir\Saltarelle\SaltarelleParser\SaltarelleParser.Server\bin\SaltarelleParser.xml" "$out_dir\zip\Server"
	copy "$base_dir\Saltarelle\SaltarelleParser\SaltarelleParser.Server\bin\SaltarelleParser.pdb" "$out_dir\zip\Server"
	copy "$base_dir\Saltarelle\Saltarelle.UI\Saltarelle.UI.Server\bin\Saltarelle.UI.dll" "$out_dir\zip\Server"
	copy "$base_dir\Saltarelle\Saltarelle.UI\Saltarelle.UI.Server\bin\Saltarelle.UI.xml" "$out_dir\zip\Server"
	copy "$base_dir\Saltarelle\Saltarelle.UI\Saltarelle.UI.Server\bin\Saltarelle.UI.pdb" "$out_dir\zip\Server"
	copy "$base_dir\Saltarelle\Saltarelle.Mvc\bin\Saltarelle.Mvc.dll" "$out_dir\zip\Server"
	copy "$base_dir\Saltarelle\Saltarelle.Mvc\bin\Saltarelle.Mvc.xml" "$out_dir\zip\Server"
	copy "$base_dir\Saltarelle\Saltarelle.Mvc\bin\Saltarelle.Mvc.pdb" "$out_dir\zip\Server"

	copy "$base_dir\Saltarelle\VSIntegrationInstaller\bin\SaltarelleVSIntegration.msi" "$out_dir\zip\Tools\SaltarelleVSIntegration-$script:ProductVersion.msi"

	Exec { & "$buildtools_dir\7z.exe" a -y -bd -r -tzip "$out_zip" "$out_dir\zip\Client" "$out_dir\zip\Server" "$out_dir\zip\Tools" }
}

Task Publish-Nupkg -Depends Determine-Version, Build, Run-Tests {
	$authors = "Erik Källén"
	
	$targetsContent = [xml](Get-Content "$base_dir\Saltarelle\Executables\SalgenTask\Saltarelle.targets")
	$x = Select-Xml -Xml $targetsContent -Xpath "//x:UsingTask" -Namespace @{ x = "http://schemas.microsoft.com/developer/msbuild/2003" }
	$x.Node.RemoveAttribute("AssemblyName") > $null
	$x.Node.SetAttribute("AssemblyFile", "Saltarelle.SalgenTask.dll") > $null
	
	$targetsContent.Save("$out_dir\Saltarelle.targets") > $null
	
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
		<file src="$out_dir\Saltarelle.targets" target="tools"/>
		<file src="$base_dir\Saltarelle\Executables\Salgen.exe\bin\salgen.exe" target="tools"/>
		<file src="$base_dir\Saltarelle\packages-manual\ScriptSharp\sscorlib.dll" target="lib"/>
		<file src="$base_dir\Saltarelle\packages-manual\ScriptSharp\nStuff.ScriptSharp.dll" target="tools"/>
		<file src="$base_dir\Saltarelle\packages-manual\ScriptSharp\nStuff.ScriptSharp.targets" target="tools"/>
		<file src="$base_dir\Saltarelle\packages-manual\ScriptSharp\ssc.exe" target="tools"/>
		<file src="$base_dir\Saltarelle\packages-manual\ScriptSharp\sspp.exe" target="tools"/>
		<file src="$base_dir\Saltarelle\NuGet\InstallCore.ps1" target="tools\install.ps1"/>
	</files>
</package>
"@ >"$out_dir\SaltarelleCore.nuspec"

	Exec { & "$buildtools_dir\nuget.exe" pack "$out_dir\SaltarelleCore.nuspec" -OutputDirectory "$out_dir\Publish" }
	rm "$out_dir\SaltarelleCore.nuspec" > $null
	rm "$out_dir\Saltarelle.targets" > $null

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
"@ | Out-File "$base_dir\Saltarelle\VSIntegrationInstaller\Version.wxi" -Encoding UTF8
}
