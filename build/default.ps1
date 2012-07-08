Framework "4.0x86"

properties {
	$base_dir = Resolve-Path ".."
	$buildtools_dir = Resolve-Path "."
	$out_dir = "$(Resolve-Path "".."")\build_out"
	$configuration = "Debug"
	$release_tag_pattern = "release-(.*)"
}

$jsonNetVersion = "4.5.7"

Task default -Depends Build

Task Build -Depends Build-NuGetPackages {
}

Task Clean {
	if (Test-Path $out_dir) {
		rm -Recurse -Force "$out_dir" >$null
	}
	md "$out_dir" >$null
}

Task Build-Solution -Depends Clean, Generate-VersionInfo {
	Exec { msbuild "$base_dir\Saltarelle\Saltarelle.sln" /verbosity:minimal /p:"Configuration=$configuration" }
}

Task Build-NuGetPackages -Depends Determine-Version, Build-Solution, Run-Tests {
	$authors = "Erik Källén"

	If (Test-Path "$out_dir\zip") {
		rm -Recurse -Force "$out_dir\Publish" | Out-Null
	}
	md "$out_dir\Publish" | Out-Null
	
	$dependencyVersion = New-Object System.Version($script:ProductVersion.Major, $script:ProductVersion.Minor)
	
@"
<package xmlns="http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd">
	<metadata>
		<id>Saltarelle.Framework.Client</id>
		<version>$script:FrameworkVersion</version>
		<title>Saltarelle Web Framework Client Library</title>
		<description>Saltarelle Web Framework Client Library</description>
		<authors>$authors</authors>
		<dependencies>
			<dependency id="Saltarelle.Compiler" version="0.0" />
			<dependency id="Saltarelle.Runtime" version="0.0" />
			<dependency id="Saltarelle.Web" version="0.0" />
			<dependency id="Saltarelle.jQuery" version="0.0" />
		</dependencies>
	</metadata>
	<files>
		<file src="$base_dir\Saltarelle\SaltarelleLib\SaltarelleLib.Client\bin\SaltarelleLib.Client.dll" target="lib"/>
		<file src="$base_dir\Saltarelle\SaltarelleLib\SaltarelleLib.Client\bin\SaltarelleLib.Client.xml" target="lib"/>
		<file src="$base_dir\Saltarelle\VSIntegrationInstaller\bin\SaltarelleVSIntegration.msi" target="tools\SaltarelleVSIntegration-$script:ProductVersion.msi"/>
		<file src="$base_dir\Saltarelle\Executables\SalgenTask\bin\Saltarelle.SalgenTask.dll" target="tools"/>
		<file src="$base_dir\Saltarelle\Executables\SalgenTask\bin\Saltarelle.targets" target="tools"/>
		<file src="$base_dir\Saltarelle\Executables\Salgen.exe\bin\salgen.exe" target="tools"/>
		<file src="$base_dir\Saltarelle\SaltarelleLib\install-client.ps1" target="tools\install.ps1"/>
		<file src="$base_dir\Saltarelle\SaltarelleLib\init.ps1" target="tools"/>
		<file src="$base_dir\Saltarelle\SaltarelleLib\SaltarelleVSModule.psm1" target="tools"/>
	</files>
</package>
"@ >"$out_dir\FrameworkClient.nuspec"

@"
<package xmlns="http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd">
	<metadata>
		<id>Saltarelle.Framework.Server</id>
		<version>$script:FrameworkVersion</version>
		<title>Saltarelle Web Framework Server Library</title>
		<description>Saltarelle Web Framework Server Library</description>
		<authors>$authors</authors>
		<dependencies>
			<dependency id="Newtonsoft.Json" version="$jsonNetVersion" />
		</dependencies>
	</metadata>
	<files>
		<file src="$base_dir\Saltarelle\SaltarelleLib\SaltarelleLib.Server\bin\SaltarelleLib.dll" target="lib"/>
		<file src="$base_dir\Saltarelle\SaltarelleLib\SaltarelleLib.Server\bin\SaltarelleLib.xml" target="lib"/>
		<file src="$base_dir\Saltarelle\SaltarelleLib\SaltarelleLib.Server\bin\SaltarelleLib.pdb" target="lib"/>
		<file src="$base_dir\Saltarelle\VSIntegrationInstaller\bin\SaltarelleVSIntegration.msi" target="tools\SaltarelleVSIntegration-$script:ProductVersion.msi"/>
		<file src="$base_dir\Saltarelle\Executables\SalgenTask\bin\Saltarelle.SalgenTask.dll" target="tools"/>
		<file src="$base_dir\Saltarelle\Executables\SalgenTask\bin\Saltarelle.targets" target="tools"/>
		<file src="$base_dir\Saltarelle\Executables\Salgen.exe\bin\salgen.exe" target="tools"/>
		<file src="$base_dir\Saltarelle\SaltarelleLib\install-server.ps1" target="tools\install.ps1"/>
		<file src="$base_dir\Saltarelle\SaltarelleLib\init.ps1" target="tools"/>
		<file src="$base_dir\Saltarelle\SaltarelleLib\SaltarelleVSModule.psm1" target="tools"/>
	</files>
</package>
"@ >"$out_dir\FrameworkServer.nuspec"

@"
<package xmlns="http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd">
	<metadata>
		<id>Saltarelle.Parser.Client</id>
		<version>$script:ParserVersion</version>
		<title>Saltarelle Parser Client Library</title>
		<description>Saltarelle Parser Client Library</description>
		<authors>$authors</authors>
		<dependencies>
			<dependency id="Saltarelle.Framework.Client" version="$dependencyVersion" />
		</dependencies>
	</metadata>
	<files>
		<file src="$base_dir\Saltarelle\SaltarelleParser\SaltarelleParser.Client\bin\SaltarelleParser.Client.dll" target="lib"/>
		<file src="$base_dir\Saltarelle\SaltarelleParser\SaltarelleParser.Client\bin\SaltarelleParser.Client.xml" target="lib"/>
	</files>
</package>
"@ >"$out_dir\ParserClient.nuspec"

@"
<package xmlns="http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd">
	<metadata>
		<id>Saltarelle.Parser.Server</id>
		<version>$script:ParserVersion</version>
		<title>Saltarelle Parser Server Library</title>
		<description>Saltarelle Parser Server Library</description>
		<authors>$authors</authors>
		<dependencies>
			<dependency id="Saltarelle.Framework.Server" version="$dependencyVersion" />
		</dependencies>
	</metadata>
	<files>
		<file src="$base_dir\Saltarelle\SaltarelleParser\SaltarelleParser.Server\bin\SaltarelleParser.dll" target="lib"/>
		<file src="$base_dir\Saltarelle\SaltarelleParser\SaltarelleParser.Server\bin\SaltarelleParser.xml" target="lib"/>
		<file src="$base_dir\Saltarelle\SaltarelleParser\SaltarelleParser.Server\bin\SaltarelleParser.pdb" target="lib"/>
	</files>
</package>
"@ >"$out_dir\ParserServer.nuspec"

@"
<package xmlns="http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd">
	<metadata>
		<id>Saltarelle.UI.Client</id>
		<version>$script:UIVersion</version>
		<title>Saltarelle UI Client Library</title>
		<description>Saltarelle UI Client Library</description>
		<authors>$authors</authors>
		<dependencies>
			<dependency id="Saltarelle.Framework.Client" version="$dependencyVersion" />
		</dependencies>
	</metadata>
	<files>
		<file src="$base_dir\Saltarelle\Saltarelle.UI\Saltarelle.UI.Client\bin\Saltarelle.UI.Client.dll" target="lib"/>
		<file src="$base_dir\Saltarelle\Saltarelle.UI\Saltarelle.UI.Client\bin\Saltarelle.UI.Client.xml" target="lib"/>
	</files>
</package>
"@ >"$out_dir\UIClient.nuspec"

@"
<package xmlns="http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd">
	<metadata>
		<id>Saltarelle.UI.Server</id>
		<version>$script:UIVersion</version>
		<title>Saltarelle UI Server Library</title>
		<description>Saltarelle UI Server Library</description>
		<authors>$authors</authors>
		<dependencies>
			<dependency id="Saltarelle.Framework.Server" version="$dependencyVersion" />
		</dependencies>
	</metadata>
	<files>
		<file src="$base_dir\Saltarelle\Saltarelle.UI\Saltarelle.UI.Server\bin\Saltarelle.UI.dll" target="lib"/>
		<file src="$base_dir\Saltarelle\Saltarelle.UI\Saltarelle.UI.Server\bin\Saltarelle.UI.xml" target="lib"/>
		<file src="$base_dir\Saltarelle\Saltarelle.UI\Saltarelle.UI.Server\bin\Saltarelle.UI.pdb" target="lib"/>
	</files>
</package>
"@ >"$out_dir\UIServer.nuspec"

@"
<package xmlns="http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd">
	<metadata>
		<id>Saltarelle.Mvc</id>
		<version>$script:MvcVersion</version>
		<title>Saltarelle Mvc Libraries</title>
		<description>Saltarelle Mvc Libraries</description>
		<authors>$authors</authors>
		<dependencies>
			<dependency id="dotless" version="1.3.0.3" />
			<dependency id="Mono.Cecil" version="0.9.5.3" />
			<dependency id="Saltarelle.Framework.Server" version="$dependencyVersion" />
		</dependencies>
	</metadata>
	<files>
		<file src="$base_dir\Saltarelle\Saltarelle.Mvc\bin\Saltarelle.Mvc.dll" target="lib"/>
		<file src="$base_dir\Saltarelle\Saltarelle.Mvc\bin\Saltarelle.Mvc.xml" target="lib"/>
		<file src="$base_dir\Saltarelle\Saltarelle.Mvc\bin\Saltarelle.Mvc.pdb" target="lib"/>
	</files>
</package>
"@ >"$out_dir\Mvc.nuspec"

@"
<package xmlns="http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd">
	<metadata>
		<id>Saltarelle.CastleWindsor</id>
		<version>$script:CastleWindsorVersion</version>
		<title>Saltarelle Castle Windsor Bindings</title>
		<description>Saltarelle Castle Windsor Bindings</description>
		<authors>$authors</authors>
		<dependencies>
			<dependency id="Castle.Windsor" version="3.0.0.4001" />
			<dependency id="Saltarelle.Framework.Server" version="$dependencyVersion" />
			<dependency id="Saltarelle.Parser.Server" version="$dependencyVersion" />
		</dependencies>
	</metadata>
	<files>
		<file src="$base_dir\Saltarelle\Saltarelle.CastleWindsor\bin\Saltarelle.CastleWindsor.dll" target="lib"/>
		<file src="$base_dir\Saltarelle\Saltarelle.CastleWindsor\bin\Saltarelle.CastleWindsor.xml" target="lib"/>
		<file src="$base_dir\Saltarelle\Saltarelle.CastleWindsor\bin\Saltarelle.CastleWindsor.pdb" target="lib"/>
	</files>
</package>
"@ >"$out_dir\CastleWindsor.nuspec"

	Exec { & "$buildtools_dir\nuget.exe" pack "$out_dir\FrameworkClient.nuspec" -OutputDirectory "$out_dir\Publish" }
	Exec { & "$buildtools_dir\nuget.exe" pack "$out_dir\FrameworkServer.nuspec" -OutputDirectory "$out_dir\Publish" }
	Exec { & "$buildtools_dir\nuget.exe" pack "$out_dir\ParserClient.nuspec" -OutputDirectory "$out_dir\Publish" }
	Exec { & "$buildtools_dir\nuget.exe" pack "$out_dir\ParserServer.nuspec" -OutputDirectory "$out_dir\Publish" }
	Exec { & "$buildtools_dir\nuget.exe" pack "$out_dir\UIClient.nuspec" -OutputDirectory "$out_dir\Publish" }
	Exec { & "$buildtools_dir\nuget.exe" pack "$out_dir\UIServer.nuspec" -OutputDirectory "$out_dir\Publish" }
	Exec { & "$buildtools_dir\nuget.exe" pack "$out_dir\Mvc.nuspec" -OutputDirectory "$out_dir\Publish" }
	Exec { & "$buildtools_dir\nuget.exe" pack "$out_dir\CastleWindsor.nuspec" -OutputDirectory "$out_dir\Publish" }
}

