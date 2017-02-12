
# Script by: Enrico Sada
#    https://github.com/enricosada/fsharp-dotnet-cli-samples/blob/master/scripts/show-dotnet-info.ps1
if (Get-Command dotnet -errorAction SilentlyContinue) {
	Write-Host "Using dotnet '$((Get-Command dotnet).Path)'"
	dotnet --version
}
else {
	Write-Host "dotnet.exe not found"
}