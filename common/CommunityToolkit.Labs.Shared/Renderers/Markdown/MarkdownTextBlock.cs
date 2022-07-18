// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if HAS_UNO
    using ToolkitMTB = CommunityToolkit.Labs.Shared.Renderers.JavaScriptBackedControl;
#else
    #if WINAPPSDK
    using ToolkitMTB = CommunityToolkit.WinUI.UI.Controls.MarkdownTextBlock;
    #else
    using ToolkitMTB = Microsoft.Toolkit.Uwp.UI.Controls.MarkdownTextBlock;
    #endif
#endif

namespace CommunityToolkit.Labs.Shared.Renderers;

/// <summary>
/// Provide an abstraction around the Toolkit MarkdownTextBlock for both UWP and WinUI 3 in the same namespace (until 8.0) as well as a polyfill for WebAssembly/WASM.
/// </summary>
public partial class MarkdownTextBlock : ToolkitMTB
{
#if HAS_UNO
    private const string MarkedEmbeddedJavaScriptFile = @"CommunityToolkit.Labs.Wasm.Renderers.Markdown.marked.min.js";

    public event EventHandler? MarkedReady;

    public bool IsMarkedReady { get; private set; }

    public string Text
    {
        get { return (string)GetValue(TextProperty); }
        set { SetValue(TextProperty, value); }
    }

    // Using a DependencyProperty as the backing store for MarkdownText.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register(nameof(Text), typeof(string), typeof(MarkdownTextBlock), new PropertyMetadata(null, MarkdownTextChanged));

    private static async void MarkdownTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is MarkdownTextBlock mtb)
        {
            await mtb.DisplayMarkdownText();
        }        
    }

    private async Task DisplayMarkdownText()
    {
        if (IsMarkedReady && !string.IsNullOrWhiteSpace(Text))
        {
            await DisplayMarkdown(Text);
        }
    }

    protected override async Task LoadJavaScript()
    {
        await LoadEmbeddedJavaScriptFile(MarkedEmbeddedJavaScriptFile);

        IsMarkedReady = true;

        await DisplayMarkdownText();

        MarkedReady?.Invoke(this, EventArgs.Empty);
    }

    public async Task DisplayMarkdown(string markdown)
    {
        markdown = markdown.Replace("\n", "\\n").Replace("\r", "\\r").Replace("\"", "\\\"").Replace("\'", "\\\'");//.Replace("\t","\\t").Replace("`","");
        System.Diagnostics.Debug.WriteLine(markdown);
        await UpdateHtmlFromScript($"marked.parse('{markdown}')");
    }
#endif
}
