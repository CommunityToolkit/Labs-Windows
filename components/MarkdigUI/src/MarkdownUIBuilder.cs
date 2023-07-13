// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.Labs.WinUI.MarkdigUI.Renderers;
using CommunityToolkit.Labs.WinUI.MarkdigUI.TextElements;
using Markdig;

namespace CommunityToolkit.Labs.WinUI.MarkdigUI;

public static class MarkdownUIBuilder
{
    public static UIElement Build(MarkdownConfig config)
    {
        if (config == null) throw new ArgumentNullException(nameof(config));
        var pipeline = new MarkdownPipelineBuilder()
            .UseEmphasisExtras()
            .UseAutoLinks()
            .UseTaskLists()
            .UsePipeTables()
            .Build();

        var result = new MyFlowDocument();
        var renderer = new UWPRenderer(result, config);

        pipeline.Setup(renderer);

        var document = Markdown.Parse(config.Markdown ?? "", pipeline);
        renderer.Render(document);

        return result.RichTextBlock;
    }
}
