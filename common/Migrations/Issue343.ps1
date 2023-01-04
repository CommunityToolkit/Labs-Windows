# Find all .props files with "Labs" in the name in the parent directory
$propsFiles = Get-ChildItem "$PSScriptRoot/../" -Recurse -Filter "Labs*.props" -File

foreach ($propsFile in $propsFiles) {
    # Construct the new name for the .props file
    $newName = $propsFile.Name -replace "Labs.", ""

    # Find all files in the parent directory that contain a reference to the old .props file name
    $referencingFiles = Get-ChildItem -Include "*.csproj", "*.ps1", "*.props" -Path "$PSScriptRoot/../../" -Recurse -File | Where-Object {
        (Get-Content $_.FullName -Encoding utf8) -match $propsFile.Name
    }

    # Update the reference to the .props file in each referencing file
    foreach ($referencingFile in $referencingFiles) {
        # Read the contents of the referencing file into memory
        $contents = Get-Content -Encoding utf8 $referencingFile.FullName

        # Update the references to the .props file in the contents
        $contents = $contents -replace $propsFile.Name, $newName

        # Write the updated contents back to the referencing file
        $contents | Set-Content $referencingFile.FullName
    }

    # Rename the .props file
    Rename-Item $propsFile.FullName $newName
}
