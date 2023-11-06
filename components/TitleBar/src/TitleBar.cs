// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.WinUI.Controls;

[TemplateVisualState(Name = BackButtonVisibleState, GroupName = BackButtonStates)]
[TemplateVisualState(Name = BackButtonCollapsedState, GroupName = BackButtonStates)]
[TemplateVisualState(Name = PaneButtonVisibleState, GroupName = PaneButtonStates)]
[TemplateVisualState(Name = PaneButtonCollapsedState, GroupName = PaneButtonStates)]
[TemplateVisualState(Name = WindowActivatedState, GroupName = ActivationStates)]
[TemplateVisualState(Name = WindowDeactivatedState, GroupName = ActivationStates)]
[TemplateVisualState(Name = StandardState, GroupName = DisplayModeStates)]
[TemplateVisualState(Name = TallState, GroupName = DisplayModeStates)]
[TemplateVisualState(Name = IconVisibleState, GroupName = IconStates)]
[TemplateVisualState(Name = IconCollapsedState, GroupName = IconStates)]
[TemplateVisualState(Name = ContentVisibleState, GroupName = ContentStates)]
[TemplateVisualState(Name = ContentCollapsedState, GroupName = ContentStates)]
[TemplateVisualState(Name = FooterVisibleState, GroupName = FooterStates)]
[TemplateVisualState(Name = FooterCollapsedState, GroupName = FooterStates)]
[TemplateVisualState(Name = WideState, GroupName = ReflowStates)]
[TemplateVisualState(Name = NarrowState, GroupName = ReflowStates)]
[TemplatePart(Name = PartBackButton, Type = typeof(Button))]
[TemplatePart(Name = PartPaneButton, Type = typeof(Button))]
[TemplatePart(Name = nameof(PART_LeftPaddingColumn), Type = typeof(ColumnDefinition))]
[TemplatePart(Name = nameof(PART_RightPaddingColumn), Type = typeof(ColumnDefinition))]
[TemplatePart(Name = nameof(PART_ButtonHolder), Type = typeof(StackPanel))]

public partial class TitleBar : Control
{
    private const string PartBackButton = "PART_BackButton";
    private const string PartPaneButton = "PART_PaneButton";

    private const string BackButtonVisibleState = "BackButtonVisible";
    private const string BackButtonCollapsedState = "BackButtonCollapsed";
    private const string BackButtonStates = "BackButtonStates";

    private const string PaneButtonVisibleState = "PaneButtonVisible";
    private const string PaneButtonCollapsedState = "PaneButtonCollapsed";
    private const string PaneButtonStates = "PaneButtonStates";

    private const string WindowActivatedState = "Activated";
    private const string WindowDeactivatedState = "Deactivated";
    private const string ActivationStates = "WindowActivationStates";

    private const string IconVisibleState = "IconVisible";
    private const string IconCollapsedState = "IconCollapsed";
    private const string IconStates = "IconStates";

    private const string StandardState = "Standard";
    private const string TallState = "Tall";
    private const string DisplayModeStates = "DisplayModeStates";

    private const string ContentVisibleState = "ContentVisible";
    private const string ContentCollapsedState = "ContentCollapsed";
    private const string ContentStates = "ContentStates";

    private const string FooterVisibleState = "FooterVisible";
    private const string FooterCollapsedState = "FooterCollapsed";
    private const string FooterStates = "FooterStates";

    private const string WideState = "Wide";
    private const string NarrowState = "Narrow";
    private const string ReflowStates = "ReflowStates";

    ColumnDefinition? PART_LeftPaddingColumn;
    ColumnDefinition? PART_RightPaddingColumn;
    StackPanel? PART_ButtonHolder;

    public TitleBar()
    {
        this.DefaultStyleKey = typeof(TitleBar);
    }

