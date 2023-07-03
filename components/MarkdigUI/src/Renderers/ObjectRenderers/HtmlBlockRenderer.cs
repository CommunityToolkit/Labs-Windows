using HtmlAgilityPack;
using Markdig.Syntax;

namespace CommunityToolkit.Labs.WinUI.MarkdigUI.Renderers.ObjectRenderers;

internal class HtmlBlockRenderer : UWPObjectRenderer<HtmlBlock>
{
    protected override void Write(UWPRenderer renderer, HtmlBlock obj)
    {
        if (renderer == null) throw new ArgumentNullException(nameof(renderer));
        if (obj == null) throw new ArgumentNullException(nameof(obj));

        var stringBuilder = new StringBuilder();
        foreach (var line in obj.Lines.Lines)
        {
            var lineText = line.Slice.ToString().Trim();
            if (String.IsNullOrWhiteSpace(lineText))
            {
                continue;
            }
            stringBuilder.AppendLine(lineText);
        }

        var html = Regex.Replace(stringBuilder.ToString(), @"\t|\n|\r", "");
        html = Regex.Replace(html, @"&nbsp;", " ");
        var doc = new HtmlDocument();
        doc.LoadHtml(html);
        HtmlWriter.WriteHtml(renderer, doc.DocumentNode.ChildNodes);
    }
}
