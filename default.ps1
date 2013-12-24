if((Get-Module | Where-Object {$_.Name -eq "psake"}) -eq $null) 
    { 
        Write-Host "psake module not found, importing it" 
        $scriptPath = Split-Path $MyInvocation.InvocationName 
        Import-Module .\tools\psake.4.2.0.1\tools\psake.psm1
    } 
Import-Module .\tools\SetVersion.psm1

function Get-VersionNumber
{
  $completeVersionNumber = ""

  $buildNumber = $Env:BUILD_NUMBER
  
  if([string]::IsNullOrEmpty($buildNumber))
  {
    $completeVersionNumber = $majorMinorVersion + ".*"
  }
  else
  {
    #running in TeamCity
    $completeVersionNumber = $majorMinorVersion + "." + $buildNumber
  }

  return ,$completeVersionNumber
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
	$serviceModelNuspecFile ="$srcRoot\$serviceModelNamePart\$serviceModelProjectBaseName\$serviceModelProjectBaseName.nuspec"
    $framework = "4.0"
    $xunitRunner = ".\tools\xunit.runners.1.9.1\tools\xunit.console.clr4.exe"
    $nugetOutputDir = ".\ReleasePackages"
    $nugetExe = "$rootLocation\tools\nuget\nuget.exe"
    $versionFile = ".\MajorMinorVersion.txt"
    $majorMinorVersion = Get-Content $versionFile
    $completeVersionNumber = Get-VersionNumber
	$gitHubRepoUrl = "https://github.com/AnthonyCarl/ServiceStack.PartialResponse"
    $versionSwitch = ""
    
    if(!$completeVersionNumber.EndsWith(".*"))
    {
      #running in TeamCity
      $versionSwitch = "-Version $completeVersionNumber"
      Write-Host "##teamcity[buildNumber '$completeVersionNumber']"
    }
}

task Default -depends BuildAll

task SetVersionServiceModel {
  Write-Host "Setting version to $completeVersionNumber"
  Set-Version $completeVersionNumber
}

task CleanServiceModel -depends SetVersionServiceModel {
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

task SetReleaseNotesServiceModel -depends TestServiceModel {
  $releaseNotesText = $Env:ReleaseNotes
  $vcsNumber = $Env:BUILD_VCS_NUMBER

  if(![string]::IsNullOrEmpty($vcsNumber))
  {
    Write-Host "Found VCS number: $vcsNumber"
    $releaseNotesText += [System.Environment]::NewLine + [System.Environment]::NewLine + "Includes changes up to and including: $gitHubRepoUrl/commit/$vcsNumber" + [System.Environment]::NewLine
  }
  else
  {
    Write-Host "No VCS number found."
  }

  if(![string]::IsNullOrEmpty($releaseNotesText))
  {
    Write-Host "Setting release notes to:"
    Write-Host "$releaseNotesText"

    $nuspecContents = [Xml](Get-Content "$serviceModelNuspecFile")
    $releaseNotesNode = $nuspecContents.package.metadata.SelectSingleNode("releaseNotes")
    if($releaseNotesNode -eq $null)
    {
      $releaseNotesNode = $nuspecContents.CreateElement('releaseNotes')
      $ignore = $nuspecContents.package.metadata.AppendChild($releaseNotesNode)
    }

    $ignore = $releaseNotesNode.InnerText = $releaseNotesText
    $nuspecContents.Save("$serviceModelNuspecFile")
  }
  else
  {
    Write-Host "No release notes added."
  }
}

task PackServiceModel -depends SetReleaseNotesServiceModel {
  mkdir -path "$nugetOutputDir" -force
  invoke-expression "& '$nugetExe' pack '$serviceModelCsprojFile' -Symbols -Properties Configuration=$configuration -OutputDirectory '$nugetOutputDir'"
}

task BuildAll -Depends PackServiceModel
