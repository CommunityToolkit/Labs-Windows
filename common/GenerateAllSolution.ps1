<#
.SYNOPSIS
    Generates the solution file comprising of platform heads for samples, individual component projects, and tests.
.DESCRIPTION
    Used mostly for CI building of everything and testing end-to-end scenarios involving the full
    sample app experience.

    Otherwise it is recommended to focus on an individual component's solution instead.
.PARAMETER IncludeHeads
    List of TFM based projects to include. This can be 'all', 'uwp', or 'winappsdk'.

    Defaults to 'all' for local-use.
.PARAMETER UseDiagnostics
    Add extra diagnostic output to running slngen, such as a binlog, etc...
.EXAMPLE
    C:\PS> .\GenerateAllSolution -IncludeHeads winappsdk
    Build a solution that doesn't contain UWP projects.
.NOTES
    Author: Windows Community Toolkit Labs Team
    Date:   April 27, 2022
#>
Param (
    [Parameter(HelpMessage = "The heads to include for building platform samples and tests.", ParameterSetName = "IncludeHeads")]
    [ValidateSet('all', 'uwp', 'winappsdk')]
    [string]$IncludeHeads = 'all',

    [Parameter(HelpMessage = "Add extra diagnostic output to slngen generator.")]
    [switch]$UseDiagnostics = $false
)

# Generate required props for "All" solution.
& ./common/MultiTarget/GenerateAllProjectReferences.ps1
& ./common/GenerateVSCodeLaunchConfig.ps1

# Set up constant values
$generatedSolutionFilePath = 'Toolkit.Labs.All.sln'
$platforms = '"Any CPU;x64;x86;ARM64"'
$slngenConfig = "--folders true --collapsefolders true --ignoreMainProject"

# Remove previous file if it exists
if (Test-Path -Path $generatedSolutionFilePath)
{
    Remove-Item $generatedSolutionFilePath
    Write-Host "Removed previous solution file"
}

# Projects to include
$projects = [System.Collections.ArrayList]::new()

# Common/Dependencies for shared infrastructure
[void]$projects.Add(".\common\CommunityToolkit*\*.*proj")

# App Head and Test Head
if ($IncludeHeads -ne 'winappsdk')
{
    [void]$projects.Add(".\common\ProjectHeads\AllComponents\**\*.Uwp.csproj")
}

if ($IncludeHeads -ne 'uwp')
{
    [void]$projects.Add(".\common\ProjectHeads\AllComponents\**\*.WinAppSdk.csproj")
}

[void]$projects.Add(".\common\ProjectHeads\AllComponents\**\*.Wasm.csproj")

# Individual projects
[void]$projects.Add(".\components\**\src\*.csproj")
[void]$projects.Add(".\components\**\samples\*.Samples.csproj")
[void]$projects.Add(".\components\**\tests\*.Tests\*.shproj")

if ($UseDiagnostics.IsPresent)
{
    $sdkoptions = " -d"
    $diagnostics = '-bl:slngen.binlog --consolelogger:"ShowEventId;Summary;Verbosity=Detailed" --filelogger:"LogFile=slngen.log;Append;Verbosity=Diagnostic;Encoding=UTF-8" '
}
else
{
    $sdkoptions = ""
    $diagnostics = ""
}

$cmd = "dotnet$sdkoptions tool run slngen -o $generatedSolutionFilePath $slngenConfig $diagnostics--platform $platforms $($projects -Join ' ')"

Write-Output "Running Command: $cmd"

Invoke-Expression $cmd
