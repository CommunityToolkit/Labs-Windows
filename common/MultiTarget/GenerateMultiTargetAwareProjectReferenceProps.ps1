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
    [string]$projectRootPlaceholder = "[ProjectRoot]"
)

$preWorkingDir = $pwd;
Set-Location "$PSScriptRoot/../../"

$relativeProjectPath = Resolve-Path -Relative -Path $projectPath
$templateContents = Get-Content -Path $templatePath;

Set-Location $preWorkingDir;

# Insert csproj file name.
$csprojFileName = [System.IO.Path]::GetFileName($relativeProjectPath);
$templateContents = $templateContents -replace [regex]::escape($projectFileNamePlaceholder), $csprojFileName;

# Insert project directory
$projectDirectoryRelativeToRoot = [System.IO.Path]::GetDirectoryName($relativeProjectPath).TrimStart('.').TrimStart('\');
$templateContents = $templateContents -replace [regex]::escape($projectRootPlaceholder), "$projectDirectoryRelativeToRoot";

function LoadMultiTargetsFrom([string] $path) {
    $fileContents = "";

    # If file does not exist
    if ($false -eq (Test-Path -Path $path -PathType Leaf)) {
        # Load first available default
        foreach ($fallbackPath in $multiTargetFallbackPropsPath) {
            if (Test-Path $fallbackPath) {
                $fileContents = Get-Content $fallbackPath -ErrorAction Stop;
                break;
            }
        }
    }
    else {
        # Load requested file
        $fileContents = Get-Content $path -ErrorAction Stop;
    }

    # Parse file contents
    $regex = Select-String -Pattern '<MultiTarget>(.+?)<\/MultiTarget>' -InputObject $fileContents;

    if ($null -eq $regex -or $null -eq $regex.Matches -or $null -eq $regex.Matches.Groups -or $regex.Matches.Groups.Length -lt 2) {
        Write-Error "Couldn't get MultiTarget property from $path";
        exit(-1);
    }

    return $regex.Matches.Groups[1].Value;
}

# Load multitarget preferences for project
$multiTargets = LoadMultiTargetsFrom("$([System.IO.Path]::GetDirectoryName($projectPath))\MultiTarget.props");

$templateContents = $templateContents -replace [regex]::escape("[IntendedTargets]"), $multiTargets;

$multiTargets = $multiTargets.Split(';');
Write-Host "Generating project references for $([System.IO.Path]::GetFileNameWithoutExtension($csprojFileName)): $($multiTargets -Join ', ')"

$templateContents = $templateContents -replace [regex]::escape("[CanTargetWasm]"), "'$($multiTargets.Contains("wasm").ToString().ToLower())'";
$templateContents = $templateContents -replace [regex]::escape("[CanTargetUwp]"), "'$($multiTargets.Contains("uwp").ToString().ToLower())'";
$templateContents = $templateContents -replace [regex]::escape("[CanTargetWasdk]"), "'$($multiTargets.Contains("wasdk").ToString().ToLower())'";
$templateContents = $templateContents -replace [regex]::escape("[CanTargetWpf]"), "'$($multiTargets.Contains("wpf").ToString().ToLower())'";
$templateContents = $templateContents -replace [regex]::escape("[CanTargetLinuxGtk]"), "'$($multiTargets.Contains("linuxgtk").ToString().ToLower())'";
$templateContents = $templateContents -replace [regex]::escape("[CanTargetMacOS]"), "'$($multiTargets.Contains("macos").ToString().ToLower())'";
$templateContents = $templateContents -replace [regex]::escape("[CanTargetiOS]"), "'$($multiTargets.Contains("ios").ToString().ToLower())'";
$templateContents = $templateContents -replace [regex]::escape("[CanTargetDroid]"), "'$($multiTargets.Contains("android").ToString().ToLower())'";

# Save to disk
Set-Content -Path $outputPath -Value $templateContents;
