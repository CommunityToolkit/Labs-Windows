using System;
using System.Diagnostics;

namespace CommunityToolkit.Labs.Core.SourceGenerators.Attributes;

/// <summary>
/// Represents a boolean sample option that the user can manipulate and the XAML can bind to.
/// </summary>
/// <remarks>
/// Using this attribute will automatically generate an <see cref="INotifyPropertyChanged"/>-enabled property
/// that you can bind to in XAML, and displays an options pane alonside your sample which allows the user to manipulate the property.
/// </remarks>
[Conditional("COMMUNITYTOOLKIT_KEEP_SAMPLE_ATTRIBUTES")]
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class ToolkitSampleBoolOptionAttribute : ToolkitSampleOptionBaseAttribute
{
    /// <summary>
    /// Creates a new instance of <see cref="ToolkitSampleBoolOptionAttribute"/>.
    /// </summary>
    /// <param name="bindingName">The name of the generated property, which you can bind to in XAML.</param>
    /// <param name="defaultState">The initial value for the bound property.</param>
    /// <param name="title">A title to display on top of this option.</param>
    public ToolkitSampleBoolOptionAttribute(string bindingName, string label, bool defaultState, string? title = null)
        : base(bindingName, defaultState, title)
    {
        Label = label;
    }

    /// <summary>
    /// The source generator-friendly type name used for casting.
    /// </summary>
    internal override string TypeName { get; } = "bool";

    /// <summary>
    /// A label to display along the boolean option.
    /// </summary>
    public string Label { get; }
}
