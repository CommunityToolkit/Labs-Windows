// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.Tooling.SampleGen.Attributes;

namespace CommunityToolkit.Tooling.SampleGen;

public partial class ToolkitSampleMetadataGenerator
{
    /// <summary>
    /// Used to hold interim data during the source generation process by <see cref="ToolkitSampleMetadataGenerator"/>.
    /// </summary>
    /// <remarks>
    /// A new record must be used instead of using <see cref="ToolkitSampleMetadata"/> directly
    /// because we cannot <c>Type.GetType</c> using the <paramref name="SampleAssemblyQualifiedName"/>,
    /// but we can safely generate a type reference in the final output using <c>typeof(AssemblyQualifiedName)</c>.
    /// </remarks>
    private sealed record ToolkitSampleRecord(
        string Id,
        string DisplayName,
        string Description,
        string SampleAssemblyQualifiedName,
        string? SampleOptionsAssemblyQualifiedName,
        IEnumerable<ToolkitSampleOptionBaseAttribute>? GeneratedSampleOptions = null);
}
