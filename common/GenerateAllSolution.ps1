Param (
    [Parameter(HelpMessage = "The WinUI version to use when building an Uno head.", ParameterSetName = "UseUnoWinUI")]
    [ValidateSet('all', 'uwp', 'winappsdk')]
    [string]$IncludeTests = 'all'
)

# Generate required props for "All" solution.
& ./common/MultiTarget/GenerateAllProjectReferences.ps1
& ./common/GenerateVSCodeLaunchConfig.ps1

# Set up constant values
$generatedSolutionFilePath = 'Toolkit.Labs.All.sln'
$platforms = '"Any CPU;x64;x86;ARM64"' # ARM64 is ignored here currently: https://github.com/microsoft/slngen/issues/437
$slngenConfig = "--folders true --collapsefolders true --ignoreMainProject"

# remove previous file if it exists
if (Test-Path -Path $generatedSolutionFilePath)
{
    Remove-Item $generatedSolutionFilePath
    Write-Host "Removed previous solution file"
}

# Projects to include
$projects = [System.Collections.ArrayList]::new()

# Common/Dependencies for shared infrastructure
[void]$projects.Add(".\common\**\*.*proj")

# Sample Apps
[void]$projects.Add(".\platforms\**\*.csproj") # All Platform heads TODO uwp/winappsdk/uno split

# Tests
[void]$projects.Add(".\tests\**\*.csproj") # Test Runner heads TODO: one or other for uwp/winappsdk

# Individual projects
[void]$projects.Add(".\labs\**\src\*.csproj")
[void]$projects.Add(".\labs\**\samples\*.Samples\*.Samples.csproj")
[void]$projects.Add(".\labs\**\tests\*.Tests\*.shproj")

# Remove test project we don't want to build (Uwp for WinAppSdk and vice versa
<# if ($IncludeTests -eq "uwp") {
	Write-Output "Remove WinAppSdk Test Project";
	$index = $solutionTemplate.IndexOf($templatedProjectTestWinAppSdkMarker)
	$solutionTemplate.RemoveAt($index);
	$solutionTemplate.RemoveAt($index);
	$solutionTemplate.RemoveAt($index);

	# Remove link to Folder
	$index = $solutionTemplate.IndexOf($templatedProjectTestWinAppSdkMarker)
	$solutionTemplate.RemoveAt($index);
	$solutionTemplate.RemoveAt($index);	
} elseif ($IncludeTests -eq "winappsdk") {
	Write-Output "Remove Uwp Test Project";
	$index = $solutionTemplate.IndexOf($templatedProjectTestUwpMarker)
	$solutionTemplate.RemoveAt($index);
	$solutionTemplate.RemoveAt($index);
	$solutionTemplate.RemoveAt($index);

	# Remove link to Folder
	$index = $solutionTemplate.IndexOf($templatedProjectTestUwpMarker)
	$solutionTemplate.RemoveAt($index);
	$solutionTemplate.RemoveAt($index);	
}#>

$cmd = "dotnet slngen -o $generatedSolutionFilePath $slngenConfig --platform $platforms $($projects -Join ' ')"

Write-Output "Running Command: $cmd"

Invoke-Expression $cmd
