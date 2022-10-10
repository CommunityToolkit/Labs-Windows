// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.Labs.WinUI;

/// <summary>
/// This is an example control based off of the BoxPanel sample here: https://docs.microsoft.com/windows/apps/design/layout/boxpanel-example-custom-panel. If you need this similar sort of layout component for an application, see UniformGrid in the Toolkit.
/// It is provided as an example of how to inherit from another control like <see cref="Panel"/>.
/// </summary>
public partial class SettingsCard : ButtonBase
{
    private const string NormalState = "Normal";
    private const string PointerOverState = "PointerOver";
    private const string PressedState = "Pressed";
    private const string DisabledState = "Disabled";

    private const string RightState = "Right";
    private const string LeftState = "Left";
    private const string VerticalState = "Vertical";

    private const string ActionIconPresenter = "PART_ActionIconPresenter";
    private const string HeaderPresenter = "PART_HeaderPresenter";
    private const string DescriptionPresenter = "PART_DescriptionPresenter";
    private const string HeaderIconPresenter = "PART_HeaderIconPresenter";
    /// <summary>
    /// Creates a new instance of the <see cref="SettingsCard"/> class.
    /// </summary>
    public SettingsCard()
    {
        this.DefaultStyleKey = typeof(SettingsCard);
    }

    /// <inheritdoc />
    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        IsEnabledChanged -= OnIsEnabledChanged;
        SizeChanged -= OnSizeChanged;
        OnButtonIconChanged();
        OnHeaderChanged();
        OnHeaderIconChanged();
        OnDescriptionChanged();
        OnIsClickEnabledChanged();
        OnContentAlignmentChanged();
        VisualStateManager.GoToState(this, IsEnabled ? NormalState : DisabledState, true);
        RegisterAutomation();
        IsEnabledChanged += OnIsEnabledChanged;
        SizeChanged += OnSizeChanged;
    }

    private void RegisterAutomation()
    {
        if (Header != null && Header.GetType() == typeof(string))
        {
            string? headerString = Header.ToString();
            if (!string.IsNullOrEmpty(headerString))
            {
                AutomationProperties.SetName(this, headerString);
            }

            if (Content != null && Content.GetType() != typeof(Button))
            {
                // We do not want to override the default AutomationProperties.Name of a button. Its Content property already describes what it does.
                if (!string.IsNullOrEmpty(headerString))
                {
                    AutomationProperties.SetName((UIElement)Content, headerString);
                }
            }
        }
    }

    private void EnableButtonInteraction()
    {
        DisableButtonInteraction();

        PointerEntered += Control_PointerEntered;
        PointerExited += Control_PointerExited;
        PreviewKeyDown += Control_PreviewKeyDown;
        PreviewKeyUp += Control_PreviewKeyUp;
    }


    private void DisableButtonInteraction()
    {
        PointerEntered -= Control_PointerEntered;
        PointerExited -= Control_PointerExited;
        PreviewKeyDown -= Control_PreviewKeyDown;
        PreviewKeyUp -= Control_PreviewKeyUp;
    }



    private void Control_PreviewKeyUp(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter || e.Key == Windows.System.VirtualKey.Space || e.Key == Windows.System.VirtualKey.GamepadA)
        {
            VisualStateManager.GoToState(this, NormalState, true);
        }
    }

    private void Control_PreviewKeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter || e.Key == Windows.System.VirtualKey.Space || e.Key == Windows.System.VirtualKey.GamepadA)
        {
            VisualStateManager.GoToState(this, PressedState, true);
        }
    }

    public void Control_PointerExited(object sender, PointerRoutedEventArgs e)
    {
        base.OnPointerExited(e);
        VisualStateManager.GoToState(this, NormalState, true);
    }
    public void Control_PointerEntered(object sender, PointerRoutedEventArgs e)
    {
        base.OnPointerEntered(e);
        VisualStateManager.GoToState(this, PointerOverState, true);
    }

    protected override void OnPointerPressed(PointerRoutedEventArgs e)
    {
        //  e.Handled = true;
        if (IsClickEnabled)
        {
            base.OnPointerPressed(e);
            VisualStateManager.GoToState(this, PressedState, true);
        }
    }
    protected override void OnPointerReleased(PointerRoutedEventArgs e)
    {
        if (IsClickEnabled)
        {
            base.OnPointerReleased(e);
            VisualStateManager.GoToState(this, NormalState, true);
        }
    }


    /// <summary>
    /// Creates AutomationPeer
    /// </summary>
    /// <returns>An automation peer for this <see cref="SettingsCard"/>.</returns>
    protected override AutomationPeer OnCreateAutomationPeer()
    {
        return new SettingsCardAutomationPeer(this);
    }

    private void OnIsClickEnabledChanged()
    {
        OnButtonIconChanged();
        if (IsClickEnabled)
        {
            EnableButtonInteraction();
        }
        else
        {
            DisableButtonInteraction();
        }
    }

    private void OnIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        VisualStateManager.GoToState(this, IsEnabled ? NormalState : DisabledState, true);
    }

    private void OnButtonIconChanged()
    {
        if (GetTemplateChild(ActionIconPresenter) is FrameworkElement buttonIconPresenter)
        {
            buttonIconPresenter.Visibility = IsClickEnabled
                ? Visibility.Visible
                : Visibility.Collapsed;
        }
    }

    private void OnHeaderIconChanged()
    {
        if (GetTemplateChild(HeaderIconPresenter) is FrameworkElement headerIconPresenter)
        {
            headerIconPresenter.Visibility = HeaderIcon != null
                ? Visibility.Visible
                : Visibility.Collapsed;
        }
    }

    private void OnDescriptionChanged()
    {
        if (GetTemplateChild(DescriptionPresenter) is FrameworkElement descriptionPresenter)
        {
            descriptionPresenter.Visibility = Description != null
                ? Visibility.Visible
                : Visibility.Collapsed;
        }
    }

    private void OnHeaderChanged()
    {
        if (GetTemplateChild(HeaderPresenter) is FrameworkElement headerPresenter)
        {
            headerPresenter.Visibility = Header != null
                ? Visibility.Visible
                : Visibility.Collapsed;
        }
    }
    private void OnContentAlignmentChanged()
    {
        //switch (ContentAlignment)
        //{
        //    case ContentAlignment.Right: VisualStateManager.GoToState(this, RightState, true); break;
        //    case ContentAlignment.Left: VisualStateManager.GoToState(this, LeftState, true); break;
        //    case ContentAlignment.Vertical: VisualStateManager.GoToState(this, VerticalState, true); break;
        //}
    }
    private void OnSizeChanged(object sender, SizeChangedEventArgs args)
    {
        //if (ContentAlignment == ContentAlignment.Right)
        //{
        //    if (this.ActualWidth < WrapThreshold)
        //    {
        //        VisualStateManager.GoToState(this, VerticalState, true);
        //    }
        //    else
        //    {
        //        VisualStateManager.GoToState(this, RightState, true);
        //    }           
        //}
    }
}
