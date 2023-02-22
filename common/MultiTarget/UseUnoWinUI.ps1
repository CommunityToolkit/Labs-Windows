Param (
    [Parameter(HelpMessage = "The WinUI version to use when building an Uno head.", Mandatory = $true)]
    [ValidateSet('2', '3')]
    [string]$winUIMajorVersion,
    
    [Parameter(HelpMessage = "Disables suppressing changes to the affected files in git, allowing changes to be committed.")] 
    [switch]$allowGitChanges = $false
)

# Suppress git warning "core.useBuiltinFSMonitor will be deprecated soon; use core.fsmonitor instead"
& git config advice.useCoreFSMonitorConfig false;

function SupressOrAllowChanges([string] $filePath) {
    if ($allowGitChanges.IsPresent) {
        git update-index --no-assume-unchanged $filePath
    }
    else {
        git update-index --assume-unchanged $filePath
    }
}

function ApplyWinUISwap([string] $filePath) {
    SupressOrAllowChanges $filePath;

    $fileContents = Get-Content -Path $filePath;
    
    if ($winUIMajorVersion -eq "3") {
        $fileContents = $fileContents -replace '<WinUIMajorVersion>2</WinUIMajorVersion>', '<WinUIMajorVersion>3</WinUIMajorVersion>';
        $fileContents = $fileContents -replace '<PackageIdVariant>Uwp</PackageIdVariant>', '<PackageIdVariant>WinUI</PackageIdVariant>';
        $fileContents = $fileContents -replace 'Uno.UI', 'Uno.WinUI';

        $fileContents = $fileContents -replace '\$\(DefineConstants\);WINUI2;', '$(DefineConstants);WINUI3;WINAPPSDK;';
    }

    if ($winUIMajorVersion -eq "2") {
        $fileContents = $fileContents -replace '<WinUIMajorVersion>3</WinUIMajorVersion>', '<WinUIMajorVersion>2</WinUIMajorVersion>';
        $fileContents = $fileContents -replace '<PackageIdVariant>WinUI</PackageIdVariant>', '<PackageIdVariant>Uwp</PackageIdVariant>';
        $fileContents = $fileContents -replace 'Uno.WinUI', 'Uno.UI';

        $fileContents = $fileContents -replace '\$\(DefineConstants\);WINUI3;WINAPPSDK;', '$(DefineConstants);WINUI2;';
    }

    Set-Content -Force -Path $filePath -Value $fileContents;
    Write-Output "Updated $(Resolve-Path -Relative $filePath)"
}

Write-Output "Switching to WinUI $winUIMajorVersion";

ApplyWinUISwap $PSScriptRoot/../ProjectHeads/App.Head.Uno.props
ApplyWinUISwap $PSScriptRoot/Uno.props
ApplyWinUISwap $PSScriptRoot/ProjectIdentifiers.props

if ($allowGitChanges.IsPresent) {
    Write-Warning "Changes to the default Uno package settings in Labs can now be committed.`r`nRun this command again without -allowGitChanges to disable committing further changes.";
}
else {
    Write-Output "Changes to the default Uno package settings in Labs are now suppressed.`r`nTo switch branches, run `"git reset --hard`" with a clean working tree.";
}

Write-Output "Done, switched to WinUI $winUIMajorVersion"