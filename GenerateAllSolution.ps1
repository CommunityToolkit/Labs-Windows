Param (
    [Parameter(HelpMessage = "The WinUI version to use when building an Uno head.", ParameterSetName = "UseUnoWinUI")]
    [ValidateSet('2', '3')]
    [string]$UseUnoWinUI = 2
)

# Generate required props for "All" solution.
& ./common/MultiTarget/GenerateAllProjectReferences.ps1
& ./common/GenerateVSCodeLaunchConfig.ps1

# Set WinUI version for Uno projects
$originalWorkingDirectory = Get-Location;

Set-Location common/Scripts/
& ./UseUnoWinUI.ps1 $UseUnoWinUI

Set-Location $originalWorkingDirectory;

# Set up contant values
$templatedProjectFolderConfigTemplateMarker = "[TemplatedProjectFolderConfig]";
$templatedProjectConfigurationTemplateMarker = "[TemplatedProjectConfigurations]";
$templatedProjectDefinitionsMarker = "[TemplatedProjectDefinitions]";
$templatedSharedTestProjectSelfDefinitionsMarker = "[TemplatedSharedTestProjectDefinitions]";
$templatedSharedTestUwpProjectSelfDefinitionsMarker = "[TemplatedSharedTestUwpProjectDefinitions]";
$templatedSharedTestWinAppSdkProjectSelfDefinitionsMarker = "[TemplatedSharedTestWinAppSdkProjectDefinitions]";

$sampleProjectTypeGuid = "9A19103F-16F7-4668-BE54-9A1E7A4F7556";
$sharedProjectTypeGuid = "D954291E-2A0B-460D-934E-DC6B0785DB48";
$libProjectTypeGuid = $sampleProjectTypeGuid;

$solutionTemplatePath = 'common/Toolkit.Labs.All.sln.template';
$generatedSolutionFilePath = 'Toolkit.Labs.All.sln'

function CreateProjectConfiguration {
	param (
		[string]$projectGuid
	)

	# Solution files are VERY picky about the unicode characters used as newlines and tabs.
	# These characters act strange when paired next to characters found in ASCII, so we append
	# as separate strings.
	#
	# Not doing this would cause Visual Studio to fix the file on load, and always ask the user
	# if they want to save the changes.
	return "
" + "		" + "{$projectGuid}.Ad-Hoc|Any CPU.ActiveCfg = Debug|Any CPU
		{$projectGuid}.Ad-Hoc|Any CPU.Build.0 = Debug|Any CPU
		{$projectGuid}.Ad-Hoc|ARM.ActiveCfg = Release|Any CPU
		{$projectGuid}.Ad-Hoc|ARM.Build.0 = Release|Any CPU
		{$projectGuid}.Ad-Hoc|ARM64.ActiveCfg = Release|Any CPU
		{$projectGuid}.Ad-Hoc|ARM64.Build.0 = Release|Any CPU
		{$projectGuid}.Ad-Hoc|iPhone.ActiveCfg = Release|Any CPU
		{$projectGuid}.Ad-Hoc|iPhone.Build.0 = Release|Any CPU
		{$projectGuid}.Ad-Hoc|iPhoneSimulator.ActiveCfg = Release|Any CPU
		{$projectGuid}.Ad-Hoc|iPhoneSimulator.Build.0 = Release|Any CPU
		{$projectGuid}.Ad-Hoc|x64.ActiveCfg = Release|Any CPU
		{$projectGuid}.Ad-Hoc|x64.Build.0 = Release|Any CPU
		{$projectGuid}.Ad-Hoc|x86.ActiveCfg = Release|Any CPU
		{$projectGuid}.Ad-Hoc|x86.Build.0 = Release|Any CPU
		{$projectGuid}.AppStore|Any CPU.ActiveCfg = Release|Any CPU
		{$projectGuid}.AppStore|Any CPU.Build.0 = Release|Any CPU
		{$projectGuid}.AppStore|ARM.ActiveCfg = Release|Any CPU
		{$projectGuid}.AppStore|ARM.Build.0 = Release|Any CPU
		{$projectGuid}.AppStore|ARM64.ActiveCfg = Release|Any CPU
		{$projectGuid}.AppStore|ARM64.Build.0 = Release|Any CPU
		{$projectGuid}.AppStore|iPhone.ActiveCfg = Release|Any CPU
		{$projectGuid}.AppStore|iPhone.Build.0 = Release|Any CPU
		{$projectGuid}.AppStore|iPhoneSimulator.ActiveCfg = Release|Any CPU
		{$projectGuid}.AppStore|iPhoneSimulator.Build.0 = Release|Any CPU
		{$projectGuid}.AppStore|x64.ActiveCfg = Release|Any CPU
		{$projectGuid}.AppStore|x64.Build.0 = Release|Any CPU
		{$projectGuid}.AppStore|x86.ActiveCfg = Release|Any CPU
		{$projectGuid}.AppStore|x86.Build.0 = Release|Any CPU
		{$projectGuid}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
		{$projectGuid}.Debug|Any CPU.Build.0 = Debug|Any CPU
		{$projectGuid}.Debug|ARM.ActiveCfg = Debug|Any CPU
		{$projectGuid}.Debug|ARM.Build.0 = Debug|Any CPU
		{$projectGuid}.Debug|ARM64.ActiveCfg = Debug|Any CPU
		{$projectGuid}.Debug|ARM64.Build.0 = Debug|Any CPU
		{$projectGuid}.Debug|iPhone.ActiveCfg = Debug|Any CPU
		{$projectGuid}.Debug|iPhone.Build.0 = Debug|Any CPU
		{$projectGuid}.Debug|iPhoneSimulator.ActiveCfg = Debug|Any CPU
		{$projectGuid}.Debug|iPhoneSimulator.Build.0 = Debug|Any CPU
		{$projectGuid}.Debug|x64.ActiveCfg = Debug|Any CPU
		{$projectGuid}.Debug|x64.Build.0 = Debug|Any CPU
		{$projectGuid}.Debug|x86.ActiveCfg = Debug|Any CPU
		{$projectGuid}.Debug|x86.Build.0 = Debug|Any CPU
		{$projectGuid}.Release|Any CPU.ActiveCfg = Release|Any CPU
		{$projectGuid}.Release|Any CPU.Build.0 = Release|Any CPU
		{$projectGuid}.Release|ARM.ActiveCfg = Release|Any CPU
		{$projectGuid}.Release|ARM.Build.0 = Release|Any CPU
		{$projectGuid}.Release|ARM64.ActiveCfg = Release|Any CPU
		{$projectGuid}.Release|ARM64.Build.0 = Release|Any CPU
		{$projectGuid}.Release|iPhone.ActiveCfg = Release|Any CPU
		{$projectGuid}.Release|iPhone.Build.0 = Release|Any CPU
		{$projectGuid}.Release|iPhoneSimulator.ActiveCfg = Release|Any CPU
		{$projectGuid}.Release|iPhoneSimulator.Build.0 = Release|Any CPU
		{$projectGuid}.Release|x64.ActiveCfg = Release|Any CPU
		{$projectGuid}.Release|x64.Build.0 = Release|Any CPU
		{$projectGuid}.Release|x86.ActiveCfg = Release|Any CPU
		{$projectGuid}.Release|x86.Build.0 = Release|Any CPU";
}

