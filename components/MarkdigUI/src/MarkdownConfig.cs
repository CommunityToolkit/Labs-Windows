namespace CommunityToolkit.Labs.WinUI.MarkdigUI;

public record MarkdownConfig
{
    public string? BaseUrl { get; set; }
    public string? Markdown { get; set; }
    public IImageProvider? ImageProvider { get; set; }
    public ISVGRenderer? SVGRenderer { get; set; }
}
