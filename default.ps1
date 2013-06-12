Import-Module .\packages\psake.4.2.0.1\tools\psake.psm1

properties {
    # $home = $psake.build_script_dir + "/../.."
    $configuration = "Release"
    $projectBaseName = "ServiceStack.PartialResponse.ServiceModel"
    $testProjectBaseName = "$projectBaseName.UnitTests"
    $testDll = "$testProjectBaseName\bin\$configuration\$testProjectBaseName.dll"
    $sln_file = ".\$projectBaseName.sln"
    $framework = "4.0"
    $xunitRunner = ".\packages\xunit.runners.1.9.1\tools\xunit.console.clr4.exe"
    $nugetOutputDir = ".\ReleasePackages"
}

task Default -depends Pack

task Compile -depends Clean {
  msbuild "$sln_file" /p:Configuration=$configuration 
}

task Clean {
  msbuild "$sln_file" /t:Clean /p:Configuration=$configuration
}

task Test -depends Compile {
  .$xunitRunner "$testDll"
}

task Pack -depends Test {
  mkdir -p "$nugetOutputDir" -force
  nuget pack "$projectBaseName\$projectBaseName.csproj" -Symbols -Properties Configuration=$configuration -OutputDirectory "$nugetOutputDir" 
}

