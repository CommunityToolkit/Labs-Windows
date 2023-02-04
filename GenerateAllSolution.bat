@ECHO OFF
SET "IncludeHeads=%1"
IF "%IncludeHeads%"=="" SET "IncludeHeads=all"

powershell .\common\GenerateAllSolution.ps1 -IncludeHeads %IncludeHeads%