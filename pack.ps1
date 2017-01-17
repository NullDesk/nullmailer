$solutionVer = $null
$rootDir = Get-Location
$outputDir = Join-Path -Path $rootDir -ChildPath '/dist'
$srcDir = Get-ChildItem ./src
$testDir = Get-ChildItem ./test
$config = "Release"

# get the current git branch
$branch = (git symbolic-ref HEAD).Split("/")[-1];
if(($branch -ne "develop") -and ($branch -ne "master")){
    Write-Output "Publish should only be used from the 'develop' or 'master' branch."
    Write-Output "Current branch is '$branch'"
    #exit 1
}
Write-Output ""
Write-Output "Building for branch: $branch"
# make an output folder if not already present 
if(!(Test-Path -Path $outputDir )){
    New-Item -ItemType directory -Path $outputDir
}

function CheckProjectVersion($buildVersion, $folder){
    # set solution version
    if($solutionVer -eq $null){
        $solutionVer = $buildVersion
    } 
    elseif ($solutionVer -ne $buildVersion){
        Write-Output ""
        Write-Output "Solution version does not match project version for: $folder.Name"
        # project has a version that is different from other projects
        #     exit with status code so build fails, and packages aren't deployed with mis-matched versions
        Set-Location $rootDir
        exit 1 
    }
}

#loop through src folders and restore & build
foreach ($folder in $srcDir) {
    $p = Join-Path -Path $folder.FullName -ChildPath 'project.json';
    
    # only restore projects -> folders with a project.json file 
    if (Test-Path $p -PathType Leaf) {
        Set-Location $folder.FullName
        dotnet restore
        dotnet build --configuration $config
    }
}
#loop through test folders and restore & build
foreach ($folder in $testDir) {
    $p = Join-Path -Path $folder.FullName -ChildPath 'project.json';
    
    # only restore projects -> folders with a project.json file 
    if (Test-Path $p -PathType Leaf) {
        Set-Location $folder.FullName
        dotnet restore
        dotnet build --configuration $config
    }
}

$abort = false
# loop through projects under ./src
foreach ($folder in $srcDir) {
    if($abort){
        exit 1
    }
    $p = Join-Path -Path $folder.FullName -ChildPath 'project.json';
    
    # only build projects -> folders with a project.json file 
    if (Test-Path $p -PathType Leaf) {
        Write-Output ""
        Write-Output "Next project: $folder.Name"
        Set-Location $folder.FullName
        # grab the project.json contents
        $origJson = Get-Content -Raw -Path $p;
        $json = $origJson | ConvertFrom-Json  
        $verSuffix = $null

        switch($branch){
            "develop" {
                # for dev branch, we want to build a beta package using the next incremental minor version
                #      and then appending the prerelease suffix "beta-*" 
                
                # pull out version info
                $verArray = $json.version.Split(".")
                $minorInt = [convert]::ToInt32( $verArray[1], 10)
                [int]$incMinor = $minorInt + 1  
                $buildVersion =  $verArray[0] + "." + $incMinor + ".0"

                CheckProjectVersion($buildVersion, $folder)

                $json.version = "$buildVersion-*" #don't forget to add back the "-*"
                $json | ConvertTo-Json -depth 100 | Out-File $p
                
                #TODO: should look in output dir and increment beta-x based on the highest previous beta found
                $verSuffix = "beta-1"             
            }
            "master" {
                # for master, the GitFlow release or hotfix commit should have included any bump in version already
                #     all we have to do is set or check the solution version
 
                # pull out version info
                $buildVersion =  $json.version -replace '-\*$','';

                CheckProjectVersion($buildVersion, $folder)
            }
        }
        
        # find the test project, if one exists, and run each
        $testFolderPath = "..\..\test\" + $folder.Name + ".Tests"
        if (Test-Path $testFolderPath -PathType Container){
            Set-Location $testFolderPath
            # find the test project, if one exists, and run each
            $testFolderPath = "..\..\test\" + $folder.Name + ".Tests"
            if (Test-Path $testFolderPath -PathType Container){
                $x = Join-Path -Path $testFolderPath -ChildPath 'xunit.runner.json';
                if (Test-Path $x -PathType Leaf) {
                    dotnet test --configuration $config -trait "TestType=Unit"
                }
            }
            Set-Location $folder.FullName
        }
        else{
            Write-Output ""
            Write-Output "test path not found"
        }
        
        if($LASTEXITCODE -ne "0"){
            Set-Location $rootDir
            Write-Output ""
            Write-Output "Abort, Unit tests failed"
            $abort = true
        }
        else{
            # do the packaging
            if($verSuffix -ne $null){
                dotnet pack --configuration $config --version-suffix $verSuffix --output $outputDir
            } else {
                dotnet pack --configuration $config --output $outputDir
            }
        } 
        switch($branch){
            "develop" {
                #put the original content back in the src project file  
                $origJson | Out-File $p
            }
        }
            
    }
}
Set-Location $rootDir
Write-Output Done