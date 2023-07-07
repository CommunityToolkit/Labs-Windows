// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if WINDOWS_UWP && !HAS_UNO
using Windows.ApplicationModel.Core;
using Windows.UI.ViewManagement;
using Windows.UI;

namespace CommunityToolkit.WinUI.Controls;

[TemplatePart(Name = nameof(PART_DragRegion), Type = typeof(Grid))]

public partial class TitleBar : Control
{
    Grid? PART_DragRegion;

    private void SetUWPTitleBar()
    {
        if (AutoConfigureCustomTitleBar)
        {
            CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;
            CoreApplication.GetCurrentView().TitleBar.LayoutMetricsChanged -= this.TitleBar_LayoutMetricsChanged;
            CoreApplication.GetCurrentView().TitleBar.LayoutMetricsChanged += this.TitleBar_LayoutMetricsChanged;
            Window.Current.Activated -= this.Current_Activated;
            Window.Current.Activated += this.Current_Activated;
       
            ApplicationView.GetForCurrentView().TitleBar.ButtonBackgroundColor = Colors.Transparent;
            ApplicationView.GetForCurrentView().TitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
         
            PART_DragRegion = GetTemplateChild(nameof(PART_DragRegion)) as Grid;
            Window.Current.SetTitleBar(PART_DragRegion);
        }
    }

    private void ResetUWPTitleBar()
    {
        CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = false;
        Window.Current.Activated -= this.Current_Activated;
        SizeChanged -= this.TitleBar_SizeChanged;
        CoreApplication.GetCurrentView().TitleBar.LayoutMetricsChanged -= this.TitleBar_LayoutMetricsChanged;
        Window.Current.SetTitleBar(null);
    }

    private void Current_Activated(object sender, Windows.UI.Core.WindowActivatedEventArgs e)
    {
        if (e.WindowActivationState == Windows.UI.Core.CoreWindowActivationState.Deactivated)
        {
            VisualStateManager.GoToState(this, WindowDeactivatedState, true);
        }
        else
        {
            VisualStateManager.GoToState(this, WindowActivatedState, true);
        }
    }

    private void TitleBar_LayoutMetricsChanged(CoreApplicationViewTitleBar sender, object args)
    {
        PART_LeftPaddingColumn!.Width = new GridLength(CoreApplication.GetCurrentView().TitleBar.SystemOverlayLeftInset);
        PART_RightPaddingColumn!.Width = new GridLength(CoreApplication.GetCurrentView().TitleBar.SystemOverlayRightInset);
    }
}
#endif
