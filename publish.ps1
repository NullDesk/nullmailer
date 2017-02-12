<#
.SYNOPSIS
    Publish script to increment versions for releases or hotfixes.

.DESCRIPTION
    Script will increment either the minor or patch version for all projects under /src 

.PARAMETER mode 
    Specify 'release' or 'hotfix'

.EXAMPLE 
    publish.ps1 release

#>

[cmdletbinding()]
param([string]$mode)


if($mode -ne "release" -and $mode -ne "hotfix"){
    Write-Output "Syntax:  publish.ps1 [[-mode] <String>]"
    Write-Output "Please specify a mode; 'release' or 'hotfix'."
    Write-Output "Get-Help ./publish.ps1 for more info"
    exit 0;
}
Write-Output "mode = $mode"



$srcDir = Get-ChildItem ./src


foreach ($folder in $srcDir) {
    $p = Join-Path -Path $folder.FullName -ChildPath '*.csproj';
    # only src project folders -> folders with a csproj file 
    if (Test-Path $p -PathType Leaf) {
        $projectFolders += $folder.FullName
        $projFile =  Get-ChildItem -Path $p | Select-Object -last 1
        $proj = [xml](get-content $projFile)
        $proj.GetElementsByTagName("VersionPrefix") | ForEach-Object{
            $origVer = $_."#text"
            $verArray = $_."#text".Split(".")
            $majorInt = [convert]::ToInt32( $verArray[0], 10)
            $minorInt = [convert]::ToInt32( $verArray[1], 10)
            $patchInt = [convert]::ToInt32( $verArray[2], 10)
            switch($mode) {
                "release"{
                    $minorInt = $minorInt + 1
                    $patchInt = 0
                }
                "hotfix"{
                    $patchInt = $patchInt + 1
                }
            }
            $buildVersion =  $majorInt.ToString() + "." + $minorInt.ToString() + "." + $patchInt.ToString()
            $_."#text" = $buildVersion
            Write-Output "Incrementing version for: $($projFile.Name)"
            Write-Output "    $origVer --> $buildVersion"
            
           $proj.Save($projFile)
        }
    }


}