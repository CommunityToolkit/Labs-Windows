// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Markdig.Syntax.Inlines;
using CommunityToolkit.WinUI.Controls.TextElements;

namespace CommunityToolkit.WinUI.Controls.Renderers.ObjectRenderers.Inlines;

internal class LinkInlineRenderer : UWPObjectRenderer<LinkInline>
{
    protected override void Write(WinUIRenderer renderer, LinkInline link)
    {
        if (renderer == null) throw new ArgumentNullException(nameof(renderer));
        if (link == null) throw new ArgumentNullException(nameof(link));

        var url = link.GetDynamicUrl != null ? link.GetDynamicUrl() ?? link.Url : link.Url;

        if (!Uri.IsWellFormedUriString(url, UriKind.RelativeOrAbsolute))
        {
            url = "#";
        }

        if (link.IsImage)
        {
            var image = new MyImage(link, Controls.Extensions.GetUri(url, renderer.Config.BaseUrl), renderer.Config);
            renderer.WriteInline(image);
        }
        else
        {
            if (link.FirstChild is LinkInline linkInlineChild && linkInlineChild.IsImage)
            {
                var myHyperlinkButton = new MyHyperlinkButton(link, renderer.Config.BaseUrl);
                myHyperlinkButton.ClickEvent += (sender, e) =>
                {
                    var button = (HyperlinkButton)sender;
                    var uri = button.NavigateUri;
                    var handled = renderer.MarkdownTextBlock.RaiseLinkClickedEvent(uri);
                    if (handled)
                    {
                        // Suppress default navigation by clearing NavigateUri just for this invocation
                        button.NavigateUri = null;
                        // Optionally restore later; not needed unless reused.
                    }
                };
                // Apply link foreground to nested RichTextBlock content
                // (Handled in MyHyperlinkButton initialization via MarkdownConfig.Default for now)
                renderer.Push(myHyperlinkButton);
            }
            else
            {
                var hyperlink = new MyHyperlink(link, renderer.Config.BaseUrl);
                hyperlink.TextElement.Foreground = renderer.Config.Themes.LinkForeground;
                hyperlink.ClickEvent += (sender, e) =>
                {
                    var uri = sender.NavigateUri;
                    var handled = renderer.MarkdownTextBlock.RaiseLinkClickedEvent(uri);
                    if (handled)
                    {
                        // Suppress navigation by clearing NavigateUri
                        sender.NavigateUri = null;
                    }
                };

                renderer.Push(hyperlink);
            }

            renderer.WriteChildren(link);
            renderer.Pop();
        }
    }
}
