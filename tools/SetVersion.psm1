# SetVersion.ps1
#
# Set the version in all the AssemblyInfo.cs or AssemblyInfo.vb files in any subdirectory.
#
# usage:  
#  from cmd.exe: 
#     powershell.exe SetVersion.ps1  2.8.3.0
# 
#  from powershell.exe prompt: 
#     .\SetVersion.ps1  2.8.3.0
#
# last saved Time-stamp: <Wednesday, April 23, 2008  11:46:40  (by dinoch)>
#


function Usage
{
  echo "Usage: ";
  echo "  from cmd.exe: ";
  echo "     powershell.exe SetVersion.ps1  2.8.3.0";
  echo " ";
  echo "  from powershell.exe prompt: ";
  echo "     .\SetVersion.ps1  2.8.3.0";
  echo " ";
}


function Update-SourceVersion
{
  Param ([string]$Version)
  $NewVersion = 'AssemblyVersion("' + $Version + '")';
  $FileVersion = $Version;
  if($FileVersion.EndsWith(".*"))
  {
     $FileVersion = $FileVersion.Replace(".*", "")
  }

  $NewFileVersion = 'AssemblyFileVersion("' + $FileVersion + '")';

  foreach ($o in $input) 
  {
    Write-output $o.FullName
    $TmpFile = $o.FullName + ".tmp"

    $NewContents = get-content $o.FullName | 
        %{$_ -replace 'AssemblyVersion\("[0-9]+(\.([0-9]+|\*)){1,3}"\)', $NewVersion } |
        %{$_ -replace 'AssemblyFileVersion\("[0-9]+(\.([0-9]+|\*)){1,3}"\)', $NewFileVersion } # > $TmpFile

    $NewContents | out-file $o.FullName
    #move-item $TmpFile $o.FullName -force
  }
}


function Update-AllAssemblyInfoFiles ( $version )
{
  foreach ($file in "AssemblyInfo.cs", "AssemblyInfo.vb" ) 
  {
    get-childitem -recurse |? {$_.Name -eq $file} | Update-SourceVersion $version ;
  }
}

function Set-Version
{
	Param ([string]$Version)
	# validate arguments 
	$r= [System.Text.RegularExpressions.Regex]::Match($Version, "^[0-9]+(\.[0-9]+){1,3}(\.\*)?$");
	
	if ($r.Success)
	{
	  Update-AllAssemblyInfoFiles $Version;
	}
	else
	{
	  echo " ";
	  echo "Bad Input!";
	  echo " ";
	  Usage ;

          Write-Error "Failed To Set Version"
	}
}

export-modulemember -function Set-Version -variable Version
