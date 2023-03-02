// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace ShimmerExperiment.Samples;

[ToolkitSampleBoolOption("IsLoading", true, Title = "IsLoading")]

[ToolkitSample(id: nameof(ShimmerTemplatedSample), "Basic Shimmer", description: "A sample that shows how to use a shimmer loading indicator.")]
public sealed partial class ShimmerTemplatedSample : Page
{
    public ShimmerTemplatedSample()
    {
        this.InitializeComponent();
    }
}
