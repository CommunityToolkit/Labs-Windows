# Rename ./labs/ to ./src/

function ReplaceInFile([string] $source, [string] $originalValue, [string] $newValue) {
    $sourceContents = Get-Content -Path $source;

    $sourceContents = $sourceContents -Replace $originalValue, $newValue;

    Set-Content -Path $source -Value $sourceContents;
}

if (Test-Path "$PSScriptRoot\..\..\labs") {
    Remove-Item -Path "$PSScriptRoot\..\..\src" -Force -Recurse;
    Rename-Item "$PSScriptRoot\..\..\labs" -NewName "src" -Force;

    ReplaceInFile -source "$PSScriptRoot/../../GenerateAllSolution.ps1" -originalValue 'labs/' -newValue 'src/'
    ReplaceInFile -source "$PSScriptRoot/../../.github/workflows/build.yml" -originalValue '/labs' -newValue '/src'
    ReplaceInFile -source "$PSScriptRoot/../GenerateVSCodeLaunchConfig.ps1" -originalValue 'labs/' -newValue 'src/'
    ReplaceInFile -source "$PSScriptRoot/../CommunityToolkit.Labs.Core.SourceGenerators/ToolkitSampleMetadataGenerator.Documentation.cs" -originalValue 'labs/' -newValue 'src/'
    ReplaceInFile -source "$PSScriptRoot/../CommunityToolkit.Labs.Core.SourceGenerators/ToolkitSampleMetadataGenerator.Documentation.cs" -originalValue 'labs\\' -newValue 'src\'
    ReplaceInFile -source "$PSScriptRoot/../MultiTarget/GenerateAllProjectReferences.ps1" -originalValue 'labs/' -newValue 'src/'
    ReplaceInFile -source "$PSScriptRoot/../Scripts/PackEachExperiment.ps1" -originalValue 'labs/' -newValue 'src/'
    ReplaceInFile -source "$PSScriptRoot/../../template/README.md" -originalValue 'labs/' -newValue 'src/'
    ReplaceInFile -source "$PSScriptRoot/../Labs.Head.props" -originalValue 'labs\\' -newValue 'src\'
    ReplaceInFile -source "$PSScriptRoot/../CommunityToolkit.Labs.Core.SourceGenerators.Tests/CommunityToolkit.Labs.Core.SourceGenerators.Tests/ToolkitSampleMetadataTests.cs" -originalValue 'labs\\' -newValue 'src\'
    ReplaceInFile -source "$PSScriptRoot/../../tests/CommunityToolkit.Labs.Tests.Uwp/CommunityToolkit.Labs.Tests.Uwp.csproj" -originalValue 'labs\\' -newValue 'src\'
    ReplaceInFile -source "$PSScriptRoot/../../tests/CommunityToolkit.Labs.Tests.WinAppSdk/CommunityToolkit.Labs.Tests.WinAppSdk.csproj" -originalValue 'labs\\' -newValue 'src\'
}
