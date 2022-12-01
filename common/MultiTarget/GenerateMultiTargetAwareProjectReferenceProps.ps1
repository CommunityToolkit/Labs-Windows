Param (    
    [Parameter(HelpMessage = "The full path of the csproj to generated references to.", Mandatory = $true)] 
    [string]$projectPath,

    [Parameter(HelpMessage = "A path to a .props file where generated content should be saved to.", Mandatory = $true)] 
    [string]$outputPath,

    [Parameter(HelpMessage = "The path to the template used to generate the props file.")] 
    [string]$templatePath = "$PSScriptRoot/MultiTargetAwareProjectReference.props.template",

    [Parameter(HelpMessage = "The placeholder text to replace when inserting the project file name into the template.")] 
    [string]$projectFileNamePlaceholder = "[ProjectFileName]",

    [Parameter(HelpMessage = "The placeholder text to replace when inserting the project path into the template.")] 
    [string]$projectRootPlaceholder = "[ProjectRoot]"
)

$relativeProjectPath = (Resolve-Path -Path $projectPath);
$templateContents = Get-Content -Path $templatePath;

# Insert csproj file name.
$templateContents = $templateContents -replace [regex]::escape($projectFileNamePlaceholder), ([System.IO.Path]::GetFileName(($relativeProjectPath)));

# Insert project directory
$templateContents = $templateContents -replace [regex]::escape($projectRootPlaceholder), ([System.IO.Path]::GetDirectoryName(($relativeProjectPath)));

# Save to disk
Set-Content -Path $outputPath -Value $templateContents;