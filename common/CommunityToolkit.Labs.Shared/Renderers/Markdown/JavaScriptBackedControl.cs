// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.Labs.Shared.Renderers;

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Windows.UI;
#if HAS_UNO
using Uno.Extensions;
using Uno.Foundation;
#endif
#if __WASM__
using Uno.Foundation.Interop;
#endif

#if HAS_UNO
public abstract partial class JavaScriptBackedControl :
#if !__WASM__
    UserControl
#else
    Control, IJSObject
#endif
{

#if !__WASM__
    private readonly WebView internalWebView;
#else
    private readonly JSObjectHandle _handle;
    JSObjectHandle IJSObject.Handle => _handle;
#endif

    public JavaScriptBackedControl()
    {
#if !__WASM__
        Content = internalWebView = new WebView();
        internalWebView.DefaultBackgroundColor = Colors.Transparent;
        internalWebView.NavigationCompleted += NavigationCompleted;
#else
        _handle = JSObjectHandle.Create(this);
#endif
        Loaded += JavaScriptBackedControl_Loaded;
    }

    private string HtmlContentId =>
#if __WASM__
        this.GetHtmlId();
#else
        "content";
#endif

    private void JavaScriptBackedControl_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
    {
#if !__WASM__
        var html = @"<html>
    <body>
    <div id = ""content""></ div>
</ body>
</ html>
";
        internalWebView.NavigateToString(html);
#else
        LoadJavaScript();
#endif
    }

    protected async Task LoadEmbeddedJavaScriptFile(string filename)
    {
        string markdownScript;
        using (StreamReader sr = new StreamReader(await GetEmbeddedFileStreamAsync(GetType(), filename)))
        {
            markdownScript = await sr.ReadToEndAsync();
        }

        await InvokeScriptAsync(markdownScript, false);
    }

    protected abstract Task LoadJavaScript();

#if !__WASM__
    private void NavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
    {
        LoadJavaScript();
    }
#endif

    protected async Task UpdateHtmlFromScript(string contentScript)
    {
        var color = (Foreground as SolidColorBrush)?.Color;
        if (color != null)
        {
            var colorScript = $@"document.getElementById('{HtmlContentId}').style.color = '#{color!.ToString()!.Substring(3)}';";
            await InvokeScriptAsync(colorScript);
        }

        var script = $@"document.getElementById('{HtmlContentId}').innerHTML = {contentScript};";
        await InvokeScriptAsync(script);
    }

    public async Task<string> InvokeScriptAsync(string scriptToRun, bool resizeAfterScript = true)
    {
        scriptToRun = ReplaceLiterals(scriptToRun);

#if !__WASM__
        var result = await internalWebView.InvokeScriptAsync(
            "eval", new[] { scriptToRun }).AsTask();
        if (resizeAfterScript)
        {
            await ResizeToContent();
        }
        return result;
#else
        var script = $"javascript:eval(\"{scriptToRun}\");";
        Console.Error.WriteLine(script);

        try
        {
            var result = WebAssemblyRuntime.InvokeJS(script);
            Console.WriteLine($"Result: {result}");

            if (resizeAfterScript)
            {
                await ResizeToContent();
            }

            return result;
        }
        catch (Exception e)
        {
            Console.Error.WriteLine("FAILED " + e);
            return string.Empty;
        }

#endif
    }

    private static Func<string, string> ReplaceLiterals = txt =>
#if !__WASM__
    txt;
#else
    txt.Replace("\\", "\\\\").Replace("\n", "\\n").Replace("\r", "\\r").Replace("\"", "\\\"").Replace("\'", "\\\'").Replace("`", "\\`").Replace("^", "\\^");
#endif


    public static async Task<Stream> GetEmbeddedFileStreamAsync(Type assemblyType, string fileName)
    {
        await Task.Yield();

        var manifest = assemblyType.GetTypeInfo().Assembly
            .GetManifestResourceNames();
        
        var manifestName = manifest
            .FirstOrDefault(n => n.EndsWith(fileName.Replace(" ", "_").Replace("/", ".").Replace("\\", "."), StringComparison.OrdinalIgnoreCase));

        if (manifestName == null)
        {
            throw new InvalidOperationException($"Failed to find resource [{fileName}]");
        }

        return assemblyType.GetTypeInfo().Assembly.GetManifestResourceStream(manifestName)!;
    }
    
    public async Task ResizeToContent()
    {
        var documentRoot =
#if __WASM__
            $"document.getElementById('{HtmlContentId}')";
#else
                        $"document.body";
#endif


        var heightString = await InvokeScriptAsync($"{documentRoot}.scrollHeight.toString()",
            false);
        int height;
        if (int.TryParse(heightString, out height))
        {
            this.Height = height;
        }
    }
}

//public static class WebViewExtensions
//{
//    public static async Task ResizeToContent(this WebView webView)
//    {
//        var heightString = await webView.InvokeScriptAsync("eval", new[] { "document.body.scrollHeight.toString()" });
//        int height;
//        if (int.TryParse(heightString, out height))
//        {
//            webView.Height = height;
//        }
//    }
//}

#endif
