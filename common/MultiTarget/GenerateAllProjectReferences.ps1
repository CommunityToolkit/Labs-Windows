Param (
  [Parameter(HelpMessage = "The directory where props files for discovered projects should be saved.")] 
  [string]$projectPropsOutputDir = "$PSScriptRoot/Generated",

  [Parameter(HelpMessage = "Filters projects that have paths which match the provided string.")]
  [string]$Exclude = "*template*",

  [Parameter(HelpMessage = "The directories to scan for projects. Supports wildcards.")]
  [string[]]$projectDirectories = @("$PSScriptRoot/../../*/*/samples/*.Samples/*.Samples.csproj", "$PSScriptRoot/../../*/*/src/*.csproj")
)

# Create output folder if not exists
New-Item -ItemType Directory -Force -Path $projectPropsOutputDir -ErrorAction SilentlyContinue | Out-Null;

# Discover projects in provided paths
foreach ($path in $projectDirectories) {
  foreach ($projectPath in Get-ChildItem -Recurse -Path $path -Exclude $Exclude) {
    $relativePath = Resolve-Path -Relative -Path $projectPath;
    $relativePath = $relativePath.TrimStart('.\');
    $projectName = [System.IO.Path]::GetFileNameWithoutExtension($relativePath);
    
    & $PSScriptRoot\GenerateMultiTargetAwareProjectReferenceProps.ps1 $projectPath "$projectPropsOutputDir/$projectName.props";
  }
}

