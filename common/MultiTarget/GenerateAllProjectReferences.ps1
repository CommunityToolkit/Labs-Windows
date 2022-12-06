Param (
  [Parameter(HelpMessage = "The directory where props files for discovered projects should be saved.")] 
  [string]$projectPropsOutputDir = "$PSScriptRoot/Generated",

  [Parameter(HelpMessage = "Filters projects that have paths which match the provided string.")]
  [string]$Exclude = "*template*",

  [Parameter(HelpMessage = "The path to a props file that will be populated with project imports for all discovered projects.")] 
  [string]$outputPath = "$projectPropsOutputDir/AllGeneratedProjectReferences.props",

  [Parameter(HelpMessage = "The directories to scan for projects. Supports wildcards.")]
  [string[]]$projectDirectories = @("$PSScriptRoot/../../*/*/samples/*.Samples/*.Samples.csproj", "$PSScriptRoot/../../*/*/src/*.csproj"),

  [Parameter(HelpMessage = "The path to the template used to generate the props file.")] 
  [string]$templatePath = "$PSScriptRoot/AllGeneratedProjectReferences.props.template",

  [Parameter(HelpMessage = "The placeholder text to replace when inserting the project file name into the template.")] 
  [string]$projectReferencesDefinitionMarker = "[ProjectReferenceImports]"
)

# Load template
$templateContents = Get-Content -Path $templatePath;
Write-Output "Loaded template from $(Resolve-Path $templatePath)";

mkdir $projectPropsOutputDir -ErrorAction SilentlyContinue | Out-Null;

# Discover projects in provided paths
foreach ($path in $projectDirectories) {
  foreach ($projectPath in Get-ChildItem -Recurse -Path $path -Exclude $Exclude) {
    $relativePath = Resolve-Path -Relative -Path $projectPath;
    $relativePath = $relativePath.TrimStart('.\');
    $projectName = [System.IO.Path]::GetFileNameWithoutExtension($relativePath);
    
    & $PSScriptRoot\GenerateMultiTargetAwareProjectReferenceProps.ps1 $projectPath "$projectPropsOutputDir/$projectName.props";
    $projectReferenceDefinition = "<Import Project=`"`$(RepositoryDirectory)/common/MultiTarget/Generated/$projectName.props`" />";
  
    $templateContents = $templateContents -replace [regex]::escape($projectReferencesDefinitionMarker), ($projectReferencesDefinitionMarker + "
    " + $projectReferenceDefinition);
  }
}

# Remove placeholder from template
$templateContents = $templateContents -replace [regex]::escape($projectReferencesDefinitionMarker), "";

# Save
Set-Content -Path $outputPath -Value $templateContents;
Write-Output "Project references generated at $(Resolve-Path $outputPath)";
