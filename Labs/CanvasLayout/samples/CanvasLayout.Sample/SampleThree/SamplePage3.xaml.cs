// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using CommunityToolkit.Labs.Core.SourceGenerators;
using CommunityToolkit.Labs.Core.SourceGenerators.Attributes;
using CommunityToolkit.Labs.WinUI;

#if !WINAPPSDK
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
#else
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
#endif

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace CanvasLayout.Sample.SampleThree
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    [ToolkitSample(id: nameof(SamplePage3), "Canvas Layout", ToolkitSampleCategory.Controls, ToolkitSampleSubcategory.Layout, description: "A canvas-like VirtualizingLayout for use in an ItemsRepeater")]
    public sealed partial class SamplePage3 : Page
    {
        public ObservableCollection<CanvasItem> Items = new()
        {
            new() { Left = 100, Top = 50, Width = 100, Height = 100, Text = "Item 1" },
            new() { Left = 400, Top = 250, Width = 200, Height = 200, Text = "Item 2" },
            new() { Left = 200, Top = 500, Width = 100, Height = 100, Text = "Item 3" },
            new() { Left = 1200, Top = 2500, Width = 100, Height = 100, Text = "Item 4" },
            new() { Left = 2200, Top = 1500, Width = 100, Height = 100, Text = "Item 5" },
            new() { Left = 1200, Top = 3500, Width = 100, Height = 100, Text = "Item 6" },
        };

        public SamplePage3()
        {
            this.InitializeComponent();
        }
    }

    public class CanvasItem : CanvasLayoutItem
    {
        public string Text { get; set; }
    }
}
