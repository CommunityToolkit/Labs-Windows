using CommunityToolkit.Labs.Core.SourceGenerators.Metadata;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;

#if WINAPPSDK
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
#endif

namespace CommunityToolkit.Labs.Shared.Renderers
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ToolkitDocumentationRenderer : Page
    {
        public ToolkitDocumentationRenderer()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// The documentation content.
        /// </summary>
        public string? DocumentationText
        {
            get { return (string?)GetValue(DocumentationTextProperty); }
            set { SetValue(DocumentationTextProperty, value); }
        }

        /// <summary>
        /// The backing <see cref="DependencyProperty"/> for the <see cref="DocumentationText"/> property.
        /// </summary>
        public static readonly DependencyProperty DocumentationTextProperty =
            DependencyProperty.Register(nameof(DocumentationText), typeof(string), typeof(ToolkitDocumentationRenderer), new PropertyMetadata(null));

        /// <summary>
        /// The YAML front matter metadata about this documentation file.
        /// </summary>
        public ToolkitFrontMatter? Metadata
        {
            get { return (ToolkitFrontMatter?)GetValue(MetadataProperty); }
            set { SetValue(MetadataProperty, value); }
        }

        /// <summary>
        /// The backing <see cref="DependencyProperty"/> for the <see cref="Metadata"/> property.
        /// </summary>
        public static readonly DependencyProperty MetadataProperty =
            DependencyProperty.Register(nameof(Metadata), typeof(ToolkitFrontMatter), typeof(ToolkitDocumentationRenderer), new PropertyMetadata(null, OnMetadataPropertyChanged));

        private static async void OnMetadataPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            if (dependencyObject is ToolkitDocumentationRenderer renderer &&
                renderer.Metadata != null &&
                args.OldValue != args.NewValue)
            {
                renderer.DocumentationText = await GetDocumentationFileContents(renderer.Metadata);
            }
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            Metadata = (ToolkitFrontMatter)e.Parameter;
        }

        public static async Task<string?> GetDocumentationFileContents(ToolkitFrontMatter metadata)
        {
            // TODO: Path will be different if single vs. multi-sample?
            var fileUri = new Uri($"ms-appx:///{metadata.FilePath}");

            try
            {
                var file = await StorageFile.GetFileFromApplicationUriAsync(fileUri);
                var textContents = await FileIO.ReadTextAsync(file);

                // Remove YAML
                var blocks = textContents.Split("---", StringSplitOptions.RemoveEmptyEntries);

                return blocks.LastOrDefault();
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
