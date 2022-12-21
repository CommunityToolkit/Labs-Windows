Param (
    [Parameter(HelpMessage = "The target frameworks to enable.")]
    [ValidateSet('all', 'wasm', 'uwp', 'winappsdk', 'wpf', 'gtk', 'macos', 'ios', 'droid')]
    [string[]]$targets = @('uwp', 'winappsdk', 'wasm'),
    
    [Parameter(HelpMessage = "Disables suppressing changes to the Labs.TargetFrameworks.props file in git, allowing changes to be committed.")] 
    [switch]$allowGitChanges = $false
)

# Suppress git warning "core.useBuiltinFSMonitor will be deprecated soon; use core.fsmonitor instead"
& git config advice.useCoreFSMonitorConfig false;

if ($allowGitChanges.IsPresent) {
    Write-Warning "Changes to the default TargetFrameworks in Labs can now be committed. Run this command again without the -allowGitChanges flag to disable committing further changes.";
    git update-index --no-assume-unchanged $PSScriptRoot/../Labs.TargetFrameworks.props
}
else {
    Write-Output "Changes to the default TargetFrameworks in Labs are now suppressed. To switch branches, run git reset --hard with a clean working tree.";
    git update-index --assume-unchanged $PSScriptRoot/../Labs.TargetFrameworks.props
}

$UwpTfm = "UwpTargetFramework";
$WinAppSdkTfm = "WinAppSdkTargetFramework";
$WasmTfm = "NetStandardCommonTargetFramework";
$WpfTfm = "NetStandardCommonTargetFramework";
$GtkTfm = "NetStandardCommonTargetFramework";
$macOSTfm = "MacOSLibTargetFramework";
$iOSTfm = "iOSLibTargetFramework";
$DroidTfm = "AndroidLibTargetFramework";

$fileContents = Get-Content -Path $PSScriptRoot/../Labs.TargetFrameworks.All.props

$allTargetFrameworks = @(
    $WasmTfm,
    $UwpTfm,
    $WinAppSdkTfm,
    $WpfTfm,
    $GtkTfm,
    $macOSTfm,
    $iOSTfm,
    $DroidTfm
);

$desiredTfmValues = @();

if ($targets.Contains("all")) {
    $desiredTfmValues = $allTargetFrameworks;
}

if ($targets.Contains("wasm")) {
    $desiredTfmValues += $WasmTfm;
}

if ($targets.Contains("uwp")) {
    $desiredTfmValues += $UwpTfm;
}

if ($targets.Contains("winappsdk")) {
    $desiredTfmValues += $WinAppSdkTfm;
}

if ($targets.Contains("wpf")) {
    $desiredTfmValues += $WpfTfm;
}

if ($targets.Contains("gtk")) {
    $desiredTfmValues += $GtkTfm;
}

if ($targets.Contains("macos")) {
    $desiredTfmValues += $macOSTfm;
}

if ($targets.Contains("ios")) {
    $desiredTfmValues += $iOSTfm;
}

if ($targets.Contains("droid")) {
    $desiredTfmValues += $DroidTfm;
}

$targetFrameworksToRemove = $allTargetFrameworks.Where({ -not $desiredTfmValues.Contains($_) })

$targetFrameworksToRemoveRegexPartial = $targetFrameworksToRemove -join "|";

$newFileContents = $fileContents -replace "<(?:$targetFrameworksToRemoveRegexPartial)>.+?>", '';

Set-Content -Force -Path $PSScriptRoot/../Labs.TargetFrameworks.props -Value $newFileContents;
