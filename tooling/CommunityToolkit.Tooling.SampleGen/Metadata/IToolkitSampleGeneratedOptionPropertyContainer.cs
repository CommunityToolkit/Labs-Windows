// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.Tooling.SampleGen.Metadata;

/// <summary>
/// Implementors of this class contain one or more source-generated properties
/// which are bound to in the XAML of a toolkit sample
/// and manipulated from a data-generated options pane.
/// </summary>
public interface IToolkitSampleGeneratedOptionPropertyContainer
{
    /// <summary>
    /// Holds a reference to all generated ViewModels that act
    /// as a proxy between the current actual value and the
    /// generated properties which consume them.
    /// </summary>
    public IEnumerable<IGeneratedToolkitSampleOptionViewModel>? GeneratedPropertyMetadata { get; set; }
}
