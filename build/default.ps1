Framework "4.0x86"

properties {
	$baseDir = Resolve-Path ".."
	$buildtoolsDir = Resolve-Path "."
	$outDir = "$(Resolve-Path "".."")\build_out"
	$configuration = "Debug"
	$releaseTagPattern = "release-(.*)"
	$compilerVersion = "1.2"
	$jsonNetVersion = "4.5.8"
	$autoVersion = $true
	$skipTests = $false
}

Function Parse-Version($Version) {
	Return New-Object System.Version(($Version -Replace "-.*$","")) # Remove any pre-release information
}

Task default -Depends Build

Task Build -Depends Build-NuGetPackages {
}

Task Clean {
	if (Test-Path $outDir) {
		rm -Recurse -Force "$outDir" >$null
	}
	md "$outDir" >$null
}

Task Build-Solution -Depends Clean, Generate-VersionInfo {
	Exec { msbuild "$baseDir\Saltarelle\Saltarelle.sln" /verbosity:minimal /p:"Configuration=$configuration" }
}

Task Build-NuGetPackages -Depends Determine-Version, Build-Solution, Run-Tests {
	$authors = "Erik Källén"

	If (Test-Path "$outDir\zip") {
		rm -Recurse -Force "$outDir\Publish" | Out-Null
	}
	md "$outDir\Publish" | Out-Null
	
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
			<dependency id="Saltarelle.Compiler" version="$compilerVersion" />
			<dependency id="Saltarelle.Runtime" version="$compilerVersion" />
			<dependency id="Saltarelle.Web" version="$compilerVersion" />
			<dependency id="Saltarelle.jQuery" version="$compilerVersion" />
		</dependencies>
	</metadata>
	<files>
		<file src="$baseDir\Saltarelle\SaltarelleLib\SaltarelleLib.Client\bin\SaltarelleLib.Client.dll" target="lib"/>
		<file src="$baseDir\Saltarelle\SaltarelleLib\SaltarelleLib.Client\bin\SaltarelleLib.Client.xml" target="lib"/>
		<file src="$baseDir\Saltarelle\VSIntegrationInstaller\bin\SaltarelleVSIntegration.msi" target="tools\SaltarelleVSIntegration-$script:ProductVersion.msi"/>
		<file src="$baseDir\Saltarelle\Executables\SalgenTask\bin\Saltarelle.SalgenTask.dll" target="tools"/>
		<file src="$baseDir\Saltarelle\Executables\SalgenTask\bin\Saltarelle.targets" target="tools"/>
		<file src="$baseDir\Saltarelle\Executables\Salgen.exe\bin\salgen.exe" target="tools"/>
		<file src="$baseDir\Saltarelle\SaltarelleLib\install-client.ps1" target="tools\install.ps1"/>
		<file src="$baseDir\Saltarelle\SaltarelleLib\init.ps1" target="tools"/>
		<file src="$baseDir\Saltarelle\SaltarelleLib\SaltarelleVSModule.psm1" target="tools"/>
	</files>
</package>
"@ >"$outDir\FrameworkClient.nuspec"

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
		<file src="$baseDir\Saltarelle\SaltarelleLib\SaltarelleLib.Server\bin\SaltarelleLib.dll" target="lib"/>
		<file src="$baseDir\Saltarelle\SaltarelleLib\SaltarelleLib.Server\bin\SaltarelleLib.xml" target="lib"/>
		<file src="$baseDir\Saltarelle\SaltarelleLib\SaltarelleLib.Server\bin\SaltarelleLib.pdb" target="lib"/>
		<file src="$baseDir\Saltarelle\SaltarelleLib\install-server.ps1" target="tools\install.ps1"/>
		<file src="$baseDir\Saltarelle\SaltarelleLib\init.ps1" target="tools"/>
		<file src="$baseDir\Saltarelle\SaltarelleLib\SaltarelleVSModule.psm1" target="tools"/>
	</files>
</package>
"@ >"$outDir\FrameworkServer.nuspec"

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
		<file src="$baseDir\Saltarelle\SaltarelleParser\SaltarelleParser.Client\bin\SaltarelleParser.Client.dll" target="lib"/>
		<file src="$baseDir\Saltarelle\SaltarelleParser\SaltarelleParser.Client\bin\SaltarelleParser.Client.xml" target="lib"/>
	</files>
</package>
"@ >"$outDir\ParserClient.nuspec"

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
		<file src="$baseDir\Saltarelle\SaltarelleParser\SaltarelleParser.Server\bin\SaltarelleParser.dll" target="lib"/>
		<file src="$baseDir\Saltarelle\SaltarelleParser\SaltarelleParser.Server\bin\SaltarelleParser.xml" target="lib"/>
		<file src="$baseDir\Saltarelle\SaltarelleParser\SaltarelleParser.Server\bin\SaltarelleParser.pdb" target="lib"/>
	</files>
</package>
"@ >"$outDir\ParserServer.nuspec"

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
		<file src="$baseDir\Saltarelle\Saltarelle.UI\Saltarelle.UI.Client\bin\Saltarelle.UI.Client.dll" target="lib"/>
		<file src="$baseDir\Saltarelle\Saltarelle.UI\Saltarelle.UI.Client\bin\Saltarelle.UI.Client.xml" target="lib"/>
	</files>
</package>
"@ >"$outDir\UIClient.nuspec"

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
		<file src="$baseDir\Saltarelle\Saltarelle.UI\Saltarelle.UI.Server\bin\Saltarelle.UI.dll" target="lib"/>
		<file src="$baseDir\Saltarelle\Saltarelle.UI\Saltarelle.UI.Server\bin\Saltarelle.UI.xml" target="lib"/>
		<file src="$baseDir\Saltarelle\Saltarelle.UI\Saltarelle.UI.Server\bin\Saltarelle.UI.pdb" target="lib"/>
	</files>
</package>
"@ >"$outDir\UIServer.nuspec"

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
		<file src="$baseDir\Saltarelle\Saltarelle.Mvc\bin\Saltarelle.Mvc.dll" target="lib"/>
		<file src="$baseDir\Saltarelle\Saltarelle.Mvc\bin\Saltarelle.Mvc.xml" target="lib"/>
		<file src="$baseDir\Saltarelle\Saltarelle.Mvc\bin\Saltarelle.Mvc.pdb" target="lib"/>
	</files>
</package>
"@ >"$outDir\Mvc.nuspec"

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
		<file src="$baseDir\Saltarelle\Saltarelle.CastleWindsor\bin\Saltarelle.CastleWindsor.dll" target="lib"/>
		<file src="$baseDir\Saltarelle\Saltarelle.CastleWindsor\bin\Saltarelle.CastleWindsor.xml" target="lib"/>
		<file src="$baseDir\Saltarelle\Saltarelle.CastleWindsor\bin\Saltarelle.CastleWindsor.pdb" target="lib"/>
	</files>
</package>
"@ >"$outDir\CastleWindsor.nuspec"

	Exec { & "$buildtoolsDir\nuget.exe" pack "$outDir\FrameworkClient.nuspec" -OutputDirectory "$outDir\Publish" }
	Exec { & "$buildtoolsDir\nuget.exe" pack "$outDir\FrameworkServer.nuspec" -OutputDirectory "$outDir\Publish" }
	Exec { & "$buildtoolsDir\nuget.exe" pack "$outDir\ParserClient.nuspec" -OutputDirectory "$outDir\Publish" }
	Exec { & "$buildtoolsDir\nuget.exe" pack "$outDir\ParserServer.nuspec" -OutputDirectory "$outDir\Publish" }
	Exec { & "$buildtoolsDir\nuget.exe" pack "$outDir\UIClient.nuspec" -OutputDirectory "$outDir\Publish" }
	Exec { & "$buildtoolsDir\nuget.exe" pack "$outDir\UIServer.nuspec" -OutputDirectory "$outDir\Publish" }
	Exec { & "$buildtoolsDir\nuget.exe" pack "$outDir\Mvc.nuspec" -OutputDirectory "$outDir\Publish" }
	Exec { & "$buildtoolsDir\nuget.exe" pack "$outDir\CastleWindsor.nuspec" -OutputDirectory "$outDir\Publish" }
}

