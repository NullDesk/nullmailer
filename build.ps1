<#  
.SYNOPSIS
    You can add this to you build script to ensure that psbuild is available before calling
    Invoke-MSBuild. If psbuild is not available locally it will be downloaded automatically.
#>
function EnsurePsbuildInstalled{  
    [cmdletbinding()]
    param(
        [string]$psbuildInstallUri = 'https://raw.githubusercontent.com/ligershark/psbuild/master/src/GetPSBuild.ps1'
    )
    process{
        if(-not (Get-Command "Invoke-MsBuild" -errorAction SilentlyContinue)){
            'Installing psbuild from [{0}]' -f $psbuildInstallUri | Write-Verbose
            (new-object Net.WebClient).DownloadString($psbuildInstallUri) | iex
        }
        else{
            'psbuild already loaded, skipping download' | Write-Verbose
        }

        # make sure it's loaded and throw if not
        if(-not (Get-Command "Invoke-MsBuild" -errorAction SilentlyContinue)){
            throw ('Unable to install/load psbuild from [{0}]' -f $psbuildInstallUri)
        }
    }
}

# Taken from psake https://github.com/psake/psake

<#  
.SYNOPSIS
  This is a helper function that runs a scriptblock and checks the PS variable $lastexitcode
  to see if an error occcured. If an error is detected then an exception is thrown.
  This function allows you to run command-line programs without having to
  explicitly check the $lastexitcode variable.
.EXAMPLE
  exec { svn info $repository_trunk } "Error executing SVN. Please verify SVN command-line client is installed"
#>
function Exec {
    [CmdletBinding()]
    param(
        [Parameter(Position=0,Mandatory=1)][scriptblock]$cmd,
        [Parameter(Position=1,Mandatory=0)][string]$errorMessage = ($msgs.error_bad_command -f $cmd)
    )
    & $cmd
    if ($lastexitcode -ne 0) {
        Set-Location $rootDir
        throw ("Exec: " + $errorMessage)
    }
}
Write-Output ""
Write-Output "Begin: build.ps1"
$repoTag = @{ $true = $env:APPVEYOR_REPO_TAG_NAME; $false = $NULL }[$env:APPVEYOR_REPO_TAG_NAME -ne $NULL]
#only care about tags that look like version numbers 
$isBeta = $repoTag -notmatch "^[V|v]?\d+\.\d+\.\d+"

$revision = @{ $true = $env:APPVEYOR_BUILD_NUMBER; $false = 1 }[$env:APPVEYOR_BUILD_NUMBER -ne $NULL]
$revision = "beta-{0:D4}" -f [convert]::ToInt32($revision, 10)

Write-Output "------------------------------------"
Write-Output "Repo Tag = $repoTag"
Write-Output "Building Beta = $isBeta"
Write-Output @{$true = "Revision = $revision"; $false =""}[$isBeta]
Write-Output "------------------------------------"

if(Test-Path .\dist) {
    Remove-Item .\dist -Force -Recurse 
}

EnsurePsbuildInstalled

exec { & dotnet restore }

$rootDir = Get-Location
$srcDir = Get-ChildItem ./src

[object[]]$projectFolders = $NULL
[object[]]$testFolders = $NULL

if(!$config){
    $config = "Release"
}

# loop through projects and collect src and test project paths
foreach ($folder in $srcDir) {
    $p = Join-Path -Path $folder.FullName -ChildPath 'project.json';
    # only src project folders -> folders with a project.json file 
    if (Test-Path $p -PathType Leaf) {
        $projectFolders += $folder.FullName
        # find the test project, if one exists, and run each
        $testFolderPath = ".\test\" + $folder.Name + ".Tests"
        if (Test-Path $testFolderPath -PathType Container){
            $x = Join-Path -Path $testFolderPath -ChildPath 'xunit.runner.json';
            if (Test-Path $x -PathType Leaf) {
                $testFolders += $testFolderPath    
            }
        }
    }
}

# run tests first
foreach($testFolder in $testFolders){
    Write-Output ""
    Write-Output "--------"
    Write-Output "testing : $testFolder"
    Write-Output "--------"
    Set-Location $testFolder
    exec { & dotnet test --configuration $config -trait "TestType=Unit" }
    Set-Location $rootDir
}

# package src projects
foreach($srcFolder in $projectFolders){
    Write-Output ""
    Write-Output "--------"
    Write-Output "packing : $srcFolder"
    Write-Output "--------"
    if($isBeta){
        exec { & dotnet pack $srcFolder -c $config -o .\dist --version-suffix=$revision }
    }
    else {
        exec { & dotnet pack $srcFolder -c $config -o .\dist }

    }
}

Write-Output "Done: build.ps1"