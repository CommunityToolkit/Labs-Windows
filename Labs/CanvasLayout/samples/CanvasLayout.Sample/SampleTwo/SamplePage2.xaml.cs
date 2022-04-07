// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.Labs.Core.SourceGenerators;
using CommunityToolkit.Labs.Core.SourceGenerators.Attributes;

#if WINAPPSDK
using Microsoft.UI.Xaml.Controls;
#else
using Windows.UI.Xaml.Controls;
#endif

namespace CanvasLayout.Sample.SampleTwo
{
    [ToolkitSample(id: nameof(SamplePage2), "Custom options", ToolkitSampleCategory.Controls, ToolkitSampleSubcategory.Layout, description: "An empty sample used to demonstrate the sample system.")]
    public sealed partial class SamplePage2 : UserControl
    {
        public SamplePage2()
        {
            this.InitializeComponent();
        }
    }
}
