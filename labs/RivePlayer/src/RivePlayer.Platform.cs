// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.Labs.WinUI.Rive;

// This file contains platform-specific customizations of RivePlayer.

#if WINDOWS_WINAPPSDK || HAS_UNO_WASM

#if WINDOWS_WINAPPSDK
using SkiaSharp.Views.Windows;
#else
using SkiaSharp.Views.UWP;
#endif

// WinAppSdk doesn't have SKSwapChainPanel yet.
// SKSwapChainPanel doesn't work in WASM yet.
public partial class RivePlayer
{
    // SKXamlCanvas doesn't support rendering in a background thread.
    public bool DrawInBackground { get; set; }

    protected override void OnApplyTemplate()
    {
        _skiaSurface = GetTemplateChild(SkiaSurfacePartName) as ContentPresenter;
        if (_skiaSurface != null && _skiaSurface.Content == null)
        {
            var xamlCanvas = new SKXamlCanvas();
            xamlCanvas.PaintSurface += OnPaintSurface;
            _skiaSurface.Content = xamlCanvas;
            _invalidateTimer = new InvalidateTimer(this, fps: 120);
        }
        base.OnApplyTemplate();
    }

    internal void Invalidate()
    {
        var xamlCanvas = _skiaSurface?.Content as SKXamlCanvas;
        xamlCanvas?.Invalidate();
    }

    private void OnPaintSurface(object? sender, SKPaintSurfaceEventArgs e)
    {
        this.PaintNextAnimationFrame(e.Surface, e.Info.Width, e.Info.Height);
    }
#else

// SkiaSharp.Views.UWP is on Uno too.
using SkiaSharp.Views.UWP;

// SKSwapChainPanel performs better than SKXamlCanvas.
public partial class RivePlayer
{
    public bool DrawInBackground { get; set; }

    protected override void OnApplyTemplate()
    {
        _skiaSurface = GetTemplateChild(SkiaSurfacePartName) as ContentPresenter;
        if (_skiaSurface != null && _skiaSurface.Content == null)
        {
            var swapChainPanel = new SKSwapChainPanel();
            swapChainPanel.PaintSurface += OnPaintSurface;
            _skiaSurface.Content = swapChainPanel;
            _invalidateTimer = new InvalidateTimer(this, fps: 120);
        }
        base.OnApplyTemplate();
    }

    internal void Invalidate()
    {
    }

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
