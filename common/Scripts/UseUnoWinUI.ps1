Param (
    [Parameter(HelpMessage = "The WinUI version to use when building an Uno head.", Mandatory = $true)]
    [ValidateSet('2', '3')]
    [string]$targets,
    
    [Parameter(HelpMessage = "Disables suppressing changes to the Labs.Uno.props file in git, allowing changes to be committed.")] 
    [switch]$allowGitChanges = $false
)

# Suppress git warning "core.useBuiltinFSMonitor will be deprecated soon; use core.fsmonitor instead"
& git config advice.useCoreFSMonitorConfig false;

if ($allowGitChanges.IsPresent) {
    Write-Warning "Changes to the default Uno package settings in Labs can now be committed. Run this command again without the -allowGitChanges flag to disable committing further changes.";
    git update-index --no-assume-unchanged ../Labs.Uno.props
}
else {
    Write-Output "Changes to the default Uno package settings in Labs are now suppressed. To switch branches, run git reset --hard with a clean working tree.";
    git update-index --assume-unchanged ../Labs.Uno.props
}

$fileContents = Get-Content -Path ../Labs.Uno.props

if ($targets -eq "3") {
    $fileContents = $fileContents -replace '<WinUIMajorVersion>2</WinUIMajorVersion>', '<WinUIMajorVersion>3</WinUIMajorVersion>';
    $fileContents = $fileContents -replace '<PackageIdVariant>Uwp</PackageIdVariant>', '<PackageIdVariant>WinUI</PackageIdVariant>';
    $fileContents = $fileContents -replace 'Uno.UI', 'Uno.WinUI';
    $fileContents = $fileContents -replace '\$\(DefineConstants\);', '$(DefineConstants);WINAPPSDK;';
}

if ($targets -eq "2") {
    $fileContents = $fileContents -replace '<WinUIMajorVersion>3</WinUIMajorVersion>', '<WinUIMajorVersion>2</WinUIMajorVersion>';
    $fileContents = $fileContents -replace '<PackageIdVariant>WinUI</PackageIdVariant>', '<PackageIdVariant>Uwp</PackageIdVariant>';
    $fileContents = $fileContents -replace 'Uno.WinUI', 'Uno.UI';
    $fileContents = $fileContents -replace 'WINAPPSDK;', '';
}

Set-Content -Force -Path ../Labs.Uno.props -Value $fileContents;
