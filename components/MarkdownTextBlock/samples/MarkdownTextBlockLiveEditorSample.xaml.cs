// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.Labs.WinUI.MarkdownTextBlock;
using System.Diagnostics;

namespace MarkdownTextBlockExperiment.Samples;

/// <summary>
/// A live editor sample for the MarkdownTextBlock control where users can type markdown and see it rendered in real-time.
/// </summary>
[ToolkitSampleBoolOption("UseEmphasisExtras", false, Title = "UseEmphasisExtras")]
[ToolkitSampleBoolOption("UsePipeTables", false, Title = "UsePipeTables")]
[ToolkitSampleBoolOption("UseListExtras", false, Title = "UseListExtras")]
[ToolkitSampleBoolOption("UseTaskLists", false, Title = "UseTaskLists")]
[ToolkitSampleBoolOption("UseAutoLinks", false, Title = "UseAutoLinks")]
[ToolkitSampleBoolOption("DisableHtml", false, Title = "DisableHtml")]
[ToolkitSampleBoolOption("UseSoftlineBreakAsHardlineBreak", false, Title = "UseSoftlineBreakAsHardlineBreak")]
[ToolkitSample(id: nameof(MarkdownTextBlockLiveEditorSample), "Live Editor", description: $"An interactive live editor for the {nameof(CommunityToolkit.Labs.WinUI.MarkdownTextBlock)} control. Type markdown and see it rendered in real-time.")]
public sealed partial class MarkdownTextBlockLiveEditorSample : Page
{
    private MarkdownConfig _config;

    public MarkdownConfig MarkdownConfig
    {
        get => _config;
        set => _config = value;
    }

    public MarkdownTextBlockLiveEditorSample()
    {
        this.InitializeComponent();
        _config = new MarkdownConfig();
        MarkdownTextBox.Text = "# Hello World\n\nTry typing some **markdown** here!\n\n- Item 1\n- Item 2\n- [ ] Task item\n\n> This is a blockquote\n\n```csharp\nConsole.WriteLine(\"Hello, World!\");\n```";
        MarkdownTextBlock.OnLinkClicked += MarkdownTextBlock_OnLinkClicked;
    }

    private void MarkdownTextBlock_OnLinkClicked(object? sender, LinkClickedEventArgs e)
    {
        Debug.WriteLine($"Link Clicked: {e.Uri}");
    }
}
