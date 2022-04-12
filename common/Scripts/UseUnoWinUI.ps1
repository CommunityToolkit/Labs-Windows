Param (
    [Parameter(HelpMessage = "The WinUI version to use when building an Uno head.", Mandatory = $true)]
    [ValidateSet('2', '3')]
    [string]$targets,
    
    [Parameter(HelpMessage = "Disables suppressing changes to the Labs.Uno.props file in git, allowing changes to be committed.")] 
    [switch]$allowGitChanges = $false
)

if ($allowGitChanges.IsPresent) {
    Write-Warning "Changes to the default Uno package reference in Labs can now be committed. Run this command again without the --allow-git-changes flag to disable committing further changes.";
    git update-index --no-assume-unchanged ../Labs.Uno.props
}
else {
    git update-index --assume-unchanged ../Labs.Uno.props
}

$fileContents = Get-Content -Path ../Labs.Uno.props

if ($targets -eq "3") {
    $fileContents = $fileContents -replace 'Uno.UI', 'Uno.WinUI';
    $fileContents = $fileContents -replace '\$\(DefineConstants\);', '$(DefineConstants);WINAPPSDK;';
}

if ($targets -eq "2") {
    $fileContents = $fileContents -replace 'Uno.WinUI', 'Uno.UI';
    $fileContents = $fileContents -replace '\$\(DefineConstants\);WINAPPSDK;', '$(DefineConstants);';
}

Set-Content -Force -Path ../Labs.Uno.props -Value $fileContents;
