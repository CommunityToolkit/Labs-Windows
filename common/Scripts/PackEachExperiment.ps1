
foreach ($experimentProjPath in Get-ChildItem -Recurse -Path '../../labs/*/src/*.csproj') {
  & msbuild.exe -t:pack /p:Configuration=Release /p:DebugType=Portable $experimentProjPath
}
