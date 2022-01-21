using System;

namespace CommunityToolkit.Labs.Core.Attributes;

/// <summary>
/// Registers a control as the options panel for a toolkit sample.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class ToolkitSampleOptionsPaneAttribute : Attribute
{
    /// <summary>
    /// Creates a new instance of <see cref="ToolkitSampleOptionsPaneAttribute"/>.
    /// </summary>
    /// <param name="sampleId">The unique identifier of a toolkit sample, provided via <see cref="ToolkitSampleAttribute.Id"/>.</param>
    public ToolkitSampleOptionsPaneAttribute(string sampleId)
    {
        SampleId = sampleId;
    }

    /// <summary>
    /// The unique identifier of a toolkit sample, provided via <see cref="ToolkitSampleAttribute.Id"/>.
    /// </summary>
    public string SampleId { get; }
}
