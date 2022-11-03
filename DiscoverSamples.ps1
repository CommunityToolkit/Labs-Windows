Param (    
    [Parameter(HelpMessage = "Disables suppressing changes to the ./.vscode/launch.json file in git, allowing changes to be committed.")] 
    [switch]$allowGitChanges = $false
)

$templatedSampleProjectReferencesDefinitionMarker = "[TemplatedSampleProjectReferences]"
$sampleRefsPropsTemplatePath = "$PSScriptRoot/common/Labs.SampleRefs.props.template";
$generatedSampleRefsPropsPath = "$PSScriptRoot/common/Labs.SampleRefs.props";

function CreateVsCodeLaunchConfigJson {
  param (
    [string]$projectName
  )

  return "{
    `"name`": `"$projectName`",
    `"type`": `"coreclr`",
    `"request`": `"launch`",
    `"program`": `"dotnet`",
    `"args`": [
      `"run`",
      `"build`",
      `"/r`",
      `"/p:UnoSourceGeneratorUseGenerationHost=true`",
      `"/p:UnoSourceGeneratorUseGenerationController=false`",
      `"/p:UnoRemoteControlPort=443`",
      `"--project=`$`{workspaceFolder`}/labs/$projectName/samples/$projectName.Wasm/$projectName.Wasm.csproj`"
    ],
    `"presentation`": {
      `"group`": `"2`"
    },
    `"cwd`": `"`$`{workspaceFolder`}/labs/$projectName/samples/$projectName.Wasm`"
  }";
}

# Execute ProjectReference generation for all heads
$sampleRefsPropsTemplate = Get-Content -Path $sampleRefsPropsTemplatePath;
Write-Output "Loaded sample ProjectReference template from $sampleRefsPropsTemplatePath";

# Add sample projects
foreach ($sampleProjectPath in Get-ChildItem -Recurse -Path "$PSScriptRoot/labs/*/samples/*.Samples/*.Samples.csproj") {
  $relativePath = Resolve-Path -Relative -Path $sampleProjectPath;
  $relativePath = $relativePath.TrimStart('.\');
  $projectName = [System.IO.Path]::GetFileNameWithoutExtension($relativePath);

  Write-Host "Adding $projectName to project references";

  $projectReferenceDefinition = "<ProjectReference Include=`"`$(RepositoryDirectory)$relativePath`" />";

  $sampleRefsPropsTemplate = $sampleRefsPropsTemplate -replace [regex]::escape($templatedSampleProjectReferencesDefinitionMarker), ($templatedSampleProjectReferencesDefinitionMarker + "
	" + $projectReferenceDefinition);
}

# Add library projects
foreach ($sampleProjectPath in Get-ChildItem -Recurse -Path "$PSScriptRoot/labs/*/src/*.csproj") {
  $relativePath = Resolve-Path -Relative -Path $sampleProjectPath;
  $relativePath = $relativePath.TrimStart('.\');
  $projectName = [System.IO.Path]::GetFileNameWithoutExtension($relativePath);

  Write-Host "Adding $projectName to project references";

  $projectReferenceDefinition = "<ProjectReference Include=`"`$(RepositoryDirectory)$relativePath`" />";

  $sampleRefsPropsTemplate = $sampleRefsPropsTemplate -replace [regex]::escape($templatedSampleProjectReferencesDefinitionMarker), ($templatedSampleProjectReferencesDefinitionMarker + "
	" + $projectReferenceDefinition);
}

$sampleRefsPropsTemplate = $sampleRefsPropsTemplate -replace [regex]::escape($templatedSampleProjectReferencesDefinitionMarker), "";

# Save
Set-Content -Path $generatedSampleRefsPropsPath -Value $sampleRefsPropsTemplate;
Write-Output "Sample project references generated at $generatedSampleRefsPropsPath";

$launchConfigJson = Get-Content -Path "$PSScriptRoot/.vscode/launch.json";
$launchConfig = $launchConfigJson | ConvertFrom-Json;

# Remove all non-generated configurations
$originalConfigurations = $launchConfig.configurations;
$launchConfig.configurations = @();
$launchConfig.configurations += $originalConfigurations[0];
$launchConfig.configurations += $originalConfigurations[1];

foreach ($wasmProjectPath in Get-ChildItem -Recurse -Path "$PSScriptRoot/labs/*/samples/*.Wasm/*.Wasm.csproj") {
  $projectName = [System.IO.Path]::GetFileNameWithoutExtension($wasmProjectPath) -Replace ".Wasm", "";
  Write-Host "Generating VSCode launch config for $projectName";

  $configJson = CreateVsCodeLaunchConfigJson $projectName;
  $config = $configJson | ConvertFrom-Json;

  $launchConfig.configurations += $config;
}

if ($allowGitChanges.IsPresent) {
  Write-Warning "Changes to the default launch.json in Labs can now be committed. Run this command again without the -allowGitChanges flag to disable committing further changes.";
  git update-index --no-assume-unchanged ./.vscode/launch.json
}
else {
  Write-Output "Changes to the default launch.json in Labs are now suppressed. To switch branches, run git reset --hard with a clean working tree. Include the -allowGitChanges flag to enable committing changes.";
  git update-index --assume-unchanged $PSScriptRoot/.vscode/launch.json
}

# Save
Set-Content -Path "$PSScriptRoot/.vscode/launch.json" -Value ($launchConfig | ConvertTo-Json -Depth 9);
Write-Output "Saved VSCode launch configs to $PSScriptRoot/.vscode/launch.json";