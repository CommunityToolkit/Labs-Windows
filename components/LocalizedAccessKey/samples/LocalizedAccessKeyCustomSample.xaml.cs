// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.WinUI.Controls;

namespace LocalizedAccessKeyExperiment.Samples;

/// <summary>
/// An example of the <see cref="LocalizedAccessKey"/> usage.
/// </summary>
[ToolkitSample(id: nameof(LocalizedAccessKeyCustomSample), "LocalizedAccessKey extension sample", description: $"A sample for showing how to use the {nameof(LocalizedAccessKey)} markup extension.")]
public sealed partial class LocalizedAccessKeyCustomSample : Page
{
    public LocalizedAccessKeyCustomSample() => InitializeComponent();
}
