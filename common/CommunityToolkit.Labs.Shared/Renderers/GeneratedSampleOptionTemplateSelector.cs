// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.Labs.Core.SourceGenerators.Metadata;

#if WINAPPSDK
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endif

namespace CommunityToolkit.Labs.Shared.Renderers;

/// <summary>
/// Selects a sample option template for the provided <see cref="IGeneratedToolkitSampleOptionViewModel"/>.
/// </summary>
internal class GeneratedSampleOptionTemplateSelector : DataTemplateSelector
{
    public DataTemplate? BoolOptionTemplate { get; set; }

    public DataTemplate? MultiChoiceOptionTemplate { get; set; }

    protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
    {
        return item switch
        {
            ToolkitSampleBoolOptionMetadataViewModel => BoolOptionTemplate ?? base.SelectTemplateCore(item, container),
            ToolkitSampleMultiChoiceOptionMetadataViewModel => MultiChoiceOptionTemplate ?? base.SelectTemplateCore(item, container),
            _ => base.SelectTemplateCore(item, container),
        };
    }
}
