// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.Tooling.SampleGen.Attributes;

/// <summary>
/// Represents a boolean sample option.
/// </summary>
/// <remarks>
/// Using this attribute will automatically generate an <see cref="INotifyPropertyChanged"/>-enabled property
/// that you can bind to in XAML, and displays an options pane alonside your sample which allows the user to manipulate the property.
/// <para/>
/// </remarks>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class ToolkitSampleNumericOptionAttribute : ToolkitSampleOptionBaseAttribute
{
    /// <summary>
    /// Creates a new instance of <see cref="ToolkitSampleNumericOptionAttribute"/>.
    /// </summary>
    /// <param name="bindingName">The name of the generated property, which you can bind to in XAML.</param>
    /// <param name="choices">A list of the choices to display to the user. Can be literal values, or labeled values. Use a " : " separator (single colon surrounded by at least 1 whitespace) to separate a label from a value.</param>
    /// <param name="title">A title to display on top of this option.</param>
    public ToolkitSampleNumericOptionAttribute(string bindingName, double initial = 0, double min = 0, double max = 10, double step = 1, bool showAsNumberBox = false)
        : base(bindingName, null)
    {
        Initial = initial;
        Min = min;
        Max = max;
        Step = step;
        ShowAsNumberBox = showAsNumberBox;
    }

    /// <summary>
    /// The default start value.
    /// </summary>
    public double Initial { get; }

    /// <summary>
    /// The minimal value.
    /// </summary>
    public double Min { get; }

    /// <summary>
    /// The maximum value.
    /// </summary>
    public double Max { get; }

    /// <summary>
    /// The step value.
    /// </summary>
    public double Step { get; }

    /// <summary>
    /// Determines if a Slider or NumberBox is shown.
    /// </summary>
    public bool ShowAsNumberBox { get; }

    /// <summary>
    /// The source generator-friendly type name used for casting.
    /// </summary>
    internal override string TypeName { get; } = "double";
}
