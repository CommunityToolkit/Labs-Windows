# This file can execute in parallel, causing potential file access issues.
# This lock prevent us from reading or writing concurrently and getting access exceptions.
if (Test-Path ".\..\Labs.TargetFrameworks.props.lock") {
    return;
}

if ($args.Length -eq 0 -or ![bool]$args[0]) {
    Write-Error "Missing parameter. Please supply a variant value of All, Wasm or Windows.";
    exit(-1);
}

$variant = $args[0];

# MSBuild uses the SolutionName to determine the variant, but SolutionName occassionally goes missing during
# the build process, which causes the default/fallback variant to be used when it shouldn't.
# This check ensures we're always using the same variant, until the file is deleted by MSBuild during a clean.
if ((Get-Content -Path .\UpdateTargetFrameworks.CurrentVariant.txt).Trim() -notlike $variant) {
    # for debug only
    Set-Content -Path .\UpdateTargetFrameworks.CircumventedVariant.$variant.txt -Value '';
    return;
}

Set-Content -Path .\UpdateTargetFrameworks.CurrentVariant.txt -Value $variant;

Set-Content -Path .\..\Labs.TargetFrameworks.props.lock -Value '';
$fileContents = Get-Content -Path .\..\Labs.TargetFrameworks.default.props

if ($variant -eq "All") {
    # If set to All, don't do any replacements and copy all TFMs.
    $newFileContents = $fileContents;
}

if ($variant -eq "Wasm") {
    # Remove all non-wasm TFMs
    $newFileContents = $fileContents -replace '<(UwpTargetFramework|WinAppSdkTargetFramework|WpfLibTargetFramework|LinuxLibTargetFramework|AndroidLibTargetFramework|MacOSLibTargetFramework|iOSLibTargetFramework)>.+?>', '';
}

if ($variant -eq "Windows") {
    # Minimal Windows dependencies.
    $newFileContents = $fileContents -replace '<(LinuxLibTargetFramework|AndroidLibTargetFramework|MacOSLibTargetFramework|iOSLibTargetFramework)>.+?>', '';
}

# MSBuild uses last modified date instead of file hashes to determine when the content has changed.
# Only update the props file if the new content is different.
if ($newFileContents -eq $fileContents) {
    Remove-Item -Path .\..\Labs.TargetFrameworks.props.lock;
    return;
}

Set-Content -Force -Path .\..\Labs.TargetFrameworks.props -Value $newFileContents;
Remove-Item -Path .\..\Labs.TargetFrameworks.props.lock;