function CreateProjectDefinition {
	param (
		[string]$projectTypeGuid,
		[string]$projectGuid,
		[string]$projectName,
		[string]$projectPath
	)

	return "
Project(`"{$projectTypeGuid}`") = `"$projectName`", `"$projectPath`", `"{$projectGuid}`"
EndProject";
}

function CreateSharedProjectDefinition {
	param (
		[string]$projectGuid,
		[string]$sharedProjectPath,
		[string]$id
	)

	return "
"+ "		" + "$sharedProjectPath*{$projectGuid}*SharedItemsImports = $id";
}

function CreateFolderPlacementForProject {
	param (
		[string]$projectGuid,
		[string]$folderGuid
	)

	return [System.Environment]::NewLine + "		" + "{$projectGuid} = {$folderGuid}";
}

function GetFolderGuid {
	param (
		[string]$templateContents,
		[string]$folderName
	)

	$folderDefinitionRegex = "Project\(`"{2150E333-8FDC-42A3-9474-1A3956D46DE8}`"\) = `"$folderName`", `"$folderName`", `"\{(.+?)\}`"";

	if (-not ($templateContents -match $folderDefinitionRegex)) {
		Write-Error "Folder `"$folderName`" was not found";
		exit(-1);
	}

	return $Matches[1];
}

function AddProjectsToSolution {
	param (
		$templateContents,
		[string]$projectPath,
		[string]$projectTypeGuid,
		[string]$solutionFolder,
		[string]$projectGuid,
		[bool]$omitConfiguration
	)

	if ($projectGuid.Length -eq 0) {
		$projectGuid = (New-Guid).ToString().ToUpper();
	}

	$projectPath = Resolve-Path -Relative -Path $projectPath;
	$projectName = [System.IO.Path]::GetFileNameWithoutExtension($projectPath);

	$sampleFolderGuid = GetFolderGuid $solutionTemplate $solutionFolder;

	Write-Host "Adding $projectName to solution folder $solutionFolder";

	$definition = CreateProjectDefinition $projectTypeGuid $projectGuid $projectName $projectPath;

	$templateContents = $templateContents -replace [regex]::escape($templatedProjectDefinitionsMarker), ($templatedProjectDefinitionsMarker + $definition);

	if (-not $omitConfiguration) {
		$configuration = CreateProjectConfiguration $projectGuid;
		$templateContents = $templateContents -replace [regex]::escape($templatedProjectConfigurationTemplateMarker), ($templatedProjectConfigurationTemplateMarker + $configuration);
	}

	$folderPlacement = CreateFolderPlacementForProject $projectGuid $sampleFolderGuid;
	$templateContents = $templateContents -replace [regex]::escape($templatedProjectFolderConfigTemplateMarker), ($templatedProjectFolderConfigTemplateMarker + $folderPlacement);

	return $templateContents;
}

