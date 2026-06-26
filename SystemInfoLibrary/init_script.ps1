if($isWindows)
{
	$env:SOLUTION_NAME="SystemInfoLibrary.sln"
	
	if ($env:APPVEYOR_REPO_TAG -eq "true")
	{
		Update-AppveyorBuild -Version "$($env:APPVEYOR_REPO_TAG_NAME)"
	}
}