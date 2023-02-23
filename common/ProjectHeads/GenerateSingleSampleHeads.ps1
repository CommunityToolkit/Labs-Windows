<#
.SYNOPSIS
    Uses the dotnet template tool to copy and rename project heads to run sample code for different platforms.
.DESCRIPTION
    This is used to centralize configuration and reduce duplication of copying these heads for every project.

    This script also generates a solution for the project and will open Visual Studio.
.PARAMETER componentPath
    Folder for the project to copy the project heads to.
.PARAMETER heads
    Which heads to include to copy, defaults to all. (Currently ignored.)
.PARAMETER UseDiagnostics
    Add extra diagnostic output to running slngen, such as a binlog, etc...
.EXAMPLE
    C:\PS> .\GenerateSingleSampleHeads -componentPath components\testproj
    Builds project heads for component in testproj directory.
.NOTES
    Author: Windows Community Toolkit Labs Team
    Date:   Feb 9, 2023
#>
Param (
    [Parameter(HelpMessage = "The path to the containing folder for a component where sample heads should be generated.")] 
    [string]$componentPath,

    [Parameter(HelpMessage = "The heads that should be generated. If excluded, all heads will be generated. (Currently Ignored)")] 
    [string[]]$heads = @("uwp", "wasm", "winappsdk", "tests.uwp", "tests.winappsdk"),

    [Parameter(HelpMessage = "Add extra diagnostic output to slngen generator.")]
    [switch]$UseDiagnostics = $false
)

if ($Env:Path.Contains("MSBuild") -eq $false) {
    Write-Host
    Write-Host -ForegroundColor Red "Please run from a command window that has MSBuild.exe on the PATH"
    Write-Host
    Write-Host "Press any key to continue"
    [void]$host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")

    Exit
}

$headsFolderName = "heads"
$componentName = (Get-Item $componentPath -ErrorAction Stop).Name

# Remove existing heads directory to refresh
Remove-Item -Recurse -Force "$componentPath/$headsFolderName/" -ErrorAction SilentlyContinue;

# Intall our heads as a temporary template
dotnet new --install "$PSScriptRoot/SingleComponent" --force

# We need to copy files and run slngen from the target directory path
Push-Location $componentPath

# Copy and rename projects
dotnet new ct-tooling-heads -n $componentName

# Rename folder from component name (dotnet tooling default) to 'heads'
Rename-Item -Path "$componentName" -NewName $headsFolderName -Force

# Remove template, as just for script
dotnet new --uninstall "$PSScriptRoot/SingleComponent"


# Generate Solution
#------------------

# Install slgnen
dotnet tool restore

$generatedSolutionFilePath = "$componentPath/$componentName.sln"
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

# Include all projects in component folder
[void]$projects.Add(".\**.*proj")

# Include common dependencies required for solution to build
[void]$projects.Add("..\..\common\CommunityToolkit.Labs.*Shared\**\*.*proj")
[void]$projects.Add("..\..\common\CommunityToolkit.Tooling.SampleGen\*.csproj")
[void]$projects.Add("..\..\common\CommunityToolkit.Tooling.TestGen\*.csproj")
[void]$projects.Add("..\..\common\CommunityToolkit.Tooling.XamlNamedPropertyRelay\*.csproj")

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

# go back to main working directory
Pop-Location
