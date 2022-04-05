$slnName = $args[0];

$fileContents = Get-Content -Path .\..\Labs.TargetFrameworks.default.props
$newFileContents = $fileContents;

# If solution is set to All, don't do any replacements and copy all TFMs.
if (-not($slnName -eq "Toolkit.Labs.All")) {

    # If WASM, remove all non-wasm TFMs
    if ($slnName -eq "Toolkit.Labs.Wasm") {
        $newFileContents = $fileContents -replace '<(UwpTargetFramework|WinAppSdkTargetFramework|WpfLibTargetFramework|LinuxLibTargetFramework|AndroidLibTargetFramework|MacOSLibTargetFramework|iOSLibTargetFramework)>.+?>', '';
    }
    # If any other solution, assume minimal Windows dependencies.
    else {
        $newFileContents = $fileContents -replace '<(LinuxLibTargetFramework|AndroidLibTargetFramework|MacOSLibTargetFramework|iOSLibTargetFramework)>.+?>', '';
    }
}

if ($newFileContents -eq $fileContents) {
    return;
}

if (Test-Path '.\..\Labs.TargetFrameworks.props') {
    Clear-Content -Path .\..\Labs.TargetFrameworks.props;
}

Add-Content -Force -Path .\..\Labs.TargetFrameworks.props -Value $newFileContents;