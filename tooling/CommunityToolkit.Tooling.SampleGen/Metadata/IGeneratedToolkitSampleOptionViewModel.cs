// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.Tooling.SampleGen.Metadata;

/// <summary>
/// A common interface for all generated toolkit sample options.
/// Implementations of this interface are updated from the sample pane UI, and referenced by the generated property.
/// </summary>
/// <remarks>
/// Must implement <see cref="INotifyPropertyChanged"/> to notify when the user changes a value in the sample pane UI.
/// <para/>
/// However, the <see cref="Name"/> must be emitted as the changed property name when <see cref="Value"/> updates, so the
/// propogated IPNC event can notify the sample control of the change.
/// </remarks>
public interface IGeneratedToolkitSampleOptionViewModel : INotifyPropertyChanged
{
    /// <summary>
    /// The current value. Can be updated by the user via the sample pane UI.
    /// <para/>
    /// A generated property's getter and setter directly references this value, making it available to bind to.
    /// </summary>
    public object? Value { get; set; }

    /// <summary>
    /// A unique identifier name for this option.
    /// </summary>
    /// <remarks>
    /// Used by the sample system to match up <see cref="ToolkitSampleBoolOptionMetadataViewModel"/> to the original <see cref="ToolkitSampleBoolOptionAttribute"/> and the control that declared it.
    /// </remarks>
    public string Name { get; }
}
