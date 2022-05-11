using System;
using System.Linq;

namespace CommunityToolkit.Labs.Core.SourceGenerators.Attributes;

/// <summary>
/// Represents a boolean sample option.
/// </summary>
/// <remarks>
/// Using this attribute will automatically generate an <see cref="INotifyPropertyChanged"/>-enabled property
/// that you can bind to in XAML, and displays an options pane alonside your sample which allows the user to manipulate the property.
/// <para/>
/// </remarks>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class ToolkitSampleMultiChoiceOptionAttribute : ToolkitSampleOptionBaseAttribute
{
    /// <summary>
    /// Creates a new instance of <see cref="ToolkitSampleBoolOptionAttribute"/>.
    /// </summary>
    /// <param name="bindingName">The name of the generated property, which you can bind to in XAML.</param>
    /// <param name="choices">A list of the choices to display to the user. Can be literal values, or labeled values. Use a " : " separator (single colon surrounded by at least 1 whitespace) to separate a label from a value.</param>
    /// <param name="title">A title to display on top of this option.</param>
    public ToolkitSampleMultiChoiceOptionAttribute(string bindingName, string? title = null, params string[] choices)
        : base(bindingName, null, title)
    {
        Choices = choices.Select(x =>
        {
            if (x.Contains(" : "))
            {
                var parts = x.Split(new string[] { " : " }, StringSplitOptions.RemoveEmptyEntries);
                return new MultiChoiceOption(parts[0].TrimEnd(), parts[1].TrimStart());
            }

            return new MultiChoiceOption(x, x);
        }).ToArray();
    }

    /// <summary>
    /// A collection of choices to display in the options pane.
    /// </summary>
    public MultiChoiceOption[] Choices { get; }

    /// <summary>
    /// The source generator-friendly type name used for casting.
    /// </summary>
    internal override string TypeName { get; } = "string";
}
