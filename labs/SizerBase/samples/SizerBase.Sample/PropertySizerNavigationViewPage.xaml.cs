// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.Labs.Core.SourceGenerators;
using CommunityToolkit.Labs.Core.SourceGenerators.Attributes;

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

namespace SizerBase.Sample;

[ToolkitSample(id: nameof(PropertySizerNavigationViewPage), "NavigationView Shelf", description: "Shows how to create an expandable shelf using a NavigationView and PropertySizer.")]
public sealed partial class PropertySizerNavigationViewPage : Page
{
    public PropertySizerNavigationViewPage()
    {
        this.InitializeComponent();
    }
}
