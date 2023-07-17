// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if WINAPPSDK
using Microsoft.UI;
using Microsoft.UI.Windowing;

namespace CommunityToolkit.WinUI.Controls;

[TemplatePart(Name = nameof(PART_ButtonsHolderColumn), Type = typeof(ColumnDefinition))]
[TemplatePart(Name = nameof(PART_IconColumn), Type = typeof(ColumnDefinition))]
[TemplatePart(Name = nameof(PART_TitleColumn), Type = typeof(ColumnDefinition))]
[TemplatePart(Name = nameof(PART_LeftDragColumn), Type = typeof(ColumnDefinition))]
[TemplatePart(Name = nameof(PART_ContentColumn), Type = typeof(ColumnDefinition))]
[TemplatePart(Name = nameof(PART_FooterColumn), Type = typeof(ColumnDefinition))]
[TemplatePart(Name = nameof(PART_RightDragColumn), Type = typeof(ColumnDefinition))]
[TemplatePart(Name = nameof(PART_TitleHolder), Type = typeof(StackPanel))]

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

            List<Windows.Graphics.RectInt32> dragRectsList = new();

            Windows.Graphics.RectInt32 dragRectL;
            dragRectL.X = (int)((PART_LeftPaddingColumn.ActualWidth
                            + PART_ButtonsHolderColumn!.ActualWidth)
                            * scaleAdjustment);
            dragRectL.Y = 0;
            dragRectL.Height = (int)(this.ActualHeight * scaleAdjustment);
            dragRectL.Width = (int)((PART_IconColumn!.ActualWidth
                                + PART_TitleColumn!.ActualWidth
                                + PART_LeftDragColumn!.ActualWidth)
                                * scaleAdjustment);
            dragRectsList.Add(dragRectL);

            Windows.Graphics.RectInt32 dragRectR;
            dragRectR.X = (int)((PART_LeftPaddingColumn.ActualWidth
                                + PART_IconColumn.ActualWidth
                                + PART_ButtonsHolderColumn!.ActualWidth
                                + PART_TitleHolder!.ActualWidth
                                + PART_LeftDragColumn.ActualWidth
                                + PART_ContentColumn!.ActualWidth)
                                * scaleAdjustment);
            dragRectR.Y = 0;
            dragRectR.Height = (int)(this.ActualHeight * scaleAdjustment);
            dragRectR.Width = (int)(PART_RightDragColumn!.ActualWidth * scaleAdjustment);
            dragRectsList.Add(dragRectR);

            Windows.Graphics.RectInt32[] dragRects = dragRectsList.ToArray();

            Window.AppWindow.TitleBar.SetDragRectangles(dragRects);
        }
    }
}
#endif
