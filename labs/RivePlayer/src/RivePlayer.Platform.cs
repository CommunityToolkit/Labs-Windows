// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if WINDOWS_WINAPPSDK
using SkiaSharp.Views.Windows;
#else
// SkiaSharp.Views.UWP is on Uno too.
using SkiaSharp.Views.UWP;
#endif

namespace CommunityToolkit.Labs.WinUI.Rive;

// This file contains platform-specific customizations of RivePlayer.
public partial class RivePlayer
{
    // SKXamlCanvas doesn't support rendering in a background thread.
    public bool DrawInBackground { get; set; }

    protected override void OnApplyTemplate()
    {
        _skiaSurface = GetTemplateChild(SkiaSurfacePartName) as ContentPresenter;
        if (_skiaSurface != null && _skiaSurface.Content == null)
        {
#if WINDOWS_WINAPPSDK || HAS_UNO_WASM
            // WinAppSdk doesn't have SKSwapChainPanel yet.
            // SKSwapChainPanel doesn't work in WASM yet.
            var xamlCanvas = new SKXamlCanvas();
            xamlCanvas.PaintSurface += OnPaintSurface;
            _skiaSurface.Content = xamlCanvas;
#else
            // SKSwapChainPanel performs better than SKXamlCanvas.
            var swapChainPanel = new SKSwapChainPanel();
            swapChainPanel.PaintSurface += OnPaintSurface;
            _skiaSurface.Content = swapChainPanel;
#endif
            _invalidateTimer = new InvalidateTimer(this, fps: 120);
        }
        base.OnApplyTemplate();
    }

    internal void Invalidate()
    {
#if WINDOWS_WINAPPSDK || HAS_UNO_WASM
        var xamlCanvas = _skiaSurface?.Content as SKXamlCanvas;
        xamlCanvas?.Invalidate();
#else
        var swapChainPanel = _skiaSurface?.Content as SKSwapChainPanel;
        swapChainPanel?.Invalidate();
#endif
    }

#if WINDOWS_WINAPPSDK || HAS_UNO_WASM
    private void OnPaintSurface(object? sender, SKPaintSurfaceEventArgs e)
    {
        this.PaintNextAnimationFrame(e.Surface, e.Info.Width, e.Info.Height);
    }
#else
    private void OnPaintSurface(object? sender, SKPaintGLSurfaceEventArgs e)
    {
        this.PaintNextAnimationFrame(e.Surface, e.BackendRenderTarget.Width, e.BackendRenderTarget.Height);
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
