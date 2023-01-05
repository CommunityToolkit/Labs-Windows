Param (
    [Parameter(HelpMessage = "Migrate only the provided project folder path.")] 
    [string]$ProjectPath = $null
)

function MigrateProject([string] $ProjectPath) {
    $projectName = [System.IO.Path]::GetFileName($ProjectPath);
    Write-Output "Migrating $projectName";

    mkdir "$ProjectPath/heads" | Out-Null
    mkdir "$ProjectPath/heads/Uwp" | Out-Null
    mkdir "$ProjectPath/heads/Wasm" | Out-Null
    mkdir "$ProjectPath/heads/WinAppSdk" | Out-Null
    mkdir "$ProjectPath/heads/Tests.Uwp" | Out-Null
    mkdir "$ProjectPath/heads/Tests.WinAppSdk" | Out-Null

    Move-Item (Resolve-Path "$ProjectPath/samples/$projectName.Uwp/") (Resolve-Path "$ProjectPath/heads/Uwp/") -ErrorAction Stop
    Move-Item (Resolve-Path "$ProjectPath/samples/$projectName.Wasm/") (Resolve-Path "$ProjectPath/heads/Wasm/") -ErrorAction Stop
    Move-Item (Resolve-Path "$ProjectPath/samples/$projectName.WinAppSdk/") (Resolve-Path "$ProjectPath/heads/WinAppSdk/") -ErrorAction Stop
    Move-Item (Resolve-Path "$ProjectPath/samples/$projectName.Samples/") (Resolve-Path "$ProjectPath/samples/") -ErrorAction Stop

    Move-Item (Resolve-Path "$ProjectPath/tests/$projectName.Tests.Uwp/") (Resolve-Path "$ProjectPath/heads/Tests.Uwp/") -ErrorAction Stop
    Move-Item (Resolve-Path "$ProjectPath/tests/$projectName.Tests.WinAppSdk/") (Resolve-Path "$ProjectPath/heads/Tests.WinAppSdk/") -ErrorAction Stop
    Move-Item (Resolve-Path "$ProjectPath/tests/$projectName.Tests/") (Resolve-Path "$ProjectPath/tests/") -ErrorAction Stop
}

function ReplaceInFile([string] $Source, [string] $OriginalValue, [string] $NewValue) {
    $sourceContents = Get-Content -Path $Source;

    $sourceContents = $sourceContents -Replace $OriginalValue, $NewValue;

    Set-Content -Path $Source -Value $sourceContents;
}

if (!($null -eq $ProjectPath) -and $ProjectPath.Length -gt 0) {
    MigrateProject -ProjectPath $ProjectPath;
    return;
}

foreach($projectPath in Get-ChildItem -Directory "$PSScriptRoot\..\..\src\*") {
    MigrateProject -ProjectPath $projectPath;
}

MigrateProject -ProjectPath "$PSScriptRoot\..\..\template\ProjectTemplate"