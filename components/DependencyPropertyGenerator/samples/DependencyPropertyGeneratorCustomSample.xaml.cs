// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.WinUI;

namespace DependencyPropertyGenerator.Samples;

[ToolkitSample(id: nameof(DependencyPropertyGeneratorCustomSample), "Custom control", description: $"A sample for showing how to use {nameof(GeneratedDependencyPropertyAttribute)} to register dependency properties.")]
public sealed partial class DependencyPropertyGeneratorCustomSample : Page
{
    public DependencyPropertyGeneratorCustomSample()
    {
        this.InitializeComponent();
    }
}
