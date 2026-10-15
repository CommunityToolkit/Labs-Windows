// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace DependencyPropertyExperiment.Samples;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
[ToolkitSample(id: nameof(PlaceholderSample), "Placeholder sample", description: "A sample that does not appear in the sample gallery, acts as a placeholder to work around incremental build issue for no-sample components.")]
public sealed partial class PlaceholderSample : Page
{
    public PlaceholderSample()
    {
        this.InitializeComponent();
    }
}
