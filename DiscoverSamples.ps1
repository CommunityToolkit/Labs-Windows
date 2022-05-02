$templatedSampleProjectReferencesDefinitionMarker = "[TemplatedSampleProjectReferences]"
$sampleRefsPropsTemplatePath = 'common/Labs.SampleRefs.props.template';
$generatedSampleRefsPropsPath = 'common/Labs.SampleRefs.props';

# Execute ProjectReference generation for all heads
$sampleRefsPropsTemplate = Get-Content -Path $sampleRefsPropsTemplatePath;
Write-Output "Loaded sample ProjectReference template from $sampleRefsPropsTemplatePath";

# Add sample projects
foreach ($sampleProjectPath in Get-ChildItem -Recurse -Path 'labs/*/samples/*.Sample/*.Sample.csproj') {
	$relativePath =  Resolve-Path -Relative -Path $sampleProjectPath;
	$relativePath = $relativePath.TrimStart('.\');
	$projectName = [System.IO.Path]::GetFileNameWithoutExtension($relativePath);

	Write-Host "Adding $projectName to project references";

	$projectReferenceDefinition = "<ProjectReference Include=`"`$(RepositoryDirectory)$relativePath`" />";

	$sampleRefsPropsTemplate = $sampleRefsPropsTemplate -replace [regex]::escape($templatedSampleProjectReferencesDefinitionMarker), ($templatedSampleProjectReferencesDefinitionMarker + "
	" + $projectReferenceDefinition);
}

# Add library projects
foreach ($sampleProjectPath in Get-ChildItem -Recurse -Path 'labs/*/src/*.csproj') {
	$relativePath = Resolve-Path -Relative -Path $sampleProjectPath;
	$relativePath = $relativePath.TrimStart('.\');
	$projectName = [System.IO.Path]::GetFileNameWithoutExtension($relativePath);

	Write-Host "Adding $projectName to project references";

	$projectReferenceDefinition = "<ProjectReference Include=`"`$(RepositoryDirectory)$relativePath`" />";

	$sampleRefsPropsTemplate = $sampleRefsPropsTemplate -replace [regex]::escape($templatedSampleProjectReferencesDefinitionMarker), ($templatedSampleProjectReferencesDefinitionMarker + "
	" + $projectReferenceDefinition);
}

$sampleRefsPropsTemplate = $sampleRefsPropsTemplate -replace [regex]::escape($templatedSampleProjectReferencesDefinitionMarker), "";

# Save
Set-Content -Path $generatedSampleRefsPropsPath -Value $sampleRefsPropsTemplate;
Write-Output "Sample project references generated at $generatedSampleRefsPropsPath";