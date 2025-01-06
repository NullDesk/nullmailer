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
param([Parameter(Mandatory)][string]$apikey, [switch]$publish = $false)

$rootDir = Get-Location
$srcDir = Get-ChildItem ./src
$outputDir = Join-Path -Path $rootDir -ChildPath '/packOutput'
Write-Output `n

if (-not (Test-Path -Path $outputDir -PathType Container)) {
    Write-Output "Creating output directory: $outputDir"
    New-Item -Path $outputDir -ItemType Directory
}
Write-Output "Cleaning output directory: $outputDir"
Remove-Item $outputDir\*
Write-Output `n

if(!$config){
    $config = "Release"
    Write-Output "Configuration $config"
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
Write-Output `n
if($publish){
    Write-Output "Publish packages"
    & dotnet nuget push $outputDir\*.* -s https://api.nuget.org/v3/index.json -k $apikey
    Write-Output `n
}
else
{
    Write-Output "Skiped publishing packages; use the -publish switch to publish"
}
Write-Output "Done"
