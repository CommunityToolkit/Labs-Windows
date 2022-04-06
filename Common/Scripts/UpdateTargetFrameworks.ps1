if (Test-Path ".\..\Labs.TargetFrameworks.props.lock") {
    return;
}

if ($args.Length -eq 0 -or ![bool]$args[0]) {
    Write-Error "Missing parameter. Please supply a variant value of All, Wasm or Windows.";
    exit(-1);
}

$variant = $args[0];

Set-Content -Path .\..\Labs.TargetFrameworks.props.lock -Value '';
$fileContents = Get-Content -Path .\..\Labs.TargetFrameworks.default.props

if ($variant -eq "All") {
    # If set to All, don't do any replacements and copy all TFMs.
    $newFileContents = $fileContents;
}

if ($variant -eq "Wasm") {
    # Remove all non-wasm TFMs
    $newFileContents = $fileContents -replace '<(UwpTargetFramework|WinAppSdkTargetFramework|WpfLibTargetFramework|LinuxLibTargetFramework|AndroidLibTargetFramework|MacOSLibTargetFramework|iOSLibTargetFramework)>.+?>', '';
    Set-Content -Path .\..\Labs.TargetFrameworks.props.wasmonly -Value '';
}

if ($variant -eq "Windows") {
    # Minimal Windows dependencies.
    $newFileContents = $fileContents -replace '<(LinuxLibTargetFramework|AndroidLibTargetFramework|MacOSLibTargetFramework|iOSLibTargetFramework)>.+?>', '';
    Set-Content -Path .\..\Labs.TargetFrameworks.props.windowsonly -Value '';
}

if ($newFileContents -eq $fileContents) {
    Remove-Item -Path .\..\Labs.TargetFrameworks.props.lock;
    return;
}

Set-Content -Force -Path .\..\Labs.TargetFrameworks.props -Value $newFileContents;

Remove-Item -Path .\..\Labs.TargetFrameworks.props.lock;