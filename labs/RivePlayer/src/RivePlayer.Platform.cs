// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.Labs.WinUI.Rive;

#if WINDOWS_WINAPPSDK
using SkiaSharp.Views.Windows;
#else
using SkiaSharp.Views.UWP;
#endif

// This file contains platform-specific customizations of RivePlayer.
#if WINDOWS_UWP
// SKSwapChainPanel performs better on UWP.
public partial class RivePlayer : SKSwapChainPanel
{
    private void OnPaintSurface(object? sender, SKPaintGLSurfaceEventArgs e)
    {
        this.PaintNextAnimationFrame(e.Surface, e.BackendRenderTarget.Width, e.BackendRenderTarget.Height);
    }
#else
public partial class RivePlayer : SKXamlCanvas
{
    // SKXamlCanvas doesn't support rendering in a background thread.
    public bool DrawInBackground { get; set; }

    private void OnPaintSurface(object? sender, SKPaintSurfaceEventArgs e)
    {
        this.PaintNextAnimationFrame(e.Surface, e.Info.Width, e.Info.Height);
    }
#endif

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
#if HAS_UNO
        // XamlRoot.IsHostVisible isn't implemented in Uno.
        OnXamlRootChanged(isHostVisible:true);
#else
        this.XamlRoot.Changed += (XamlRoot xamlRoot, XamlRootChangedEventArgs a) =>
        {
            OnXamlRootChanged(xamlRoot.IsHostVisible);
        };
        OnXamlRootChanged(this.XamlRoot.IsHostVisible);
#endif
    }
}

