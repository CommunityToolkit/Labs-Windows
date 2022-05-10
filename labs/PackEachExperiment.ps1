$curDir = Get-Location

Get-ChildItem -Path $curDir 
 -Attributes Directory |

ForEach-Object {

$labPath = Join-Path $curDir.Path $_
$labPath = Join-Path $labPath "src"

Set-Location "$($curDir)\$($_.BaseName)\src"

& msbuild.exe -t:pack /p:Configuration=Release /p:DebugType=Portable

}
