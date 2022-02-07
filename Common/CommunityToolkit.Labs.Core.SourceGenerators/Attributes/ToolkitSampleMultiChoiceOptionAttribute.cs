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
public sealed class ToolkitSampleMultiChoiceOptionAttribute : ToolkitSampleOptionBaseAttribute
{
    /// <summary>
    /// Creates a new instance of <see cref="ToolkitSampleBoolOptionAttribute"/>.
    /// </summary>
    public ToolkitSampleMultiChoiceOptionAttribute(string bindingName, string label, string value, string? title = null)
        : base(bindingName, null, title)
    {
        Label = label;
        Value = value;
    }

    /// <summary>
    /// The source generator-friendly type name used for casting.
    /// </summary>
    public override string TypeName { get; } = "string";

    /// <summary>
    /// The displayed text shown beside this option.
    /// </summary>
    public string Label { get; private set; }

    /// <summary>
    /// The value to provide in XAML when this item is selected.
    /// </summary>
    public string Value { get; private set; }
}
