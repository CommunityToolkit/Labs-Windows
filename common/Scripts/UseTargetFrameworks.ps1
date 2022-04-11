Param (
	[ValidateSet('all', 'wasm', 'uwp', 'winappsdk', 'wpf', 'gtk', 'macos', 'ios', 'droid')]
	[string[]]$targets
)

if ($args.Contains("--allow-git-changes")) {
    Write-Warning "Changes to the default TargetFrameworks in Labs can now be committed. Run this command again without the --allow-git-changes flag to disable committing further changes.";
    git update-index --no-assume-unchanged ../Labs.TargetFrameworks.props
}
else {
    git update-index --assume-unchanged ../Labs.TargetFrameworks.props
}

$WasmTfm = "WasmLibTargetFramework";
$UwpTfm = "UwpTargetFramework";
$WinAppSdkTfm = "WinAppSdkTargetFramework";
$WpfTfm = "WpfLibTargetFramework";
$GtkTfm = "LinuxLibTargetFramework";
$macOSTfm = "MacOSLibTargetFramework";
$iOSTfm = "iOSLibTargetFramework";
$DroidTfm = "AndroidLibTargetFramework";

$fileContents = Get-Content -Path ../Labs.TargetFrameworks.All.props

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

Set-Content -Force -Path ../Labs.TargetFrameworks.props -Value $newFileContents;
