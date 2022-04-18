$templatedProjectFolderConfigTemplateMarker = "[TemplatedProjectFolderConfig]";
$templatedProjectConfigurationTemplateMarker = "[TemplatedProjectConfigurations]";
$templatedProjectDefinitionsMarker = "[TemplatedProjectDefinitions]";

$sampleProjectTypeGuid = "9A19103F-16F7-4668-BE54-9A1E7A4F7556";
$libProjectTypeGuid = $sampleProjectTypeGuid;

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
        [string]$solutionFolder
    )

    $projectGuid = (New-Guid).ToString().ToUpper();
    $projectName = [System.IO.Path]::GetFileNameWithoutExtension($sampleProjectPath);
    $sampleFolderGuid = GetFolderGuid $solutionTemplate $solutionFolder;

    Write-Host "Adding $projectName to solution folder $solutionFolder";

    $definition = CreateProjectDefinition $projectTypeGuid $projectGuid $projectName $sampleProjectPath;
    
    $templateContents = $templateContents -replace [regex]::escape($templatedProjectDefinitionsMarker), ($templatedProjectDefinitionsMarker + $definition); 

    $configuration = CreateProjectConfiguration $projectGuid;
    $templateContents = $templateContents -replace [regex]::escape($templatedProjectConfigurationTemplateMarker), ($templatedProjectConfigurationTemplateMarker + $configuration);

    $folderPlacement = CreateFolderPlacementForProject $projectGuid $sampleFolderGuid;
    $templateContents = $templateContents -replace [regex]::escape($templatedProjectFolderConfigTemplateMarker), ($templatedProjectFolderConfigTemplateMarker + $folderPlacement);

    return $templateContents;
}

$solutionTemplate = Get-Content -Path 'common/Toolkit.Labs.All.sln.template';

foreach ($sampleProjectPath in Get-ChildItem -Recurse -Path 'labs/*/samples/*.Sample/*.csproj') {
    $solutionTemplate = AddProjectsToSolution $solutionTemplate $sampleProjectPath $sampleProjectTypeGuid "Samples" 
}

foreach ($sampleProjectPath in Get-ChildItem -Recurse -Path 'labs/*/src/*.csproj') {
    $solutionTemplate = AddProjectsToSolution $solutionTemplate $sampleProjectPath $libProjectTypeGuid "Library" 
}

# Clean up template markers
$solutionTemplate = $solutionTemplate -replace [regex]::escape($templatedProjectFolderConfigTemplateMarker), "";
$solutionTemplate = $solutionTemplate -replace [regex]::escape($templatedProjectConfigurationTemplateMarker), "";
$solutionTemplate = $solutionTemplate -replace [regex]::escape($templatedProjectDefinitionsMarker), "";
$solutionTemplate = $solutionTemplate  -replace "(?m)^\s*`r`n", "";

Set-Content -Path 'Toolkit.Labs.All.sln' -Value $solutionTemplate;



