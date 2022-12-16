Param (
  [Parameter(HelpMessage = "The directory where props files for discovered projects should be saved.")]
  [string]$projectPropsOutputDir = "$PSScriptRoot/Generated"
)

$preWorkingDir = $pwd;
Set-Location $PSScriptRoot;

# Delete and recreate output folder.
Remove-Item -Path $projectPropsOutputDir -Recurse -Force -ErrorAction SilentlyContinue | Out-Null;
New-Item -ItemType Directory -Force -Path $projectPropsOutputDir -ErrorAction SilentlyContinue | Out-Null;

# Discover projects in provided paths
foreach ($projectPath in Get-ChildItem -Directory -Depth 0 -Path "$PSScriptRoot/../../labs/") {
  # Normalize project path
  $projectName = $projectPath.Name;

  # Folder layout is expected to match the Community Toolkit.
  # Uses the <MultiTarget> values from the source library project as the fallback for the sample project.
  # This behavior also implemented in MultiTarget.props for TargetFramework evaluation. 
  $srcPath = Resolve-Path "$($projectPath.FullName)\src";
  $srcProjectPath = Get-ChildItem -File "$srcPath\*.csproj";

  $samplePath = Resolve-Path "$($projectPath.FullName)\samples\$projectName.Samples";
  $sampleProjectPath = Get-ChildItem -File "$samplePath\*.csproj";

  if ($srcProjectPath.Length -eq 0) {
    Write-Error "Could not locate source csproj for $projectName";
    exit(-1);
  }

  if ($sampleProjectPath.Length -eq 0) {
    Write-Error "Could not locate sample csproj for $projectName";
    exit(-1);
  }

  # Generate <ProjectReference>s for sample project
  # Use source project MultiTarget as first fallback.
  & $PSScriptRoot\GenerateMultiTargetAwareProjectReferenceProps.ps1 -projectPath $sampleProjectPath -outputPath "$projectPropsOutputDir/$($sampleProjectPath.BaseName).props" -multiTargetFallbackPropsPath @("$srcPath/MultiTarget.props", "$samplePath/MultiTarget.props", "$PSScriptRoot/Defaults.props");

  # Generate <ProjectReference>s for src project
  & $PSScriptRoot\GenerateMultiTargetAwareProjectReferenceProps.ps1 -projectPath $srcProjectPath -outputPath "$projectPropsOutputDir/$($srcProjectPath.BaseName).props" -multiTargetFallbackPropsPath @("$srcPath/MultiTarget.props", "$PSScriptRoot/Defaults.props");
}


Set-Location $preWorkingDir;