# Execute solution generation from template
$solutionTemplate = Get-Content -Path $solutionTemplatePath;
Write-Output "Loaded solution template from $solutionTemplatePath";

# Add sample projects
foreach ($sampleProjectPath in Get-ChildItem -Recurse -Path 'labs/*/samples/*.Samples/*.Samples.csproj') {
	$solutionTemplate = AddProjectsToSolution $solutionTemplate $sampleProjectPath $sampleProjectTypeGuid "Samples"
}

# Add library projects
foreach ($sampleProjectPath in Get-ChildItem -Recurse -Path 'labs/*/src/*.csproj') {
	$solutionTemplate = AddProjectsToSolution $solutionTemplate $sampleProjectPath $libProjectTypeGuid "Library"
}

# Add shared test projects
foreach ($sharedProjectItemsPath in Get-ChildItem -Recurse -Path 'labs/*/tests/*.projitems') {
	$projitemsContents = Get-Content -Path $sharedProjectItemsPath;

	$projectGuidRegex = '<SharedGUID>(.+?)<\/SharedGUID>';

	$regex = Select-String -Pattern $projectGuidRegex -InputObject $projitemsContents;

	if ($null -eq $regex -or $null -eq $regex.Matches -or $null -eq $regex.Matches.Groups -or $regex.Matches.Groups.Length -lt 2) {
		Write-Error "Couldn't get SharedGUID property from $sharedProjectItemsPath";
		exit(-1);
	}

	$projectGuid = $regex.Matches.Groups[1].Value;

	$sharedProjectItemsPath = Resolve-Path -Relative -Path $sharedProjectItemsPath;

	$sharedProjectPath = $sharedProjectItemsPath -replace "projitems", "shproj";
	$solutionTemplate = AddProjectsToSolution $solutionTemplate $sharedProjectPath $sharedProjectTypeGuid "Experiments" $projectGuid.ToUpper() $true;

	$sharedProjectItemsName = [System.IO.Path]::GetFileNameWithoutExtension($sharedProjectItemsPath);

	Write-Output "Linking $sharedProjectItemsName.projitems to $sharedProjectItemsName";
	$sharedProjectDefinition = CreateSharedProjectDefinition $projectGuid $sharedProjectItemsPath "13"
	$solutionTemplate = $solutionTemplate -replace [regex]::escape($templatedSharedTestProjectSelfDefinitionsMarker), ($templatedSharedTestProjectSelfDefinitionsMarker + $sharedProjectDefinition);

	Write-Output "Linking $sharedProjectItemsName.projitems to CommunityToolkit.Labs.Tests.Uwp";
	$uwpSharedProjectDefinition = CreateSharedProjectDefinition "fd78002e-c4e6-4bf8-9ec3-c06250dfef34" $sharedProjectItemsPath "4"
	$solutionTemplate = $solutionTemplate -replace [regex]::escape($templatedSharedTestUwpProjectSelfDefinitionsMarker), ($templatedSharedTestUwpProjectSelfDefinitionsMarker + $uwpSharedProjectDefinition);

	Write-Output "Linking $sharedProjectItemsName.projitems to CommunityToolkit.Labs.Tests.WinAppSdk";
	$winAppSdkSharedProjectDefinition = CreateSharedProjectDefinition "53892f07-fe54-4e36-81d8-105427d097e5" $sharedProjectItemsPath "5"
	$solutionTemplate = $solutionTemplate -replace [regex]::escape($templatedSharedTestWinAppSdkProjectSelfDefinitionsMarker), ($templatedSharedTestWinAppSdkProjectSelfDefinitionsMarker + $winAppSdkSharedProjectDefinition);
}

# Clean up template markers
$solutionTemplate = $solutionTemplate -replace [regex]::escape($templatedProjectFolderConfigTemplateMarker), "";
$solutionTemplate = $solutionTemplate -replace [regex]::escape($templatedProjectConfigurationTemplateMarker), "";
$solutionTemplate = $solutionTemplate -replace [regex]::escape($templatedProjectDefinitionsMarker), "";
$solutionTemplate = $solutionTemplate -replace [regex]::escape($templatedSharedTestProjectSelfDefinitionsMarker), "";
$solutionTemplate = $solutionTemplate -replace [regex]::escape($templatedSharedTestUwpProjectSelfDefinitionsMarker), "";
$solutionTemplate = $solutionTemplate -replace [regex]::escape($templatedSharedTestWinAppSdkProjectSelfDefinitionsMarker), "";
$solutionTemplate = $solutionTemplate -replace "(?m)^\s*`r`n", "";

# Save
Set-Content -Path $generatedSolutionFilePath -Value $solutionTemplate;
Write-Output "Solution generated at $generatedSolutionFilePath";
