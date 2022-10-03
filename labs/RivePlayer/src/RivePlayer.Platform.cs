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
#if WINDOWS_WINAPPSDK || HAS_UNO_WASM
            // WinAppSdk doesn't have SKSwapChainPanel yet.
            // SKSwapChainPanel doesn't work in WASM yet.
            var xamlCanvas = new SKXamlCanvas();
            xamlCanvas.PaintSurface += OnPaintSurface;
            _skiaSurface.Content = xamlCanvas;
#else
            // SKSwapChainPanel performs better than SKXamlCanvas.
            var swapChainPanel = new SKSwapChainPanel
            {
                DrawInBackground = _drawInBackground
            };
            swapChainPanel.PaintSurface += OnPaintSurface;
            _skiaSurface.Content = swapChainPanel;
#endif
            _animationTimer = new AnimationTimer(this, fps: 120);
        }
        base.OnApplyTemplate();
    }

    /// <summary>
    /// Schedules a repaint and a call to <see cref="PaintNextAnimationFrame"/>. Overlapping calls
    /// between frame boundaries are coalesced.
    /// </summary>
    internal void InvalidateAnimation()
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