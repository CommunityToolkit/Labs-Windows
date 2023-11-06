// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if WINAPPSDK
using Microsoft.UI;
using Microsoft.UI.Windowing;
using static CommunityToolkit.WinUI.Controls.NativeMethods;

namespace CommunityToolkit.WinUI.Controls;

[TemplatePart(Name = nameof(PART_ButtonsHolderColumn), Type = typeof(ColumnDefinition))]
[TemplatePart(Name = nameof(PART_IconColumn), Type = typeof(ColumnDefinition))]
[TemplatePart(Name = nameof(PART_TitleColumn), Type = typeof(ColumnDefinition))]
[TemplatePart(Name = nameof(PART_LeftDragColumn), Type = typeof(ColumnDefinition))]
[TemplatePart(Name = nameof(PART_ContentColumn), Type = typeof(ColumnDefinition))]
[TemplatePart(Name = nameof(PART_FooterColumn), Type = typeof(ColumnDefinition))]
[TemplatePart(Name = nameof(PART_RightDragColumn), Type = typeof(ColumnDefinition))]
[TemplatePart(Name = nameof(PART_TitleHolder), Type = typeof(StackPanel))]
[TemplatePart(Name = nameof(PART_RootGrid), Type = typeof(Grid))]

public partial class TitleBar : Control
{
    ColumnDefinition? PART_ButtonsHolderColumn;
    ColumnDefinition? PART_IconColumn;
    ColumnDefinition? PART_TitleColumn;
    ColumnDefinition? PART_LeftDragColumn;
    ColumnDefinition? PART_ContentColumn;
    ColumnDefinition? PART_FooterColumn;
    ColumnDefinition? PART_RightDragColumn;
    StackPanel? PART_TitleHolder;
    Grid? PART_RootGrid;

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

            // Set the width of padding columns in the UI.
            PART_ButtonsHolderColumn = GetTemplateChild(nameof(PART_ButtonsHolderColumn)) as ColumnDefinition;
            PART_IconColumn = GetTemplateChild(nameof(PART_IconColumn)) as ColumnDefinition;
            PART_TitleColumn = GetTemplateChild(nameof(PART_TitleColumn)) as ColumnDefinition;
            PART_LeftDragColumn = GetTemplateChild(nameof(PART_LeftDragColumn)) as ColumnDefinition;
            PART_ContentColumn = GetTemplateChild(nameof(PART_ContentColumn)) as ColumnDefinition;
            PART_RightDragColumn = GetTemplateChild(nameof(PART_RightDragColumn)) as ColumnDefinition;
            PART_FooterColumn = GetTemplateChild(nameof(PART_FooterColumn)) as ColumnDefinition;
            PART_TitleHolder = GetTemplateChild(nameof(PART_TitleHolder)) as StackPanel;
            PART_RootGrid = GetTemplateChild(nameof(PART_RootGrid)) as Grid;

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

    private void SetDragRegionForCustomTitleBar()
    {
        if (AutoConfigureCustomTitleBar && Window != null && PART_RightPaddingColumn != null && PART_LeftPaddingColumn != null)
        {
            double scaleAdjustment = GetScaleAdjustment();

            PART_RightPaddingColumn.Width = new GridLength(Window.AppWindow.TitleBar.RightInset / scaleAdjustment);
            PART_LeftPaddingColumn.Width = new GridLength(Window.AppWindow.TitleBar.LeftInset / scaleAdjustment);

            var height = (int)(this.ActualHeight * scaleAdjustment);
            Windows.Graphics.RectInt32 rect1 = new(0, 0, 0, height);
            Windows.Graphics.RectInt32 rect2 = new(0, 0, 0, height);
            Windows.Graphics.RectInt32 rect3 = new(0, 0, 0, height);
            Windows.Graphics.RectInt32 rect4 = new(0, 0, 0, height);

            rect1.X = 0;
            rect1.Width = (int)((PART_RootGrid.Padding.Left
                                + PART_LeftPaddingColumn.ActualWidth)
                                * scaleAdjustment);

            rect2.X = rect1.X + rect1.Width + (int)((PART_ButtonsHolderColumn.ActualWidth) * scaleAdjustment);
            rect2.Width = (int)((PART_IconColumn.ActualWidth
                                + PART_TitleColumn.ActualWidth
                                + PART_LeftDragColumn.ActualWidth)
                                * scaleAdjustment);
            
            rect3.X = rect2.X + rect2.Width + (int)(PART_ContentColumn.ActualWidth * scaleAdjustment);
            rect3.Width = (int)(PART_RightDragColumn.ActualWidth * scaleAdjustment);

            rect4.X = rect3.X + rect3.Width + (int)((PART_FooterColumn.ActualWidth
                                                    + PART_RightPaddingColumn.ActualWidth
                                                    + PART_RootGrid.Padding.Right)
                                                    * scaleAdjustment);
            rect4.Width = (int)(PART_RightPaddingColumn.ActualWidth * scaleAdjustment);
 
            Windows.Graphics.RectInt32[] dragRects = new[] { rect1, rect2, rect3, rect4 };

            Window.AppWindow.TitleBar.SetDragRectangles(dragRects);
        }
    }

    private double GetScaleAdjustment()
    {
        DisplayArea displayArea = DisplayArea.GetFromWindowId(this.Window.AppWindow.Id, DisplayAreaFallback.Primary);
        IntPtr hMonitor = Win32Interop.GetMonitorFromDisplayId(displayArea.DisplayId);

        // Get DPI.
        int result = GetDpiForMonitor(hMonitor, Monitor_DPI_Type.MDT_Default, out uint dpiX, out uint _);
        if (result != 0)
        {
            throw new Exception("Could not get DPI for monitor.");
        }

        uint scaleFactorPercent = (uint)(((long)dpiX * 100 + (96 >> 1)) / 96);
        return scaleFactorPercent / 100.0;
    }
}
#endif