    protected override void OnApplyTemplate()
    {
        PART_LeftPaddingColumn = GetTemplateChild(nameof(PART_LeftPaddingColumn)) as ColumnDefinition;
        PART_RightPaddingColumn = GetTemplateChild(nameof(PART_RightPaddingColumn)) as ColumnDefinition;
        Configure();
        ConfigureButtonHolder();
        if (GetTemplateChild(PartBackButton) is Button backButton)
        {
            backButton.Click -= BackButton_Click;
            backButton.Click += BackButton_Click;
        }

        if (GetTemplateChild(PartPaneButton) is Button paneButton)
        {
            paneButton.Click -= PaneButton_Click;
            paneButton.Click += PaneButton_Click;
        }


        SizeChanged -= this.TitleBar_SizeChanged;
        SizeChanged += this.TitleBar_SizeChanged;

        Update();
        base.OnApplyTemplate();
    }

    private void TitleBar_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        UpdateVisualStateAndDragRegion(e.NewSize);
    }

    private void UpdateVisualStateAndDragRegion(Windows.Foundation.Size size)
    {
        if (size.Width <= CompactStateBreakpoint)
        {
            if (Content != null || Footer != null)
            {
                VisualStateManager.GoToState(this, NarrowState, true);
            }
        }
        else
        {
            VisualStateManager.GoToState(this, WideState, true);
        }

#if WINAPPSDK
        SetDragRegionForCustomTitleBar();
#endif
    }

    private void BackButton_Click(object sender, RoutedEventArgs e)
    {
        BackButtonClick?.Invoke(this, new RoutedEventArgs());
    }

    private void PaneButton_Click(object sender, RoutedEventArgs e)
    {
        PaneButtonClick?.Invoke(this, new RoutedEventArgs());
    }

    private void ConfigureButtonHolder()
    {
        if (PART_ButtonHolder != null)
        {
            PART_ButtonHolder.SizeChanged -= PART_ButtonHolder_SizeChanged;
        }

        PART_ButtonHolder = GetTemplateChild(nameof(PART_ButtonHolder)) as StackPanel;

        if(PART_ButtonHolder != null)
        {
            PART_ButtonHolder.SizeChanged += PART_ButtonHolder_SizeChanged;
        }
    }

    private void PART_ButtonHolder_SizeChanged(object sender, SizeChangedEventArgs e)
    {
#if WINAPPSDK
        SetDragRegionForCustomTitleBar();
#endif
    }

    private void Configure()
    {
#if WINDOWS_UWP && !HAS_UNO
        SetUWPTitleBar();
#endif
#if WINAPPSDK
    SetWASDKTitleBar();
#endif
    }

    public void Reset()
    {
#if WINDOWS_UWP && !HAS_UNO
        ResetUWPTitleBar();
#endif
#if WINAPPSDK
        ResetWASDKTitleBar();
#endif
    }

    private void Update()
    {
        if (Icon != null)
        {
            VisualStateManager.GoToState(this, IconVisibleState, true);
        }
        else
        {
            VisualStateManager.GoToState(this, IconCollapsedState, true);
        }

        VisualStateManager.GoToState(this, IsBackButtonVisible ? BackButtonVisibleState : BackButtonCollapsedState, true);
        VisualStateManager.GoToState(this, IsPaneButtonVisible ? PaneButtonVisibleState : PaneButtonCollapsedState, true);

        if (DisplayMode == DisplayMode.Tall)
        {
            VisualStateManager.GoToState(this, TallState, true);
        }
        else
        {
            VisualStateManager.GoToState(this, StandardState, true);
        }

        if (Content != null)
        {
            VisualStateManager.GoToState(this, ContentVisibleState, true);
        }
        else
        {
            VisualStateManager.GoToState(this, ContentCollapsedState, true);
        }

        if (Footer != null)
        {
            VisualStateManager.GoToState(this, FooterVisibleState, true);
        }
        else
        {
            VisualStateManager.GoToState(this, FooterCollapsedState, true);
        }

#if WINAPPSDK
        SetDragRegionForCustomTitleBar();
#endif
    }
}
