// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.WinUI.Controls;
using Markdig.Syntax.Inlines;

namespace CommunityToolkit.WinUI.Controls.TextElements;

internal class MyInlineCode : IAddChild
{
    private CodeInline _codeInline;
    private InlineUIContainer _inlineContainer;
    private MarkdownTextBlock _control;

    public TextElement TextElement
    {
        get => _inlineContainer;
    }

    public MyInlineCode(CodeInline codeInline, MarkdownTextBlock control)
    {
        _codeInline = codeInline;
        _control = control;
        _inlineContainer = new InlineUIContainer();
        var border = new Border();
        border.VerticalAlignment = VerticalAlignment.Bottom;
        border.Background = _control.Config.InlineCodeBackground;
        border.BorderBrush = _control.Config.InlineCodeBorderBrush;
        border.BorderThickness = _control.Config.InlineCodeBorderThickness;
        border.CornerRadius = _control.Config.InlineCodeCornerRadius;
        border.Padding = _control.Config.InlineCodePadding;
        CompositeTransform3D transform = new CompositeTransform3D();
        transform.TranslateY = 4;
        border.Transform3D = transform;
        var textBlock = new TextBlock();
        textBlock.FontSize = _control.Config.InlineCodeFontSize;
        textBlock.Foreground = _control.Config.InlineCodeForeground;
        textBlock.FontWeight = _control.Config.InlineCodeFontWeight;
        textBlock.Text = codeInline.Content.ToString();
        border.Child = textBlock;
        _inlineContainer.Child = border;
    }


    public void AddChild(IAddChild child) {}
}
