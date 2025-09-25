// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.UI.Xaml.Media.Imaging;

namespace Extensions.AccentExtractorExperiment.Samples;

/// <summary>
/// An example sample page of a custom control inheriting from Panel.
/// </summary>
[ToolkitSample(id: nameof(AccentExtractorSample), "Accent Extractor Extensions", description: $"A sample for showing how the accent extractor can be used.")]
public sealed partial class AccentExtractorSample : Page
{
    public AccentExtractorSample()
    {
        this.InitializeComponent();
    }
}
