
foreach ($experimentSlnPath in Get-ChildItem -Recurse -Path '*/*.sln') {
  & msbuild.exe -t:pack /p:Configuration=Release /p:DebugType=Portable $experimentSlnPath
}
