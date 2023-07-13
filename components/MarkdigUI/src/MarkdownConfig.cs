// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.Labs.WinUI.MarkdigUI;

public record MarkdownConfig
{
    public string? BaseUrl { get; set; }
    public string? Markdown { get; set; }
    public IImageProvider? ImageProvider { get; set; }
    public ISVGRenderer? SVGRenderer { get; set; }
}
