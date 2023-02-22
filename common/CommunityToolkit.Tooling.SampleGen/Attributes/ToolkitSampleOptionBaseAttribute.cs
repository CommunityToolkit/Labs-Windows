// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.Tooling.SampleGen.Attributes;

/// <summary>
/// Represents an abstraction of a sample option that the user can manipulate and the XAML can bind to.
/// </summary>
public abstract class ToolkitSampleOptionBaseAttribute : Attribute
{
    /// <summary>
    /// Creates a new instance of <see cref="ToolkitSampleOptionBaseAttribute"/>.
    /// </summary>
    /// <param name="bindingName">The name of the generated property, which you can bind to in XAML.</param>
    /// <param name="defaultState">The initial value for the bound property.</param>
    /// <param name="title">A title to display on top of this option.</param>
    public ToolkitSampleOptionBaseAttribute(string bindingName, object? defaultState)
    {
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
    public string? Title { get; set; }

    /// <summary>
    /// The source generator-friendly type name used for casting.
    /// </summary>
    internal abstract string TypeName { get; }
}
