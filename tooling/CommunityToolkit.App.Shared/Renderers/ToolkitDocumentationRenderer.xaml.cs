// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.Tooling.SampleGen;
using CommunityToolkit.Tooling.SampleGen.Metadata;
using Windows.Storage;
using Windows.System;

#if !HAS_UNO
#if !WINAPPSDK
using Microsoft.Toolkit.Uwp.UI.Controls;
#else
using CommunityToolkit.WinUI.UI.Controls;
#endif
#endif

#nullable enable

namespace CommunityToolkit.App.Shared.Renderers;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class ToolkitDocumentationRenderer : Page
{
    private const string MarkdownRegexSampleTagExpression = @"^>\s*\[!SAMPLE\s*(?<sampleid>.*)\s*\]\s*$";
    private static readonly Regex MarkdownRegexSampleTag = new Regex(MarkdownRegexSampleTagExpression, RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Multiline);
    private static string? ProjectUrl = null;

    public ToolkitDocumentationRenderer()
    {
        // Check and Cache here before we use in XAML.
        if (ProjectUrl == null)
        {
            ProjectUrl = Assembly.GetExecutingAssembly()?.GetCustomAttribute<CommunityToolkit.Attributes.PackageProjectUrlAttribute>()?.PackageProjectUrl;
        }

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

            var rest = doctext.Substring(index).Trim();
            // Put rest of text at end (if any)
            if (rest.Length > 0)
            {
                DocsAndSamples.Add(rest);
            }
        }
    }

    private void SampleSelectionBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is ComboBox comboBox && comboBox.SelectedItem is ToolkitSampleMetadata metadata)
        {
            var container = DocItemsControl.ContainerFromItem(metadata) as UIElement;
            container?.StartBringIntoView();
        }
    }
    private static async Task<string> GetDocumentationFileContents(ToolkitFrontMatter metadata)
    {
        // TODO: https://github.com/CommunityToolkit/Labs-Windows/issues/142
        // MSBuild uses wildcard to find the files, and the wildcards decide where they end up
        // Single experiments use relative paths, the allExperiment head uses absolute paths that grab from all experiments
        // The wildcard captures decide the paths. This discrepency is accounted for manually.
        // Logic here is the exact same that MSBuild uses to find and include the files we need.
        var assemblyName = typeof(ToolkitSampleRenderer).Assembly.GetName().Name;
        if (string.IsNullOrWhiteSpace(assemblyName))
            throw new InvalidOperationException();

        var isAllExperimentHead = assemblyName.StartsWith("CommunityToolkit.", StringComparison.OrdinalIgnoreCase);
        var isProjectTemplateHead = assemblyName.StartsWith("ProjectTemplate");
        var isSingleExperimentHead = !isAllExperimentHead && !isProjectTemplateHead;
        
        if (metadata.FilePath is null || string.IsNullOrWhiteSpace(metadata.FilePath))
            throw new InvalidOperationException("Missing or malformed path to markdown file. Unable to continue;");

        // Normalize the path separators
        var path = metadata.FilePath;

        var fileUri = new Uri($"ms-appx:///SourceAssets/{(isSingleExperimentHead ? Path.GetFileName(path.Replace('\\', '/')) : path)}");

        try
        {
            var file = await StorageFile.GetFileFromApplicationUriAsync(fileUri);
            var textContents = await FileIO.ReadTextAsync(file);

            // Remove YAML - need to use array overload as single string not supported on .NET Standard 2.0
            var blocks = textContents.Split(new[] { "---" }, StringSplitOptions.RemoveEmptyEntries);

            return blocks.LastOrDefault() ?? "Couldn't find content after YAML Front Matter removal.";
        }
        catch (Exception e)
        {
            return $"Exception Encountered Loading file '{fileUri}':\n{e.Message}\n{e.StackTrace}";
        }
    }

#if HAS_UNO
    private void MarkdownTextBlock_LinkClicked(object sender, LinkClickedEventArgs e)
    {
        // No-op - WASM handles via browser 'a' tag, Windows has handler below.
        // TODO: For other platforms
    }
#elif !HAS_UNO
    private async void MarkdownTextBlock_LinkClicked(object sender, LinkClickedEventArgs e)
    {
        if (!Uri.IsWellFormedUriString(e.Link, UriKind.Absolute))
        {
            await new ContentDialog
            {
                Title = "Windows Community Toolkit Labs Sample App",
                Content = $"Link {e.Link} was malformed.",
                CloseButtonText = "Close",
                XamlRoot = XamlRoot // TODO: For UWP this is only on 1903+
            }.ShowAsync();
        }
        else
        {
            await Launcher.LaunchUriAsync(new Uri(e.Link));
        }
    }
#endif

    public static Uri? ToGitHubUri(string path, int id) => IsProjectPathValid() ? new Uri($"{ProjectUrl}/{path}/{id}") : null;

    public static Visibility IsIdValid(int id) => id switch
    {
        <= 0 => Visibility.Collapsed,
        _ => Visibility.Visible,
    };

    public static bool IsProjectPathValid() => !string.IsNullOrWhiteSpace(ProjectUrl);
}
