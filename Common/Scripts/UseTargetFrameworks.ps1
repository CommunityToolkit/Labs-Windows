if ($args.Length -eq 0 -or ![bool]$args[0]) {
    Write-Error "Please supply one or more arguments. Valid values are all, wasm, uwp, winappsdk, wpf, gtk, macos, ios, and droid.";
    exit(-1);
}

$WasmTfm = "WasmLibTargetFramework";
$UwpTfm = "UwpTargetFramework";
$WinAppSdkTfm = "WinAppSdkTargetFramework";
$WpfTfm = "WpfLibTargetFramework";
$GtkTfm = "LinuxLibTargetFramework";
$macOSTfm = "MacOSLibTargetFramework";
$iOSTfm = "iOSLibTargetFramework";
$DroidTfm = "AndroidLibTargetFramework";

$fileContents = Get-Content -Path .\..\Labs.TargetFrameworks.All.props

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

if ($args.Contains("all")) {
    $desiredTfmValues = $allTargetFrameworks;
}

if ($args.Contains("wasm")) {
    $desiredTfmValues += $WasmTfm;
}

if ($args.Contains("uwp")) {
    $desiredTfmValues += $UwpTfm;
}

if ($args.Contains("winappsdk")) {
    $desiredTfmValues += $WinAppSdkTfm;
}

if ($args.Contains("wpf")) {
    $desiredTfmValues += $WpfTfm;
}

if ($args.Contains("gtk")) {
    $desiredTfmValues + $GtkTfm;
}

if ($args.Contains("macos")) {
    $desiredTfmValues += $macOSTfm;
}

if ($args.Contains("ios")) {
    $desiredTfmValues += $iOSTfm;
}

if ($args.Contains("droid")) {
    $desiredTfmValues += $DroidTfm;
}

$targetFrameworksToRemove = $allTargetFrameworks.Where({-not $desiredTfmValues.Contains($_)})

$targetFrameworksToRemoveRegexPartial = $targetFrameworksToRemove -join "|";

$newFileContents = $fileContents -replace "<(?:$targetFrameworksToRemoveRegexPartial)>.+?>", '';

Set-Content -Force -Path .\..\Labs.TargetFrameworks.props -Value $newFileContents;
