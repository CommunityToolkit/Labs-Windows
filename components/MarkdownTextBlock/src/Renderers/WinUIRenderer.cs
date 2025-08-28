// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.WinUI.Controls.Renderers.ObjectRenderers;
using CommunityToolkit.WinUI.Controls.Renderers.ObjectRenderers.Inlines;
using CommunityToolkit.WinUI.Controls.Renderers.ObjectRenderers.Extensions;
using CommunityToolkit.WinUI.Controls.TextElements;
using Markdig.Renderers;
using Markdig.Syntax;
using Markdig.Helpers;

namespace CommunityToolkit.WinUI.Controls.Renderers;

public class WinUIRenderer : RendererBase
{
    private readonly Stack<IAddChild> _stack = new Stack<IAddChild>();
    private char[] _buffer;
    private MarkdownConfig _config = MarkdownConfig.Default;
    private readonly Stack<string> _listBullets = new();

    public MyFlowDocument FlowDocument { get; private set; }
    public MarkdownConfig Config
    {
        get => _config;
        set => _config = value;
    }
    public MarkdownTextBlock MarkdownTextBlock { get; }

    public WinUIRenderer(MyFlowDocument document, MarkdownConfig config, MarkdownTextBlock markdownTextBlock)
    {
        _buffer = new char[1024];
        Config = config;
        MarkdownTextBlock = markdownTextBlock;
        FlowDocument = document;
        // set style
        _stack.Push(FlowDocument);
    }

    public override object Render(MarkdownObject markdownObject)
    {
        Write(markdownObject);
        return FlowDocument ?? new();
    }

    public void ReloadDocument()
    {
        _stack.Clear();
        FlowDocument.RichTextBlock.Blocks.Clear();
        _stack.Push(FlowDocument);
    }

    public void WriteLeafInline(LeafBlock leafBlock)
    {
        if (leafBlock == null || leafBlock.Inline == null) throw new ArgumentNullException(nameof(leafBlock));
        var inline = (Markdig.Syntax.Inlines.Inline)leafBlock.Inline;
        while (inline != null)
        {
            Write(inline);
            inline = inline.NextSibling;
        }
    }

    public void WriteLeafRawLines(LeafBlock leafBlock)
    {
        if (leafBlock == null) throw new ArgumentNullException(nameof(leafBlock));
        if (leafBlock.Lines.Lines != null)
        {
            var lines = leafBlock.Lines;
            var slices = lines.Lines;
            for (var i = 0; i < lines.Count; i++)
            {
                if (i != 0)
                    WriteInline(new MyLineBreak());

                WriteText(ref slices[i].Slice);
            }
        }
    }

    public void Push(IAddChild child)
    {
        _stack.Push(child);
    }

    public void Pop()
    {
        var popped = _stack.Pop();
        _stack.Peek().AddChild(popped);
    }

    public void WriteBlock(IAddChild obj)
    {
        _stack.Peek().AddChild(obj);
    }

    public void WriteInline(IAddChild inline)
    {
        AddInline(_stack.Peek(), inline);
    }

    public void WriteText(ref StringSlice slice)
    {
        if (slice.Start > slice.End)
            return;

        WriteText(slice.Text, slice.Start, slice.Length);
    }

    public void WriteText(string? text)
    {
        WriteInline(new MyInlineText(text ?? ""));
    }

    public void WriteText(string? text, int offset, int length)
    {
        if (text == null)
            return;

        if (offset == 0 && text.Length == length)
        {
            WriteText(text);
        }
        else
        {
            if (length > _buffer.Length)
            {
                _buffer = text.ToCharArray();
                WriteText(new string(_buffer, offset, length));
            }
            else
            {
                text.CopyTo(offset, _buffer, 0, length);
                WriteText(new string(_buffer, 0, length));
            }
        }
    }

    public void PushListBullet(string bullet)
    {
        _listBullets.Push(bullet);
    }

    public string PeekListBullet()
    {
        return _listBullets.Count > 0 ? _listBullets.Peek() : string.Empty;
    }

    public int GetListBulletCount()
    {
        return _listBullets.Count;
    }

    public void PopListBullet()
    {
        if (_listBullets.Count > 0)
        {
            _listBullets.Pop();
        }
    }

    private static void AddInline(IAddChild parent, IAddChild inline)
    {
        parent.AddChild(inline);
    }
}
