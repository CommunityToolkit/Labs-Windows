// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if HAS_UNO
    #if WINAPPSDK
    using ToolkitMTB = Microsoft.UI.Xaml.FrameworkElement;
    #else
    using ToolkitMTB = Windows.UI.Xaml.FrameworkElement;
    #endif
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
    public string? Text;
    public bool? IsTextSelectionEnabled;
    public TextWrapping? TextWrapping;
#endif
}
