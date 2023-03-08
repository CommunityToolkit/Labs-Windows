// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.Tooling.SampleGen.Metadata;

#if !WINAPPSDK
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#else
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endif

namespace CommunityToolkit.App.Shared;

public class DocOrSampleTemplateSelector : DataTemplateSelector
{
    public DataTemplate? Document { get; set; }
    public DataTemplate? Sample { get; set; }

    protected override DataTemplate SelectTemplateCore(object item) => item switch
    {
        ToolkitFrontMatter _ => Document!, // Used for concrete type in TabbedPage
        ToolkitSampleMetadata _ => Sample!,
        _ => Document! // Used for string type in ToolkitDocumentationRenderer
    };

    protected override DataTemplate SelectTemplateCore(object item, DependencyObject container) => SelectTemplateCore(item);
}
