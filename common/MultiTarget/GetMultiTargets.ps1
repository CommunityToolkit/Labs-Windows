<#
.SYNOPSIS
    Returns the defined MultiTarget value for the provided project name.
.DESCRIPTION
    Locates a component by the provided name from the root ./components folder, and pulls the <MultiTarget> value that should be used.
    The MultiTarget value can be defined by a component in multiple places, but in a MultiTarget.props next to a *.csproj.
    
    The load order is as follows:
    - ./components/SomeComponent/src/MultiTarget.props
    - ./components/SomeComponent/samples/MultiTarget.props
    - $PSScriptRoot/Defaults.props
.PARAMETER ComponentName
    The name of the component.
.EXAMPLE
    C:\PS> .\GetMultiTargets "CanvasView"
    Return the multitargets for the component named "CanvasView".
.NOTES
    Author: Windows Community Toolkit Labs Team
    Date:   February 27, 2023
#>
Param (    
    [Parameter(HelpMessage = "The name of the component.", Mandatory = $true)] 
    [string]$ComponentName
)

$componentPath = "$PSScriptRoot/../../components/$ComponentName";

Test-Path $componentPath -ErrorAction Stop | Out-Null

# Folder layout is expected to match the Community Toolkit.
# Uses the <MultiTarget> values from the source library project as the fallback for the sample project.
# This behavior also implemented in MultiTarget.props for TargetFramework evaluation. 
$parent = Split-Path -Parent $projectPath
$srcPath = Resolve-Path "$parent\..\src";
$samplePath = Resolve-Path "$parent\..\samples";

$multiTargetFallbackPropsPaths = @("$srcPath/MultiTarget.props", "$samplePath/MultiTarget.props", "$PSScriptRoot/Defaults.props")

$fileContents = "";
# Load first available default
foreach ($fallbackPath in $multiTargetFallbackPropsPaths) {
    if (Test-Path $fallbackPath) {
        $fileContents = Get-Content $fallbackPath -ErrorAction Stop;
        break;
    }
}

# Parse file contents
$regex = Select-String -Pattern '<MultiTarget>(.+?)<\/MultiTarget>' -InputObject $fileContents;

if ($null -eq $regex -or $null -eq $regex.Matches -or $null -eq $regex.Matches.Groups -or $regex.Matches.Groups.Length -lt 2) {
    Write-Error "Couldn't get MultiTarget property from $path";
    exit(-1);
}

$value = $regex.Matches.Groups[1].Value;

return $value;