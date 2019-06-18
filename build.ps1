


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
$distDir = Join-Path -Path $rootDir -ChildPath 'dist'
$srcDir = Get-ChildItem ./src

[object[]]$projectFolders = $NULL
[object[]]$testFolders = $NULL

if(!$config){
    $config = "Release"
}

# loop through projects and collect src and test project paths
foreach ($folder in $srcDir) {
    $p = Join-Path -Path $folder.FullName -ChildPath '*.csproj';
}

exec { & dotnet test --configuration $config --filter "TestType=Unit" }


# package src projects
foreach($srcFolder in $projectFolders){
    Write-Output ""
    Write-Output "--------"
    Write-Output "packing : $srcFolder"
    Write-Output "--------"
    if($isBeta){
        exec { & dotnet pack $srcFolder -c $config -o $distDir --version-suffix=$revision --include-symbols -p:SymbolPackageFormat=snupkg  }
    }
    else {
        exec { & dotnet pack $srcFolder -c $config -o $distDir --include-symbols -p:SymbolPackageFormat=snupkg}
    }
}

Write-Output "Done: build.ps1"