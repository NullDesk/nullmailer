<#
.SYNOPSIS
    Local powershell script buld and deploy nuget packages.

.DESCRIPTION
    Run from the repository root. Uses dotnet pack and nuget.exe to build and
    publish nuget packages. Often used to publish experimental beta versions
    without going through the formal CICD pipeline.

.PARAMETER apikey
    API Key for nuget.org

.EXAMPLE
    .\deploy.ps1

#>
[cmdletbinding()]
param([Parameter(Mandatory)][string]$apikey)

$rootDir = Get-Location
$srcDir = Get-ChildItem ./src
$outputDir = Join-Path -Path $rootDir -ChildPath '/packOutput'


if (-not (Test-Path -Path $outputDir -PathType Container)) {
    New-Item -Path $outputDir -ItemType Directory
}
Remove-Item $outputDir\*

#Write-Output "Getting nuget.exe"
#$sourceNugetExe = "https://dist.nuget.org/win-x86-commandline/latest/nuget.exe"
#$targetNugetExe = "$outputDir\nuget.exe"
#$progressPreference = 'silentlyContinue'
#Invoke-WebRequest $sourceNugetExe -OutFile $targetNugetExe
#$progressPreference = 'continue'
#
#Set-Alias nuget $targetNugetExe -Scope Global -Verbose

if(!$config){
    $config = "Release"
}

Write-Output "restoring packages"
& dotnet restore

[object[]]$projectFolders = $NULL

# loop through projects and collect src and test project paths
foreach ($folder in $srcDir) {
    $p = Join-Path -Path $folder.FullName -ChildPath '*.csproj';
    # only src project folders -> folders with a csproj file
    if (Test-Path $p -PathType Leaf) {
        $projectFolders += $folder.FullName
    }
}

foreach($srcFolder in $projectFolders){
    Write-Output "Build packages"
    & dotnet pack $srcFolder -c $config -o $outputDir -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg
}

 & dotnet nuget push $outputDir -s https://api.nuget.org/v3/index.json -k $apikey

#Set-Location $outputDir
#$pkgs = Get-ChildItem -Path .\ -exclude '*.snupkg','*nuget.exe'
#foreach($pkg in $pkgs){
#
#    & $targetNugetExe push $pkg.Name $apikey -Source https://api.nuget.org/v3/index.json
#}
#Set-Location $rootDir

#Remove-Item $targetNugetExe

Write-Output "Done"
