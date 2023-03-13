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

#if WASM
using Markdig;
using Uno.Foundation.Interop;
using Uno.UI.Runtime.WebAssembly;
#endif

namespace CommunityToolkit.App.Shared.Renderers;

/// <summary>
/// Provide an abstraction around the Toolkit MarkdownTextBlock for both UWP and WinUI 3 in the same namespace (until 8.0) as well as a polyfill for WebAssembly/WASM.
/// </summary>
#if WASM
[HtmlElement("div")]
public partial class MarkdownTextBlock : TextBlock
{
    public MarkdownTextBlock()
    {
        Loaded += this.MarkdownTextBlock_Loaded;
    }

    protected override void OnTextChanged(string oldValue, string newValue)
    {
        if (IsLoaded)
        {
            UpdateText(newValue);
        }
    }

    private void MarkdownTextBlock_Loaded(object sender, RoutedEventArgs e)
    {
        this.RegisterHtmlEventHandler("resize", HtmlElementResized);

        UpdateText(Text);
    }

    #nullable enable
    private void HtmlElementResized(object? sender, EventArgs e)
    {
        this.InvalidateMeasure();
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

        this.SetCssClass("fluent-hyperlink-style");

        this.SetHtmlContent(Markdown.ToHtml(markdown));

        this.InvalidateMeasure();
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        var size = this.MeasureHtmlView(availableSize, true);

        return size;
    }

    //// Polyfill dummy for event callback
    #pragma warning disable CS0067 // Unused on purpose for polyfill
    public event EventHandler<LinkClickedEventArgs>? LinkClicked;
    #pragma warning restore CS0067 // Unused on purpose for polyfill
}
#else
public partial class MarkdownTextBlock : ToolkitMTB
{
    #if !HAS_UNO
    public MarkdownTextBlock()
    {
        // Note: TODO: We can't use win:IsTextSelectionEnabled in XAML, for some reason getting a UWP compiler issue...? Maybe confused by inheritance?
        IsTextSelectionEnabled = true;
    }
    #else
    //// Polyfill dummy for event callback
    #pragma warning disable CS0067 // Unused on purpose for polyfill
    public event EventHandler<LinkClickedEventArgs>? LinkClicked;
    #pragma warning restore CS0067 // Unused on purpose for polyfill
    #endif
}
#endif

#if HAS_UNO
//// Polyfill dummy for event callback
public class LinkClickedEventArgs : EventArgs { }
#endif
