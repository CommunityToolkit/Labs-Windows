// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.Labs.Core.SourceGenerators;
using CommunityToolkit.Labs.Core.SourceGenerators.Metadata;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
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
        private const string MarkdownRegexSampleTagExpression = @"^>\s*\[!SAMPLE\s*(?<sampleid>.*)\s*\]\s*$";
        private static readonly Regex MarkdownRegexSampleTag = new Regex(MarkdownRegexSampleTagExpression, RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Multiline);

        public ToolkitDocumentationRenderer()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// List of referenced samples in this page.
        /// </summary>
        public List<ToolkitSampleMetadata> Samples
        {
            get { return (List<ToolkitSampleMetadata>)GetValue(SamplesProperty); }
            set { SetValue(SamplesProperty, value); }
        }

        /// <summary>
        /// The backing <see cref="DependencyProperty"/> for the <see cref="Samples"/> property.
        /// </summary>
        public static readonly DependencyProperty SamplesProperty
            =
            DependencyProperty.Register(nameof(Samples), typeof(List<ToolkitSampleMetadata>), typeof(ToolkitDocumentationRenderer), new PropertyMetadata(null));

        /// <summary>
        /// Intermixed list of string doc snippets for Markdown and <see cref="ToolkitSampleMetadata"/>
        /// objects for samples.
        /// </summary>
        public ObservableCollection<object> DocsAndSamples = new();

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
                await renderer.LoadData();
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            Metadata = (ToolkitFrontMatter)e.Parameter;
        }

        private async Task LoadData()
        {
            if (Metadata is null)
            {
                return;
            }

            // TODO: Show list of samples in a side-panel
            List<ToolkitSampleMetadata> samples = new();
            if (Metadata.SampleIdReferences != null && Metadata.SampleIdReferences.Length > 0 &&
                !string.IsNullOrWhiteSpace(Metadata.SampleIdReferences[0]))
            {
                foreach (var sampleid in Metadata.SampleIdReferences)
                {
                    // We don't check here for key as we validate with SG.
                    samples.Add(ToolkitSampleRegistry.Listing[sampleid]);
                }
            }
            Samples = samples;

            var doctext = await GetDocumentationFileContents(Metadata);

            var matches = MarkdownRegexSampleTag.Matches(doctext);

            DocsAndSamples.Clear();
            if (matches.Count == 0)
            {
                DocsAndSamples.Add(doctext);
            }
            else
            {
                int index = 0;
                foreach (Match match in matches)
                {
                    DocsAndSamples.Add(doctext.Substring(index, match.Index - index - 1));
                    DocsAndSamples.Add(ToolkitSampleRegistry.Listing[match.Groups["sampleid"].Value]);
                    index = match.Index + match.Length;
                }

                // Put rest of text at end
                DocsAndSamples.Add(doctext.Substring(index));
            }
        }

        private void SampleListHyperlink_Click(object sender, RoutedEventArgs e)
        {
            if (sender is HyperlinkButton btn && btn.DataContext is ToolkitSampleMetadata metadata)
            {
                var container = DocItemsControl.ContainerFromItem(metadata) as UIElement;
                container?.StartBringIntoView();
            }
        }

        private static async Task<string> GetDocumentationFileContents(ToolkitFrontMatter metadata)
        {
            // TODO: Path will be different if single vs. multi-sample?
            var fileUri = new Uri($"ms-appx:///{metadata.FilePath}");

            try
            {
                var file = await StorageFile.GetFileFromApplicationUriAsync(fileUri);
                var textContents = await FileIO.ReadTextAsync(file);

                // Remove YAML
                var blocks = textContents.Split("---", StringSplitOptions.RemoveEmptyEntries);

                return blocks.LastOrDefault() ?? "Couldn't find content after YAML Front Matter removal.";
            }
            catch (Exception e)
            {
                return $"Exception Encountered Loading file '{metadata.FilePath}':\n{e.Message}\n{e.StackTrace}";
            }
        }
    }
}
