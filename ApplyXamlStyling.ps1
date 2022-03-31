
param(
    [Parameter(HelpMessage="Runs against last commit vs. current changes")]
    [switch]$LastCommit = $false,

    [Parameter(HelpMessage="Runs against staged files vs. current changes")]
    [switch]$Staged = $false,

    [Parameter(HelpMessage="Runs against main vs. current branch (for CI)")]
    [switch]$Main = $false
)

Write-Output "Restoring dotnet tools..."
dotnet tool restore

# Look for unstaged changed files by default
$gitDiffCommand = "git diff --name-only --diff-filter=ACM"

if ($Main)
{
    Write-Output 'Checking Current Branch against `main` Files Only'
    $branch = git status | Select-String -Pattern "On branch (?<branch>.*)$"
    $branch = $branch.Matches.groups[1].Value
    $gitDiffCommand = "git diff main $branch --name-only --diff-filter=ACM"
}
elseif ($Staged)
{
    # Look for staged files
    Write-Output "Checking Staged Files Only"
    $gitDiffCommand = "git diff --cached --name-only --diff-filter=ACM"
}
elseif ($LastCommit)
{
    # Look at last commit files
    Write-Output "Checking the Last Commit's Files Only"
    $gitDiffCommand = "git diff HEAD^ HEAD --name-only --diff-filter=ACM"
}
else 
{
    Write-Output "Checking Current Unstaged Files Only"    
}

Write-Output "Running Git Diff: $gitDiffCommand"
$files = Invoke-Expression $gitDiffCommand | Select-String -Pattern "\.xaml$"

if ($files.count -gt 0)
{
    Write-Output "Running XAML Styler w/ settings.xamlstyler config file..."
    # XAML Styler doesn't return any status. https://github.com/Xavalon/XamlStyler/issues/289#issuecomment-1085105679
    dotnet tool run xstyler -c .\settings.xamlstyler -f $files

    ## Run git status and check for modified xaml files:
    $files = git status | Select-String -Pattern "\.xaml$"

    if ($files.count -gt 0)
    {
        Write-Output 'XAML Files Modified; If this message is in the CI, please run `.\ApplyXamlStyling.ps1 -Main` locally to apply changes.'
        exit 1
    }
    else
    {
        Write-Output "No XAML Files Modified by XAML Styler"
        exit 0
    }
}
else
{
    Write-Output "No XAML Files found to style..."
    exit 0 #success, we didn't have to do anything
}