Task Run-Tests {
	$testasms = dir -Path "$base_dir" -Recurse -Filter "*.Tests.csproj" | % { """$($_.FullName)""" }
	if ($testasms.Count -ne 0) {
		$runner = (dir "$base_dir\Saltarelle\packages" -Recurse -Filter nunit-console.exe | Select -ExpandProperty FullName)
		Exec { & "$runner" $testasms -nologo -xml "$out_dir\TestResults.xml" }
	}
}

Task Configure -Depends Generate-VersionInfo {
}

Function Determine-PathVersion($RefCommit, $RefVersion, $Path) {
	$revision = ((git log "$RefCommit..HEAD" --pretty=format:"%H" -- (@($Path) | % { """$_""" })) | Measure-Object).Count # Number of commits since our reference commit
	if ($revision -gt 0) {
		New-Object System.Version($RefVersion.Major, $RefVersion.Minor, $RefVersion.Build, $revision)
	}
	else {
		$RefVersion
	}
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
		$refVersion = New-Object System.Version(($refcommit -replace "^$release_tag_pattern`$","`$1"))
		If ($refVersion.Build -eq -1) {
			$refVersion = New-Object System.Version($ver.Major, $ver.Minor, 0)
		}
	}
	else {
		$refVersion = New-Object System.Version("0.0.0")
	}
	
	$script:FrameworkVersion = Determine-PathVersion -RefCommit $refCommit -RefVersion $refVersion -Path "$base_dir\Saltarelle\SaltarelleLib","$base_dir\Saltarelle\Executables","$base_dir\Saltarelle\SaltarelleParser","$base_dir\Saltarelle\Saltarelle.CastleWindsor"
	$script:ExecutablesVersion = Determine-PathVersion -RefCommit $refCommit -RefVersion $refVersion -Path "$base_dir\Saltarelle\Executables","$base_dir\Saltarelle\SaltarelleLib","$base_dir\Saltarelle\SaltarelleParser","$base_dir\Saltarelle\Saltarelle.CastleWindsor"
	$script:ParserVersion = Determine-PathVersion -RefCommit $refCommit -RefVersion $refVersion -Path "$base_dir\Saltarelle\SaltarelleParser"
	$script:UIVersion = Determine-PathVersion -RefCommit $refCommit -RefVersion $refVersion -Path "$base_dir\Saltarelle\Saltarelle.UI"
	$script:MvcVersion = Determine-PathVersion -RefCommit $refCommit -RefVersion $refVersion -Path "$base_dir\Saltarelle\Saltarelle.Mvc"
	$script:CastleWindsorVersion = Determine-PathVersion -RefCommit $refCommit -RefVersion $refVersion -Path "$base_dir\Saltarelle\Saltarelle.CastleWindsor"
	$script:ProductVersion = New-Object System.Version($script:ExecutablesVersion.Major, $script:ExecutablesVersion.Minor, $script:ExecutablesVersion.Build)

	"Framework version: $script:FrameworkVersion"
	"Parser version: $script:ParserVersion"
	"Executables version: $script:ExecutablesVersion"
	"UI version: $script:UIVersion"
	"Mvc version: $script:MvcVersion"
	"CastleWindsor version: $script:CastleWindsorVersion"
	"Product version: $script:ProductVersion"
}

