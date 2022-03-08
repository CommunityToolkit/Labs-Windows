using System;
using System.Diagnostics;

namespace CommunityToolkit.Labs.Core.SourceGenerators.Attributes;

/// <summary>
/// Represents a boolean sample option.
/// </summary>
/// <remarks>
/// Using this attribute will automatically generate an <see cref="INotifyPropertyChanged"/>-enabled property
/// that you can bind to in XAML, and displays an options pane alonside your sample which allows the user to manipulate the property.
/// <para/>
/// </remarks>
[Conditional("COMMUNITYTOOLKIT_KEEP_SAMPLE_ATTRIBUTES")]
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class ToolkitSampleMultiChoiceOptionAttribute : ToolkitSampleOptionBaseAttribute
{
    /// <summary>
    /// Creates a new instance of <see cref="ToolkitSampleBoolOptionAttribute"/>.
    /// </summary>
    /// <param name="bindingName">The name of the generated property, which you can bind to in XAML.</param>
    /// <param name="label">The displayed text shown beside this option.</param>
    /// <param name="value">The value to provide in XAML when this item is selected.</param>
    /// <param name="title">A title to display on top of this option.</param>
    public ToolkitSampleMultiChoiceOptionAttribute(string bindingName, string label, string value, string? title = null)
        : base(bindingName, null, title)
    {
        Label = label;
        Value = value;
    }

    /// <summary>
    /// The source generator-friendly type name used for casting.
    /// </summary>
    internal override string TypeName { get; } = "string";

    /// <summary>
    /// The displayed text shown beside this option.
    /// </summary>
    public string Label { get; private set; }

    /// <summary>
    /// The value to provide in XAML when this item is selected.
    /// </summary>
    public string Value { get; private set; }
}
