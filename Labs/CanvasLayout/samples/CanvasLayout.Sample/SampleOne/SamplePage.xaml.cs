// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.Labs.Core;
using CommunityToolkit.Labs.Core.Attributes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace CanvasLayout.Sample.SampleOne
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    [ToolkitSample(id: nameof(SamplePage), "Canvas Layout", ToolkitSampleCategory.Controls, ToolkitSampleSubcategory.Layout, description: "A canvas-like VirtualizingPanel for use in an ItemsControl")]
    public sealed partial class SamplePage : Page
    {
        public SamplePage()
        {
            this.InitializeComponent();
        }
    }
}
