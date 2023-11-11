// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if WINAPPSDK
using Microsoft.UI;
using Microsoft.UI.Input;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml.Media;

namespace CommunityToolkit.WinUI.Controls;

[TemplatePart(Name = nameof(PART_FooterPresenter), Type = typeof(ContentPresenter))]
[TemplatePart(Name = nameof(PART_ContentPresenter), Type = typeof(ContentPresenter))]

public partial class TitleBar : Control
{
    ContentPresenter? PART_ContentPresenter;
    ContentPresenter? PART_FooterPresenter;

    private void SetWASDKTitleBar()
    {
        if (this.Window == null)
        {
            return;
            // TO DO: Throw exception that window has not been set? 
        }
        if (AutoConfigureCustomTitleBar)
        {
            Window.AppWindow.TitleBar.ExtendsContentIntoTitleBar = true;

            this.Window.SizeChanged -= Window_SizeChanged;
            this.Window.SizeChanged += Window_SizeChanged;
            this.Window.Activated -= Window_Activated;
            this.Window.Activated += Window_Activated;

            if (Window.Content is FrameworkElement rootElement)
            {
                UpdateCaptionButtons(rootElement);
                rootElement.ActualThemeChanged += (s, e) =>
                {
                    UpdateCaptionButtons(rootElement);
                };
            }

            PART_ContentPresenter = GetTemplateChild(nameof(PART_ContentPresenter)) as ContentPresenter;
            PART_FooterPresenter = GetTemplateChild(nameof(PART_FooterPresenter)) as ContentPresenter;

            // Get caption button occlusion information.
            int CaptionButtonOcclusionWidthRight = Window.AppWindow.TitleBar.RightInset;
            int CaptionButtonOcclusionWidthLeft = Window.AppWindow.TitleBar.LeftInset;
            PART_LeftPaddingColumn!.Width = new GridLength(CaptionButtonOcclusionWidthLeft);
            PART_RightPaddingColumn!.Width = new GridLength(CaptionButtonOcclusionWidthRight);

            if (DisplayMode == DisplayMode.Tall)
            {
                // Choose a tall title bar to provide more room for interactive elements 
                // like search box or person picture controls.
                Window.AppWindow.TitleBar.PreferredHeightOption = TitleBarHeightOption.Tall;
            }
            else
            {
                Window.AppWindow.TitleBar.PreferredHeightOption = TitleBarHeightOption.Standard;
            }
            // Recalculate the drag region for the custom title bar 
            // if you explicitly defined new draggable areas.
            SetDragRegionForCustomTitleBar();
        }
    }

    private void Window_SizeChanged(object sender, WindowSizeChangedEventArgs args)
    {
        UpdateVisualStateAndDragRegion(args.Size);
    }

    private void UpdateCaptionButtons(FrameworkElement rootElement)
    {
        Window.AppWindow.TitleBar.ButtonBackgroundColor = Colors.Transparent;
        Window.AppWindow.TitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
        if (rootElement.ActualTheme == ElementTheme.Dark)
        {
            Window.AppWindow.TitleBar.ButtonForegroundColor = Colors.White;
            Window.AppWindow.TitleBar.ButtonInactiveForegroundColor = Colors.DarkGray;
        }
        else
        {
            Window.AppWindow.TitleBar.ButtonForegroundColor = Colors.Black;
            Window.AppWindow.TitleBar.ButtonInactiveForegroundColor = Colors.DarkGray;
        }
    }

    private void ResetWASDKTitleBar()
    {
        if (this.Window == null)
        {
            return;
            // TO DO: Throw exception that window has not been set? 
        }

        Window.AppWindow.TitleBar.ExtendsContentIntoTitleBar = false;
        this.Window.SizeChanged -= Window_SizeChanged;
        this.Window.Activated -= Window_Activated;
        SizeChanged -= this.TitleBar_SizeChanged;
        Window.AppWindow.TitleBar.ResetToDefault();
    }

    private void Window_Activated(object sender, WindowActivatedEventArgs args)
    {
        if (args.WindowActivationState == WindowActivationState.Deactivated)
        {
            VisualStateManager.GoToState(this, WindowDeactivatedState, true);
        }
        else
        {
            VisualStateManager.GoToState(this, WindowActivatedState, true);
        }
    }

    public void SetDragRegionForCustomTitleBar()
    {
        if (AutoConfigureCustomTitleBar && Window != null)
        {
            ClearDragRegions(NonClientRegionKind.Passthrough);
            SetDragRegion(NonClientRegionKind.Passthrough, PART_ContentPresenter, PART_FooterPresenter, PART_ButtonHolder);
        }
    }

    public double GetRasterizationScaleForElement(UIElement element)
    {
        if (element.XamlRoot != null)
        {
            return element.XamlRoot.RasterizationScale;
        }
        return 0.0;
    }

    public void SetDragRegion(NonClientRegionKind nonClientRegionKind, params FrameworkElement[] frameworkElements)
    {
        var nonClientInputSrc = InputNonClientPointerSource.GetForWindowId(Window.AppWindow.Id);
        List<Windows.Graphics.RectInt32> rects = new List<Windows.Graphics.RectInt32>();

        foreach (var frameworkElement in frameworkElements)
        {
            GeneralTransform transformElement = frameworkElement.TransformToVisual(null);
            Windows.Foundation.Rect bounds = transformElement.TransformBounds(new Windows.Foundation.Rect(0, 0, frameworkElement.ActualWidth, frameworkElement.ActualHeight));
            var scale = GetRasterizationScaleForElement(this);
            var transparentRect = new Windows.Graphics.RectInt32(
                _X: (int)Math.Round(bounds.X * scale),
                _Y: (int)Math.Round(bounds.Y * scale),
                _Width: (int)Math.Round(bounds.Width * scale),
                _Height: (int)Math.Round(bounds.Height * scale)
            );
            rects.Add(transparentRect);
        }
        
        nonClientInputSrc.SetRegionRects(nonClientRegionKind, rects.ToArray());
    }

    public void ClearDragRegions(NonClientRegionKind nonClientRegionKind)
    {
        var noninputsrc = InputNonClientPointerSource.GetForWindowId(Window.AppWindow.Id);
        noninputsrc.ClearRegionRects(nonClientRegionKind);
    }
}
#endif
