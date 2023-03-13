// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.Tooling.SampleGen.Attributes;

/// <summary>
/// Represents a boolean sample option that the user can manipulate and the XAML can bind to.
/// </summary>
/// <remarks>
/// Using this attribute will automatically generate an <see cref="INotifyPropertyChanged"/>-enabled property
/// that you can bind to in XAML, and displays an options pane alonside your sample which allows the user to manipulate the property.
/// </remarks>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class ToolkitSampleTextOptionAttribute : ToolkitSampleOptionBaseAttribute
{
    /// <summary>
    /// Creates a new instance of <see cref="ToolkitSampleTextOptionAttribute"/>.
    /// </summary>
    /// <param name="bindingName">The name of the generated property, which you can bind to in XAML.</param>
    /// <param name="defaultState">The initial value for the bound property.</param>
    /// <param name="title">A title to display on top of this option.</param>
    public ToolkitSampleTextOptionAttribute(string bindingName, string? placeholderText = null)
        : base(bindingName, placeholderText)
    {
        PlaceholderText = placeholderText;
    }

    /// <summary>
    /// The source generator-friendly type name used for casting.
    /// </summary>
    internal override string TypeName { get; } = "string";

    /// <summary>
    /// A label to display along the boolean option.
    /// </summary>
    public string? PlaceholderText { get; }
}
