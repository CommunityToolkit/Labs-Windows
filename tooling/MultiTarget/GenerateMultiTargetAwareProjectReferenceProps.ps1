Param (    
    [Parameter(HelpMessage = "The full path of the csproj to generated references to.", Mandatory = $true)] 
    [string]$projectPath,

    [Parameter(HelpMessage = "A path to a .props file where generated content should be saved to.", Mandatory = $true)] 
    [string]$outputPath,

    [Parameter(HelpMessage = "The path to the template used to generate the props file.")]
    [string]$templatePath = "$PSScriptRoot/MultiTargetAwareProjectReference.props.template",

    [Parameter(HelpMessage = "The path to the props file that contains the default MultiTarget values.")] 
    [string[]]$multiTargetFallbackPropsPath = @("$PSScriptRoot/Defaults.props"),

    [Parameter(HelpMessage = "The placeholder text to replace when inserting the project file name into the template.")] 
    [string]$projectFileNamePlaceholder = "[ProjectFileName]",

    [Parameter(HelpMessage = "The placeholder text to replace when inserting the project path into the template.")] 
    [string]$projectRootPlaceholder = "[ProjectRoot]",

    [Parameter(HelpMessage = "Only projects that support these targets will have references generated for use by deployable heads.")]
    [ValidateSet("uwp", "wasdk", "wpf", "wasm", "linuxgtk", "macos", "ios", "android")]
    [string[]] $MultiTarget = @("uwp", "wasdk", "wpf", "wasm", "linuxgtk", "macos", "ios", "android")
)

$preWorkingDir = $pwd;
Set-Location "$PSScriptRoot/../../"

$relativeProjectPath = Resolve-Path -Relative -Path $projectPath
$templateContents = Get-Content -Path $templatePath;

Set-Location $preWorkingDir;

# Insert csproj file name.
$csprojFileName = [System.IO.Path]::GetFileName($relativeProjectPath);
$templateContents = $templateContents -replace [regex]::escape($projectFileNamePlaceholder), $csprojFileName;
$projectName = (Get-Item (Split-Path -Parent $projectPath)).Name;

# Insert project directory
$projectDirectoryRelativeToRoot = [System.IO.Path]::GetDirectoryName($relativeProjectPath).TrimStart('.').TrimStart('\');
$templateContents = $templateContents -replace [regex]::escape($projectRootPlaceholder), "$projectDirectoryRelativeToRoot";

# Load multitarget preferences for project
$multiTargets = & $PSScriptRoot\GetMultiTargets.ps1 -ComponentName $projectName -ErrorAction Stop;
    
$templateContents = $templateContents -replace [regex]::escape("[IntendedTargets]"), $multiTargets;
$multiTargets = $multiTargets.Split(';');

function ShouldMultiTarget([string] $target) {
    return ($multiTargets.Contains($target) -and $MultiTarget.Contains($target)).ToString().ToLower()
}

Write-Host "Generating project references for $([System.IO.Path]::GetFileNameWithoutExtension($csprojFileName)): $($multiTargets -Join ', ')"
$templateContents = $templateContents -replace [regex]::escape("[CanTargetWasm]"), "'$(ShouldMultiTarget "wasm")'";
$templateContents = $templateContents -replace [regex]::escape("[CanTargetUwp]"), "'$(ShouldMultiTarget "uwp")'";
$templateContents = $templateContents -replace [regex]::escape("[CanTargetWasdk]"), "'$(ShouldMultiTarget "wasdk")'";
$templateContents = $templateContents -replace [regex]::escape("[CanTargetWpf]"), "'$(ShouldMultiTarget "wpf")'";
$templateContents = $templateContents -replace [regex]::escape("[CanTargetLinuxGtk]"), "'$(ShouldMultiTarget "linuxgtk")'";
$templateContents = $templateContents -replace [regex]::escape("[CanTargetMacOS]"), "'$(ShouldMultiTarget "macos")'";
$templateContents = $templateContents -replace [regex]::escape("[CanTargetiOS]"), "'$(ShouldMultiTarget "ios")'";
$templateContents = $templateContents -replace [regex]::escape("[CanTargetDroid]"), "'$(ShouldMultiTarget "droid")'";

# Save to disk
Set-Content -Path $outputPath -Value $templateContents;
