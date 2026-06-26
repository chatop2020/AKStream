if($env:CONFIGURATION -eq "Release" -and $isWindows) # is not a pull request
{
	cd $env:APPVEYOR_BUILD_FOLDER
	cd common
	nuget pack SystemInfoLibrary-devel.nuspec -Version $env:APPVEYOR_BUILD_VERSION
	$nupkg = (Get-ChildItem SystemInfoLibrary-devel*.nupkg)[0];
	Push-AppveyorArtifact $nupkg.FullName -FileName $nupkg.Name -DeploymentName "SystemInfoLibrary-devel.nupkg";
}