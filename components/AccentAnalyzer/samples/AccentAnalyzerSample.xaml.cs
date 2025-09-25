// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.UI.Xaml.Media.Imaging;

namespace AccentAnalyzerExperiment.Samples;

/// <summary>
/// An example sample page of a custom control inheriting from Panel.
/// </summary>
[ToolkitSample(id: nameof(AccentAnalyzerSample), "AccentAnalyzer helper", description: $"A sample for showing how the accent analyzer can be used.")]
public sealed partial class AccentAnalyzerSample : Page
{
    public AccentAnalyzerSample()
    {
        this.InitializeComponent();
    }
}
