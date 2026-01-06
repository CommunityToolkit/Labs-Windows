// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.WinUI.Controls;
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
[ToolkitSample(id: nameof(MarkdownTextBlockLiveEditorSample), "Live Editor", description: $"An interactive live editor for the {nameof(CommunityToolkit.WinUI.Controls.MarkdownTextBlock)} control. Type markdown and see it rendered in real-time.")]
public sealed partial class MarkdownTextBlockLiveEditorSample : Page
{
    public MarkdownTextBlockLiveEditorSample()
    {
        this.InitializeComponent();
        MarkdownTextBox.Text = """
            # Hello World

            Try typing some **markdown** here!

            - Item 1
            - Item 2
            - [ ] Task item

            > This is a blockquote

            ```csharp
            Console.WriteLine("Hello, World!");
            ```
            """;
        MarkdownTextBlock.OnLinkClicked += MarkdownTextBlock_OnLinkClicked;
    }

    private void MarkdownTextBlock_OnLinkClicked(object? sender, LinkClickedEventArgs e)
    {
        Debug.WriteLine($"Link Clicked: {e.Uri}");
    }
}
