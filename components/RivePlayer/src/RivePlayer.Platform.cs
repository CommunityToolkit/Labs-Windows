// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if WINUI3 && HAS_UNO
using SkiaSharp;
#elif !WINAPPSDK
 // WASM on WinUI2 needs SkiaSharp.Views.UWP too.
using SkiaSharp.Views.UWP;
#else
using SkiaSharp.Views.Windows;
#endif

namespace CommunityToolkit.WinUI.Rive;

// This file contains platform-specific customizations of RivePlayer.
[TemplatePart(Name = SkiaSurfacePartName, Type = typeof(ContentPresenter))]
public partial class RivePlayer : Control
{
    private const string SkiaSurfacePartName = "SkiaSurface";
    ContentPresenter? _skiaSurface;
    private bool _drawInBackground;

    /// <summary>
    /// Controls whether the RivePlayer should run its rendering and animation logic in a background
    /// thread. This cannot be modified after the control has finished loading.
    /// </summary>
    public bool DrawInBackground
    {
        get => _drawInBackground;
        set
        {
            if (_skiaSurface != null)
            {
                throw new InvalidOperationException(
                    "RivePlayer.DrawInBackground cannot be modified after the control has finished loading.");
            }
            _drawInBackground = value;
        }
    }

    /// <inheritdoc/>
    protected override void OnApplyTemplate()
    {
        _skiaSurface = GetTemplateChild(SkiaSurfacePartName) as ContentPresenter;
        if (_skiaSurface != null && _skiaSurface.Content == null)
        {
#if WINDOWS_UWP
            // SKSwapChainPanel performs better than SKXamlCanvas.
            var swapChainPanel = new SKSwapChainPanel
            {
                DrawInBackground = _drawInBackground
            };
            swapChainPanel.PaintSurface += OnPaintSurface;
            _skiaSurface.Content = swapChainPanel;
#elif WINUI3 && HAS_UNO
            // SkiaSharp.Views has no Uno 7 build, so draw on Uno's own Skia canvas.
            _skiaSurface.Content = new RiveCanvasElement(this);
#else
            // WinAppSdk doesn't have SKSwapChainPanel yet.
            // SKSwapChainPanel doesn't work in WASM yet.
            var xamlCanvas = new SKXamlCanvas();
            xamlCanvas.PaintSurface += OnPaintSurface;
            _skiaSurface.Content = xamlCanvas;
#endif
        }
        base.OnApplyTemplate();
    }

    /// <summary>
    /// Schedules a repaint and a call to <see cref="PaintNextAnimationFrame"/>. Overlapping calls
    /// between frame boundaries are coalesced.
    /// </summary>
    internal void InvalidateAnimation()
    {
#if WINDOWS_UWP
        var swapChainPanel = _skiaSurface?.Content as SKSwapChainPanel;
        swapChainPanel?.Invalidate();
#elif WINUI3 && HAS_UNO
        (_skiaSurface?.Content as RiveCanvasElement)?.Invalidate();
#else
        var xamlCanvas = _skiaSurface?.Content as SKXamlCanvas;
        xamlCanvas?.Invalidate();
#endif
    }

#if WINDOWS_UWP
    private void OnPaintSurface(object? sender, SKPaintGLSurfaceEventArgs e)
    {
        e.Surface.Canvas.Clear();
        this.PaintNextAnimationFrame(e.Surface.Canvas, e.BackendRenderTarget.Width, e.BackendRenderTarget.Height);
    }
#elif WINUI3 && HAS_UNO
    // Called by RiveCanvasElement. The canvas is shared with the rest of the window, so it must not be cleared.
    internal void PaintNextAnimationFrame(SKCanvas canvas, global::Windows.Foundation.Size area) =>
        this.PaintNextAnimationFrame(canvas, (int)area.Width, (int)area.Height);
#else
    private void OnPaintSurface(object? sender, SKPaintSurfaceEventArgs e)
    {
        e.Surface.Canvas.Clear();
        this.PaintNextAnimationFrame(e.Surface.Canvas, e.Info.Width, e.Info.Height);
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
