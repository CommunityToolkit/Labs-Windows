<#
    .SYNOPSIS 
    Modify XAML files to adhere to XAML Styler settings.

    .DESCRIPTION
    The Apply XAML Stying Script can be used to check or modify XAML files with the repo's XAML Styler settings.
    Learn more about XAML Styler at https://github.com/Xavalon/XamlStyler

    By default, unstaged files are modified only.

    Use "PS> Help .\ApplyXamlStyling.ps1 -Full" for more details on parameters.

    .PARAMETER LastCommit
    Runs against last commit vs. current changes

    .PARAMETER Staged
    Runs against staged files vs. current changes

    .PARAMETER Main
    Runs against main vs. current branch

    .PARAMETER Passive
    Runs a passive check against all files in the repo for the CI

    .EXAMPLE
    PS> .\ApplyXamlStyling.ps1 -Main
#>
param(
    [switch]$LastCommit = $false,
    [switch]$Staged = $false,
    [switch]$Main = $false,
    [switch]$Passive = $false
)

Write-Output "Use 'Help .\ApplyXamlStyling.ps1' for more info or '-Main' to run against all files."
Write-Output
Write-Output "Restoring dotnet tools..."
dotnet tool restore

if (-not $Passive)
{
    # Look for unstaged changed files by default
    $gitDiffCommand = "git diff --name-only --diff-filter=ACM"

    if ($Main)
    {
        Write-Output 'Checking Current Branch against `main` Files Only'
        $branch = git status | Select-String -Pattern "On branch (?<branch>.*)$"
        if ($null -eq $branch.Matches)
        {
            $branch = git status | Select-String -Pattern "HEAD detached at (?<branch>.*)$"
            if ($null -eq $branch.Matches)
            {
                Write-Error 'Don''t know how to fetch branch from `git status`:'
                git status | Write-Error
                exit 1
            }
        }
        $branch = $branch.Matches.groups[1].Value
        $gitDiffCommand = "git diff origin/main $branch --name-only --diff-filter=ACM"
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
        dotnet tool run xstyler -c .\settings.xamlstyler -f $files
    }
    else
    {
        Write-Output "No XAML Files found to style..."
    }
}
else 
{
    Write-Output "Checking all files (passively)"
    $files = Get-ChildItem *.xaml -Recurse | Select-Object -ExpandProperty FullName | Where-Object { $_ -notmatch "(\\obj\\)|(\\bin\\)" }

    if ($files.count -gt 0)
    {
        dotnet tool run xstyler -p -c .\settings.xamlstyler -f $files

        if ($lastExitCode -eq 1)
        {
            Write-Error 'XAML Styling is incorrect, please run `ApplyXamlStyling.ps1 -Main` locally.'
        }

        # Return XAML Styler Status
        exit $lastExitCode
    }
    else
    {
        exit 0
    }
}
