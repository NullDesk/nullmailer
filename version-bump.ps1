<#
.SYNOPSIS
    Increment .NET *.csproj file version.

.DESCRIPTION
    Script will increment either the minor or patch version for all projects under /src.

.PARAMETER mode
    Specify 'minor' or 'patch'

.PARAMETER beta
    Specify -beta switch to increment to a beta version

.EXAMPLE
    publish.ps1 -mode release -beta false

#>

[cmdletbinding()]
param([Parameter(Mandatory)][string]$mode, [switch]$beta = $false)

if($mode -ne "minor" -and $mode -ne "patch" ){
    Write-Output "Syntax:  publish.ps1 [[-mode] <String>]"
    Write-Output "Please specify a mode; 'minor' or 'patch'."
    Write-Output "Get-Help ./publish.ps1 for more info"
    exit 0;
}

Write-Output "mode = $mode"

Write-Output "beta = $beta"

$save = $true;

$srcDir = Get-ChildItem ./src

foreach ($folder in $srcDir) {
    $p = Join-Path -Path $folder.FullName -ChildPath '*.csproj';
    $majorInt = 0
    $minorInt = 0
    $patchInt = 0
    $betaInt = 0
    $origVer = $null

    # only src project folders -> folders with a csproj file
    if (Test-Path $p -PathType Leaf)
    {
        $projectFolders += $folder.FullName
        $projFile =  Get-ChildItem -Path $p | Select-Object -last 1
        $proj = [xml](get-content $projFile)
        $node = $proj.SelectSingleNode("Project/PropertyGroup/Version");
        if($node -ne $null)
        {
            $pattern = "^(?<complete>(?<version>(?<major>[0-9]+)\.(?<minor>[0-9]+)\.(?<patch>[0-9]+)(?:-(?<prerelease>[0-9a-zA-Z-]+(?:\.[0-9a-zA-Z-]+)*))?)(?:\+(?<meta>[0-9a-zA-Z-]+(?:\.[0-9a-zA-Z-]+)*))?)$"
            if($node."#text" -match $pattern)
            {
                $origVer = $Matches.complete;
                $majorInt = $Matches.major -as [int]
                $minorInt = $Matches.minor -as [int]
                $patchInt = $Matches.patch -as [int]

                if($Matches.prerelease -ne $null)
                {
                    $l = $Matches.prerelease.lastIndexOf('.');
                    if($l -gt 0){
                       $betaInt =  $Matches.prerelease.Substring($l + 1) -as [int];
                    }
                    else{
                        $betaInt = 0
                    }
                }

                switch($mode)
                {
                    "minor"
                    {
                        if($betaInt -eq 0)
                        {
                            $minorInt = $minorInt + 1
                            $patchInt = 0
                        }
                        elseif($beta -eq $false){
                            $minorInt = $minorInt + 1
                            $patchInt = 0
                        }
                    }
                    "patch"
                    {
                        if($betaInt -eq 0)
                        {
                            $patchInt = $patchInt + 1
                        }
                    }
                }
                if($beta)
                {
                    $betaInt = $betaInt + 1
                }
                if($save)
                {
                    $buildVersion =  $majorInt.ToString() + "." + $minorInt.ToString() + "." + $patchInt.ToString()
                    if($beta){
                        $betaString = "-beta.{0}" -f [convert]::ToInt32($betaInt, 10)

                        $buildVersion = $buildVersion + $betaString
                    }
                    if($buildVersion -match $pattern){

                        $node."#text" = $buildVersion
                        Write-Output "Incrementing version for: $($projFile.Name)"
                        Write-Output "    $origVer --> $buildVersion"

                        $proj.Save($projFile)
                    }
                    else{
                        Write-Output "Failure: Buildversion $buildVersion is not a valid semver"
                    }
                }
            }
        }
    }
}