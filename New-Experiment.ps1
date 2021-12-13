<#
.SYNOPSIS
    This script generates a template for quickly setting up new Labs experiements. The template includes all of the projects, test infrastructure, and document stubs needed to meet the quality bar expected of new Community Toolkit features.

.DESCRIPTION
    Use New-Experiment.ps1 to start a new Community Toolkit Labs experiment with a fresh copy of the standard experiment solution template. Provide a project name and your new experiment will be added to the Labs collection.

.PARAMETER ExperimentName
    Provide a unique name for the new experiment. This value will be used to set namespaces and directory names in the new solution.

.INPUTS
    None. You cannot pipe objects to New-Experiment.ps1.

.OUTPUTS
    New-Experiment.ps1 will generate a new folder in the Labs directory, matching the provided experiment name.

.EXAMPLE
    Example syntax for running the script or function
    PS C:\> Example
#>

# ExperimentName is mandatory.
param ([Parameter(Mandatory=$true)][string]$ExperimentName)

function New-Experiment {

    # Let's begin
    Write-Output "Generating new Community Toolkit Labs experiment..."

    # Are we in the right directory? Expected at repo root, with Template and Labs folders present.
    if (-not (Test-Path "./Labs" -PathType Container && Test-Path "./Template" -PathType Container))
    {
        Write-Error -Message "Unexpected execution location. Please run this script from the Community Toolkit Labs repo root directory."
        exit;
    }

    # Get the experiment name input string.
    # Is the name valid (a non-empty string)?

    # Does an experiment by this name already exist in the Labs folder?
    # If so, we'll need to re-prompt for the experiment name and re-validate until the name is determined to be unique.
    while (Test-Path "./Labs/$($ExperimentName)" -PathType Container)
    {
        Write-Output "An experiment with the name $($ExperimentName) already exists. Please specify a unique experiment name:"
        
        $ExperimentName= Read-Host -Prompt "ExperimentName"

        # Is the name valid (a non-empty string)?
    }

    # Is dotnet new CLI available?
    # If not, bail with message
    try {
        
    }
    catch {
        Write-Error -Message "Unexpected execution location. Please run this script from the Community Toolkit Labs repo root directory."
        exit;
    }

    # Is the new experiment template installed?
    # If not, install it.


    # Run the command: dotnet new lapsapp -n $ExperimentName
    # Make sure the output goes to the /Labs folder.

}

New-Experiment