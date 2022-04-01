
param(
    [Parameter(HelpMessage="Runs against last commit vs. current changes")]
    [switch]$LastCommit = $false,

    [Parameter(HelpMessage="Runs against staged files vs. current changes")]
    [switch]$Staged = $false,

    [Parameter(HelpMessage="Runs against main vs. current branch")]
    [switch]$Main = $false,

    [Parameter(HelpMessage="Runs a passive check against all files in the repo for the CI")]
    [switch]$Passive = $false
)

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
        dotnet tool run xstyler -p -c .\settings.xamlstyler -f $files
    }
    else
    {
        Write-Output "No XAML Files found to style..."
    }
}
else 
{
    Write-Output "Checking all files (passively)"
    $files = Get-ChildItem *.xaml -Recurse | Select-Object -ExpandProperty FullName

    if ($files.count -gt 0)
    {
        dotnet tool run xstyler -p -c .\settings.xamlstyler -f $files

        if ($lastExitCode -eq 1)
        {
            Write-Error 'XAML Styling is incorrect, please run `ApplyXamlStyling.ps1` locally.'
        }

        # Return XAML Styler Status
        exit $lastExitCode
    }
    else
    {
        exit 0
    }
}