Task Run-Tests {
	If (-not $skipTests) {
		$testasms = dir -Path "$baseDir" -Recurse -Filter "*.Tests.csproj" | % { """$($_.FullName)""" }
		if ($testasms.Count -ne 0) {
			$runner = (dir "$baseDir\Saltarelle\packages" -Recurse -Filter nunit-console.exe | Select -ExpandProperty FullName)
			Exec { & "$runner" $testasms -nologo -xml "$outDir\TestResults.xml" }
		}
	}
}

Task Configure -Depends Generate-VersionInfo {
}

Function Determine-PathVersion($RefCommit, $RefVersion, $Path) {
	if ($autoVersion) {
		$RefVersion = New-Object System.Version(($RefVersion -Replace "-.*$",""))
		if ($RefVersion.Build -lt 0) {
			$RefVersion = New-Object System.Version($RefVersion.Major, $RefVersion.Minor, 0)
		}
	
		$revision = ((git log "$RefCommit..HEAD" --pretty=format:"%H" -- (@($Path) | % { """$_""" })) | Measure-Object).Count # Number of commits since our reference commit
		if ($revision -gt 0) {
			Return New-Object System.Version($RefVersion.Major, $RefVersion.Minor, $RefVersion.Build, $revision)
		}
	}

	Return $RefVersion
}

