// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.Tooling.SampleGen.Attributes;

/// <summary>
/// Registers a control as a toolkit sample using the provided data.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class ToolkitSampleAttribute : Attribute
{
    /// <summary>
    /// Creates a new instance of <see cref="ToolkitSampleAttribute"/>.
    /// </summary>
    /// <param name="id">A unique identifier for this sample, used by the sample system (across all samples).</param>
    /// <param name="displayName">The display name for this sample page.</param>
    /// <param name="description">A short description of this sample.</param>
    public ToolkitSampleAttribute(string id, string displayName, string description)
    {
        Id = id;
        DisplayName = displayName;
        Description = description;
    }

    /// <summary>
    /// A unique identifier for this sample, used by the sample system.
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// The display name for this sample page.
    /// </summary>
    public string DisplayName { get; }

    /// <summary>
    /// A short description of this sample.
    /// </summary>
    public string Description { get; }
}
