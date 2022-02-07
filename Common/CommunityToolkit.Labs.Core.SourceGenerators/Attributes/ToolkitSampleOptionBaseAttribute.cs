using System;
using System.Diagnostics;

namespace CommunityToolkit.Labs.Core.SourceGenerators.Attributes;

/// <summary>
/// Represents a sample option that the user can manipulate and the XAML can bind to.
/// </summary>
[Conditional("COMMUNITYTOOLKIT_KEEP_SAMPLE_ATTRIBUTES")]
public abstract class ToolkitSampleOptionBaseAttribute : Attribute
{
    /// <summary>
    /// Creates a new instance of <see cref="ToolkitSampleBoolOptionAttribute"/>.
    /// </summary>
    public ToolkitSampleOptionBaseAttribute(string bindingName, object? defaultState, string? title = null)
    {
        Title = title;
        Name = bindingName;
        DefaultState = defaultState;
    }

    /// <summary>
    /// A name that you can bind to in your XAML.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// The default state.
    /// </summary>
    public object? DefaultState { get; }

    /// <summary>
    /// A title to display on top of the option.
    /// </summary>
    public string? Title { get; }

    /// <summary>
    /// The source generator-friendly type name used for casting.
    /// </summary>
    public abstract string TypeName { get; }
}
