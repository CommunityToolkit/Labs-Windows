using System;
using System.Diagnostics;

namespace CommunityToolkit.Labs.Core.SourceGenerators.Attributes;

/// <summary>
/// Represents a boolean sample option that the user can manipulate and the XAML can bind to.
/// </summary>
/// <remarks>
/// Using this attribute will automatically generate a dependency property that you can bind to in XAML.
/// </remarks>
[Conditional("COMMUNITYTOOLKIT_KEEP_SAMPLE_ATTRIBUTES")]
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class ToolkitSampleBoolOptionAttribute : ToolkitSampleOptionBaseAttribute
{
    /// <summary>
    /// Creates a new instance of <see cref="ToolkitSampleBoolOptionAttribute"/>.
    /// </summary>
    public ToolkitSampleBoolOptionAttribute(string name, string label, bool defaultState, string? title = null)
        : base(name, defaultState, title)
    {
        Label = label;
    }

    /// <summary>
    /// The source generator-friendly type name used for casting.
    /// </summary>
    public override string TypeName { get; } = "bool";

    /// <summary>
    /// A label to display along the boolean option.
    /// </summary>
    public string Label { get; }
}