Function Determine-Ref {
	$refcommit = % {
	(git log --decorate=full --simplify-by-decoration --pretty=oneline HEAD |           # Append items from the log
		Select-String '\(' |                                                            # Only include entries with names
		% { ($_ -replace "^[^(]*\(([^)]*)\).*$","`$1" -replace " ", "").Split(',') } |  # Select only the names, one line per name, delete spaces
		Select-String "^tag:$releaseTagPattern`$" |                                   # Only tags of interest
		% { $_ -replace "^tag:","" }                                                    # Remove the tag: prefix
	) } { git log --reverse --pretty=format:%H | Select-Object -First 1 } |             # Add the oldest commit as a fallback
	Select-Object -First 1
	
	If ($refcommit | Select-String "^$releaseTagPattern`$") {
		$refVersion = $refcommit -replace "^$releaseTagPattern`$","`$1"
	}
	else {
		$refVersion = "0.0.0"
	}

	Return ($refcommit, $refVersion)
}

Task Determine-Version {
	if (-not $autoVersion) {
		if ((git log -1 --decorate=full --simplify-by-decoration --pretty=oneline HEAD |
			 Select-String '\(' |
			 % { ($_ -replace "^[^(]*\(([^)]*)\).*$","`$1" -replace " ", "").Split(',') } |
			 Select-String "^tag:$releaseTagPattern`$" |
			 % { $_ -replace "^tag:","" } |
			 Measure-Object
			).Count -eq 0) {
			
			Throw "The most recent commit must be tagged when not using auto-versioning"
		}
	}
	$refs = Determine-Ref

	$script:FrameworkVersion = Determine-PathVersion -RefCommit $refs[0] -RefVersion $refs[1] -Path "$baseDir\Saltarelle\SaltarelleLib","$baseDir\Saltarelle\Executables","$baseDir\Saltarelle\SaltarelleParser","$baseDir\Saltarelle\Saltarelle.CastleWindsor"
	$script:ExecutablesVersion = Determine-PathVersion -RefCommit $refs[0] -RefVersion $refs[1] -Path "$baseDir\Saltarelle\Executables","$baseDir\Saltarelle\SaltarelleLib","$baseDir\Saltarelle\SaltarelleParser","$baseDir\Saltarelle\Saltarelle.CastleWindsor"
	$script:ParserVersion = Determine-PathVersion -RefCommit $refs[0] -RefVersion $refs[1] -Path "$baseDir\Saltarelle\SaltarelleParser"
	$script:UIVersion = Determine-PathVersion -RefCommit $refs[0] -RefVersion $refs[1] -Path "$baseDir\Saltarelle\Saltarelle.UI"
	$script:MvcVersion = Determine-PathVersion -RefCommit $refs[0] -RefVersion $refs[1] -Path "$baseDir\Saltarelle\Saltarelle.Mvc","$baseDir\Saltarelle\packages"
	$script:CastleWindsorVersion = Determine-PathVersion -RefCommit $refs[0] -RefVersion $refs[1] -Path "$baseDir\Saltarelle\Saltarelle.CastleWindsor"
	$fullProductVersion = Parse-Version($script:ExecutablesVersion)
	$script:ProductVersion = "$($fullProductVersion.Major).$($fullProductVersion.Minor).$($fullProductVersion.Build)"

	"Framework version: $script:FrameworkVersion"
	"Parser version: $script:ParserVersion"
	"Executables version: $script:ExecutablesVersion"
	"UI version: $script:UIVersion"
	"Mvc version: $script:MvcVersion"
	"CastleWindsor version: $script:CastleWindsorVersion"
	"Product version: $script:ProductVersion"
}

Function Generate-VersionFile($Path, $Version) {
	$netVer = Parse-Version($Version)

@"
[assembly: System.Reflection.AssemblyVersion("$($netVer.Major).$($netVer.Minor)")]
[assembly: System.Reflection.AssemblyFileVersion("$netVer")]
"@ | Out-File $Path -Encoding "UTF8"
}

Task Generate-VersionInfo -Depends Determine-Version {
	Generate-VersionFile -Path "$baseDir\Saltarelle\SaltarelleLib\SaltarelleLibVersion.cs" -Version $script:FrameworkVersion
	Generate-VersionFile -Path "$baseDir\Saltarelle\SaltarelleParser\SaltarelleParserVersion.cs" -Version $script:ParserVersion
	Generate-VersionFile -Path "$baseDir\Saltarelle\Executables\ExecutablesVersion.cs" -Version $script:ExecutablesVersion
	Generate-VersionFile -Path "$baseDir\Saltarelle\Saltarelle.UI\SaltarelleUIVersion.cs" -Version $script:UIVersion
	Generate-VersionFile -Path "$baseDir\Saltarelle\Saltarelle.Mvc\Properties\SaltarelleMvcVersion.cs" -Version $script:MvcVersion
	Generate-VersionFile -Path "$baseDir\Saltarelle\Saltarelle.CastleWindsor\Properties\SaltarelleCastleWindsorVersion.cs" -Version $script:CastleWindsorVersion

@"
<?xml version="1.0" encoding="utf-8"?>
<Include>
	<?define ProductVersion="$script:ProductVersion"?>
	<?define AssemblyVersion="$script:ExecutablesVersion"?>
</Include>
"@ | Out-File "$baseDir\Saltarelle\VSIntegrationInstaller\Version.wxi" -Encoding UTF8
}
