Param (
    [Parameter(HelpMessage = "The path to the containing folder for a component where sample heads should be generated.")] 
    [string]$componentPath,

    [Parameter(HelpMessage = "The heads that should be generated. If excluded, all heads will be generated.")] 
    [string[]]$heads = @("uwp", "wasm", "winappsdk", "tests.uwp", "tests.winappsdk")
)

function ReplaceInFile([string] $Source, [string] $OriginalValue, [string] $NewValue) {
    $originalContent = Get-Content -Path $Source -ErrorAction Stop
    $newContent = $originalContent.Replace($OriginalValue, $NewValue)
    
    if ($newContent -ne $originalContent) {
        Write-Host "Saving $Source"
        Set-Content -Path $Source -Value $newContent -ErrorAction Stop
    }
}

function ReplaceInName([string]$Source, [string] $OriginalValue, [string] $NewValue) {
    $item = Get-Item $Source;
    $newName = $item.Name -replace $OriginalValue, $NewValue
        
    if ($newName -ne $item.Name) {
        Rename-Item -Path $Source -NewName $newName -ErrorAction Stop
    } 
}

$headsFolder = "$componentPath/heads/";
Remove-Item -Recurse -Force $headsFolder -ErrorAction SilentlyContinue;

mkdir $headsFolder -ErrorAction SilentlyContinue | Out-Null;
$componentName = (Get-Item $componentPath -ErrorAction Stop).Name;

foreach ($availableHead in (Get-ChildItem -Directory "$PSScriptRoot/SingleComponent/" -ErrorAction Stop)) {
    foreach ($head in $heads) {
        if ($availableHead.Name.ToLower() -eq $head) {   
            Copy-Item -Recurse -Force -Path "$PSScriptRoot/SingleComponent/$($availableHead.Name)" -Destination "$headsFolder/" -ErrorAction Stop
        }
    }
}

foreach ($item in (Get-ChildItem -File -Recurse $headsFolder)) {
    ReplaceInFile $item "ProjectTemplate" $componentName
    ReplaceInName $item "ProjectTemplate" $componentName
}

foreach ($item in (Get-ChildItem -Directory -Recurse $headsFolder)) {
    ReplaceInName $item "ProjectTemplate" $componentName
}
