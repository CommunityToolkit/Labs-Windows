// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.Labs.Core.SourceGenerators.Metadata;

namespace CommunityToolkit.Labs.Shared.Renderers;

/// <summary>
/// Selects a sample option template for the provided <see cref="IGeneratedToolkitSampleOptionViewModel"/>.
/// </summary>
internal class GeneratedSampleOptionTemplateSelector : DataTemplateSelector
{
    public DataTemplate? BoolOptionTemplate { get; set; }

    public DataTemplate? MultiChoiceOptionTemplate { get; set; }

    public DataTemplate? SliderOptionTemplate { get; set; }

    public DataTemplate? NumberBoxOptionTemplate { get; set; }

    public DataTemplate? TextOptionTemplate { get; set; }

    protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
    {
        return item switch
        {
            ToolkitSampleBoolOptionMetadataViewModel => BoolOptionTemplate ?? base.SelectTemplateCore(item, container),
            ToolkitSampleMultiChoiceOptionMetadataViewModel => MultiChoiceOptionTemplate ?? base.SelectTemplateCore(item, container),
            ToolkitSampleNumericOptionMetadataViewModel { ShowAsNumberBox: true } => NumberBoxOptionTemplate ?? base.SelectTemplateCore(item, container),
            ToolkitSampleNumericOptionMetadataViewModel { ShowAsNumberBox: false } => SliderOptionTemplate ?? base.SelectTemplateCore(item, container),
            ToolkitSampleTextOptionMetadataViewModel => TextOptionTemplate ?? base.SelectTemplateCore(item, container),
            _ => base.SelectTemplateCore(item, container),
        };
    }
}
