if((Get-Module | Where-Object {$_.Name -eq "psake"}) -eq $null) 
    { 
        Write-Host "psake module not found, importing it" 
        $scriptPath = Split-Path $MyInvocation.InvocationName 
        Import-Module .\tools\psake.4.2.0.1\tools\psake.psm1
    } 

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
    $nugetExe = "$rootLocation\tools\nuget\nuget.exe"
}

task Default -depends BuildAll

task CleanServiceModel {
  write-host $myParameter
  msbuild "$ServiceModelSlnFile" /t:Clean /p:Configuration=$configuration
}

task CompileServiceModel -depends CleanServiceModel {
  msbuild "$ServiceModelSlnFile" /p:Configuration=$configuration 
}

task TestServiceModel -depends CompileServiceModel {
  .$xunitRunner "$serviceModelTestDll"
}

task IndexSrcServiceModel -depends TestServiceModel {
  invoke-expression "& '$srcIndexTools\github-sourceindexer.ps1' -symbolsFolder '$srcRoot' -userId anthonycarl -repository ServiceStack.PartialResponse -verbose -branch master -sourcesroot '$srcRoot' -dbgToolsPath '$srcIndexTools'"
}

task PackServiceModel -depends TestServiceModel {
  mkdir -p "$nugetOutputDir" -force
  invoke-expression "& '$nugetExe' pack '$serviceModelCsprojFile' -Symbols -Properties Configuration=$configuration -OutputDirectory '$nugetOutputDir'"
}

task Hello {
  Write-Host "Hello"
}

task BuildAll -Depends PackServiceModel
