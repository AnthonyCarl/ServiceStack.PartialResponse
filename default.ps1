Import-Module .\tools\psake.4.2.0.1\tools\psake.psm1

properties {
    $configuration = "Release"
    $rootLocation = get-location
    $srcRoot = "$rootLocation\src"
    $srcIndexTools ="$rootLocation\tools\srcindex"
    $serviceModelNamePart = "ServiceModel"
    $serviceModelProjectBaseName = "ServiceStack.PartialResponse.$serviceModelNamePart"
    $serviceModelCsprojFile = "$srcRoot\$serviceModelNamePart\$serviceModelProjectBaseName\$serviceModelProjectBaseName.csproj"
    $unitTestNamePart = "UnitTests"
    $serviceModelTestDll = "$srcRoot\$serviceModelNamePart\$serviceModelProjectBaseName.$unitTestNamePart\bin\$configuration\$serviceModelProjectBaseName.$unitTestNamePart.dll"
    $ServiceModelSlnFile = "$srcRoot\$serviceModelNamePart\$serviceModelProjectBaseName.sln"
    $framework = "4.0"
    $xunitRunner = ".\tools\xunit.runners.1.9.1\tools\xunit.console.clr4.exe"
    $nugetOutputDir = ".\ReleasePackages"
}

task Default -depends Pack

task Clean {
  msbuild "$ServiceModelSlnFile" /t:Clean /p:Configuration=$configuration
}

task Compile -depends Clean {
  msbuild "$ServiceModelSlnFile" /p:Configuration=$configuration 
}

task Test -depends Compile {
  .$xunitRunner "$serviceModelTestDll"
}

task IndexSrc -depends Test {
  invoke-expression "& '$srcIndexTools\github-sourceindexer.ps1' -symbolsFolder '$srcRoot' -userId anthonycarl -repository ServiceStack.PartialResponse -verbose -branch master -sourcesroot '$srcRoot' -dbgToolsPath '$srcIndexTools'"
}

task Pack -depends Test {
  mkdir -p "$nugetOutputDir" -force
  nuget pack "$serviceModelCsprojFile" -Symbols -Properties Configuration=$configuration -OutputDirectory "$nugetOutputDir"
}

