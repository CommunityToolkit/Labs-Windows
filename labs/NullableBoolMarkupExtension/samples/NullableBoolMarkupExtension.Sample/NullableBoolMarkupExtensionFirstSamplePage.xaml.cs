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


namespace NullableBoolMarkupExtension.Sample
{
    [ToolkitSampleBoolOption("IsTextVisible", "IsVisible", true)]
    // Single values without a colon are used for both label and value.
    // To provide a different label for the value, separate with a colon surrounded by a single space on both sides ("label : value").
    [ToolkitSampleMultiChoiceOption("TextSize", title: "Text size", "Small : 12", "Normal : 16", "Big : 32")]
    [ToolkitSampleMultiChoiceOption("TextFontFamily", title: "Font family", "Segoe UI", "Arial", "Consolas")]
    [ToolkitSampleMultiChoiceOption("TextForeground", title: "Text foreground",
        "Teal       : #0ddc8c",
        "Sand       : #e7a676",
        "Dull green : #5d7577")]
    
    [ToolkitSample(id: nameof(NullableBoolMarkupExtensionFirstSamplePage), "Simple Options", description: "A sample page for showing how to do simple options.")]
    public sealed partial class NullableBoolMarkupExtensionFirstSamplePage : Page
    {
        public NullableBoolMarkupExtensionFirstSamplePage()
        {
            this.InitializeComponent();
        }
    }
}
