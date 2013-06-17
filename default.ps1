Import-Module .\packages\psake.4.2.0.1\tools\psake.psm1

properties {
    $configuration = "Release"
    $projectSrcRoot = get-location
    $srcIndexTools = "$projectSrcRoot\tools\srcindex"
    $projectBaseName = "ServiceStack.PartialResponse.ServiceModel"
    $testProjectBaseName = "$projectBaseName.UnitTests"
    $testDll = "$testProjectBaseName\bin\$configuration\$testProjectBaseName.dll"
    $sln_file = ".\$projectBaseName.sln"
    $framework = "4.0"
    $xunitRunner = ".\packages\xunit.runners.1.9.1\tools\xunit.console.clr4.exe"
    $nugetOutputDir = ".\ReleasePackages"
}

task Default -depends Pack

task Clean {
  msbuild "$sln_file" /t:Clean /p:Configuration=$configuration
}

task Compile -depends Clean {
  msbuild "$sln_file" /p:Configuration=$configuration 
}

task Test -depends Compile {
  .$xunitRunner "$testDll"
}

task IndexSrc -depends Test {
  invoke-expression "& '$srcIndexTools\github-sourceindexer.ps1' -symbolsFolder '$projectSrcRoot' -userId anthonycarl -repository ServiceStack.PartialResponse -branch master -sourcesroot '$projectSrcRoot' -verbose -dbgToolsPath '$srcIndexTools'"
}

task Pack -depends IndexSrc {
  mkdir -p "$nugetOutputDir" -force
  nuget pack "$projectBaseName\$projectBaseName.csproj" -Symbols -Properties Configuration=$configuration -OutputDirectory "$nugetOutputDir" 
}

