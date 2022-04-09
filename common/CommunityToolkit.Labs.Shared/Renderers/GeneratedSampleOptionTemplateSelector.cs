using CommunityToolkit.Labs.Core.SourceGenerators.Metadata;
using System;
using System.Collections.Generic;
using System.Text;

#if WINAPPSDK
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endif

namespace CommunityToolkit.Labs.Shared.Renderers
{
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
}
