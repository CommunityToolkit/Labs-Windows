// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if HAS_UNO
    #if WINAPPSDK
    using ToolkitMTB = Microsoft.UI.Xaml.Controls.TextBlock;
    #else
    using ToolkitMTB = Windows.UI.Xaml.Controls.TextBlock;
    #endif
#else
    #if WINAPPSDK
    using ToolkitMTB = CommunityToolkit.WinUI.UI.Controls.MarkdownTextBlock;
    #else
    using ToolkitMTB = Microsoft.Toolkit.Uwp.UI.Controls.MarkdownTextBlock;
    #endif
#endif

#if __WASM__
using Markdig;
using Uno.Foundation.Interop;
#endif

namespace CommunityToolkit.Labs.Shared.Renderers;

/// <summary>
/// Provide an abstraction around the Toolkit MarkdownTextBlock for both UWP and WinUI 3 in the same namespace (until 8.0) as well as a polyfill for WebAssembly/WASM.
/// </summary>
#if __WASM__
public partial class MarkdownTextBlock : Control
{
    public string Text
    {
        get { return (string)GetValue(TextProperty); }
        set { SetValue(TextProperty, value); }
    }

    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register(nameof(Text), typeof(string), typeof(MarkdownTextBlock), new PropertyMetadata(null, MarkdownTextChanged));

    private static void MarkdownTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is MarkdownTextBlock mtb && mtb.IsLoaded)
        {
            mtb.UpdateText(e.NewValue as string ?? string.Empty);
        }
    }

    public MarkdownTextBlock()
    {
        Loaded += this.MarkdownTextBlock_Loaded;
    }

    private void MarkdownTextBlock_Loaded(object sender, RoutedEventArgs e)
    {
        this.RegisterHtmlEventHandler("resize", HtmlElementResized);

        UpdateText(Text);
    }

    #nullable enable
    private void HtmlElementResized(object? sender, EventArgs e)
    {
        this.UpdateLayout();
    }

    private void UpdateText(string markdown)
    {
        // TODO: Check color hasn't changed since last time.
        var color = (Foreground as SolidColorBrush)?.Color;
        if (color != null)
        {
            this.SetCssStyle(("color", $"#{color!.ToString()!.Substring(3)}"), ("font-family", "Segoe UI"));
        }
        else
        {
            this.SetCssStyle("fontFamily", "Segoe UI");
        }        

        this.SetHtmlContent(Markdown.ToHtml(markdown));
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        var size = this.MeasureHtmlView(availableSize, true);

        return size;
    }
}
#else
public partial class MarkdownTextBlock : ToolkitMTB
{
#if HAS_UNO
    //// Polyfill dummy for event callback
    #pragma warning disable CS0067 // Unused on purpose for polyfill
    public event EventHandler<LinkClickedEventArgs>? LinkClicked;
    #pragma warning restore CS0067 // Unused on purpose for polyfill
#endif
}

#if HAS_UNO
//// Polyfill dummy for event callback
public class LinkClickedEventArgs : EventArgs { }
#endif
