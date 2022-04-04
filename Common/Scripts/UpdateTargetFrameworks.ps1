$slnName = $args[0];

$fileContents = Get-Content -Path .\..\Labs.TargetFrameworks.default.props
$newFileContents = $fileContents;

if ($slnName -eq "Toolkit.Labs.Windows") {
    $newFileContents = $fileContents -replace '<(LinuxLibTargetFramework|AndroidLibTargetFramework|MacOSLibTargetFramework|iOSLibTargetFramework)>.+?>', '';
}

if ($slnName -eq "Toolkit.Labs.Wasm") {
    $newFileContents = $fileContents -replace '<(UwpTargetFramework|WinAppSdkTargetFramework|WpfLibTargetFramework|LinuxLibTargetFramework|AndroidLibTargetFramework|MacOSLibTargetFramework|iOSLibTargetFramework)>.+?>', '';
}

if ($newFileContents -eq $fileContents) {
    return;
}

Clear-Content -Path .\..\Labs.TargetFrameworks.props;
Add-Content -Force -Path .\..\Labs.TargetFrameworks.props -Value $newFileContents;