Function Generate-VersionFile($Path, $Version) {
@"
[assembly: System.Reflection.AssemblyVersion("$Version")]
[assembly: System.Reflection.AssemblyFileVersion("$Version")]
"@ | Out-File $Path -Encoding "UTF8"
}

Task Generate-VersionInfo -Depends Determine-Version {
	Generate-VersionFile -Path "$base_dir\Saltarelle\SaltarelleLib\SaltarelleLibVersion.cs" -Version $script:FrameworkVersion
	Generate-VersionFile -Path "$base_dir\Saltarelle\SaltarelleParser\SaltarelleParserVersion.cs" -Version $script:ParserVersion
	Generate-VersionFile -Path "$base_dir\Saltarelle\Executables\ExecutablesVersion.cs" -Version $script:ExecutablesVersion
	Generate-VersionFile -Path "$base_dir\Saltarelle\Saltarelle.UI\SaltarelleUIVersion.cs" -Version $script:UIVersion
	Generate-VersionFile -Path "$base_dir\Saltarelle\Saltarelle.Mvc\Properties\SaltarelleMvcVersion.cs" -Version $script:MvcVersion
	Generate-VersionFile -Path "$base_dir\Saltarelle\Saltarelle.CastleWindsor\Properties\SaltarelleCastleWindsorVersion.cs" -Version $script:CastleWindsorVersion

@"
<?xml version="1.0" encoding="utf-8"?>
<Include>
	<?define ProductVersion="$script:ProductVersion"?>
	<?define AssemblyVersion="$script:ExecutablesVersion"?>
</Include>
"@ | Out-File "$base_dir\Saltarelle\VSIntegrationInstaller\Version.wxi" -Encoding UTF8
}
