// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CommunityToolkit.Labs.Core.SourceGenerators;
using CommunityToolkit.Labs.Core.SourceGenerators.Attributes;
using System.Runtime.InteropServices.WindowsRuntime;

//-:cnd:noEmit
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
//+:cnd:noEmit


namespace ProjectTemplate.Sample
{
    [ToolkitSampleBoolOption("IsTextVisible", "IsVisible", true)]

    [ToolkitSampleMultiChoiceOption("TextForeground", label: "Teal", value: "#0ddc8c", title: "Text foreground")]
    [ToolkitSampleMultiChoiceOption("TextForeground", label: "Sand", value: "#e7a676")]
    [ToolkitSampleMultiChoiceOption("TextForeground", label: "Dull green", value: "#5d7577")]

    [ToolkitSampleMultiChoiceOption("TextSize", label: "Small", value: "12", title: "Text size")]
    [ToolkitSampleMultiChoiceOption("TextSize", label: "Normal", value: "16")]
    [ToolkitSampleMultiChoiceOption("TextSize", label: "Big", value: "32")]

    [ToolkitSampleMultiChoiceOption("TextFontFamily", label: "Segoe UI", value: "Segoe UI")]
    [ToolkitSampleMultiChoiceOption("TextFontFamily", label: "Arial", value: "Arial")]
    [ToolkitSampleMultiChoiceOption("TextFontFamily", label: "Consolas", value: "Consolas")]

    [ToolkitSample(id: nameof(ProjectTemplateFirstSamplePage), "Simple Options", ToolkitSampleCategory.Controls, ToolkitSampleSubcategory.Layout, description: "A sample page for showing how to do simple options.")]
    public sealed partial class ProjectTemplateFirstSamplePage : Page
    {
        public ProjectTemplateFirstSamplePage()
        {
            this.InitializeComponent();
        }
    }
}
