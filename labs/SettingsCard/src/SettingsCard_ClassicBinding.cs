// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.Labs.WinUI;

/// <summary>
/// An example templated control.
/// </summary>
[TemplateVisualState(Name = IconVisibleState, GroupName = IconStates)]
[TemplateVisualState(Name = IconCollapsedState, GroupName = IconStates)]
[TemplateVisualState(Name = DescriptionVisibleState, GroupName = DescriptionStates)]
[TemplateVisualState(Name = DescriptionCollapsedState, GroupName = DescriptionStates)]
public partial class SettingsCard_ClassicBinding : ButtonBase
{
    SettingsCard_ClassicBinding self;
    private const string NormalState = "Normal";
    private const string PointerOverState = "PointerOver";
    private const string PressedState = "Pressed";
    private const string DisabledState = "Disabled";

    private const string IconStates = "IconStates";
    private const string IconVisibleState = "IconVisible";
    private const string IconCollapsedState = "IconCollapsed";

    private const string DescriptionStates = "DescriptionStates";
    private const string DescriptionVisibleState = "DescriptionVisible";
    private const string DescriptionCollapsedState = "DescriptionCollapsed";

    /// <summary>
    /// Creates a new instance of the <see cref="SettingsCard_ClassicBinding"/> class.
    /// </summary>
    public SettingsCard_ClassicBinding()
    {
        this.DefaultStyleKey = typeof(SettingsCard_ClassicBinding);
        self = this;
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        IsEnabledChanged -= OnIsEnabledChanged;
        OnIsClickEnabledChanged();
        OnIconChanged();
        OnDescriptionChanged();
        VisualStateManager.GoToState(this, self.IsEnabled ? NormalState : DisabledState, true);
        RegisterAutomation();

        IsEnabledChanged += OnIsEnabledChanged;
    }

    private void RegisterAutomation()
    {
        if (self.IsClickEnabled)
        {
            if (!string.IsNullOrEmpty(self.Title))
            {
                AutomationProperties.SetName(this, self.Title);
            }
        }
        else
        {
            if (self.Content != null && self.Content.GetType() != typeof(Button))
            {
                // We do not want to override the default AutomationProperties.Name of a button. Its Content property already describes what it does.
                if (!string.IsNullOrEmpty(self.Title))
                {
                    AutomationProperties.SetName((UIElement)self.Content, self.Title);
                }
            }
        }
    }

    private void EnableButtonInteraction()
    {
        DisableButtonInteraction();

        IsTabStop = true;
        // UseSystemFocusVisuals = true;
        PointerEntered += Control_PointerEntered;
        PointerExited += Control_PointerExited;
        PointerPressed += Control_PointerPressed; // TO DO: THIS EVENT DOES NOT SEEM TO EXIST IN BUTTONBASE, ONLY UIELEMENT?
        PointerReleased += Control_PointerReleased;
        PreviewKeyDown += Control_PreviewKeyDown;
        PreviewKeyUp += Control_PreviewKeyUp;
    }


    private void DisableButtonInteraction()
    {
        IsTabStop = false;
        // UseSystemFocusVisuals = false;
        PointerEntered -= Control_PointerEntered;
        PointerExited -= Control_PointerExited;
        PointerPressed -= Control_PointerPressed;
        PointerReleased -= Control_PointerReleased;
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

    public void Control_PointerReleased(object sender, PointerRoutedEventArgs e)
    {
        base.OnPointerReleased(e);
        VisualStateManager.GoToState(this, NormalState, true);
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

    private void Control_PointerPressed(object sender, PointerRoutedEventArgs e)
    {
        base.OnPointerPressed(e);
        VisualStateManager.GoToState(this, PressedState, true);
    }

    /// <summary>
    /// Creates AutomationPeer
    /// </summary>
    /// <returns>An automation peer for this <see cref="SettingsCard_ClassicBinding"/>.</returns>
    protected override AutomationPeer OnCreateAutomationPeer()
    {
        return new SettingsCard_ClassicBindingAutomationPeer(this);
    }

    private void OnIsClickEnabledChanged()
    {
        // TO DO: DISABLE THE CLICK EVENT
        if (IsClickEnabled)
        {
            EnableButtonInteraction();
        }
        else
        {
            DisableButtonInteraction();
        }
    }

    private void OnIconChanged()
    {
        if (self.Icon != null)
        {
            VisualStateManager.GoToState(this, IconVisibleState, true);
        }
        else
        {
            VisualStateManager.GoToState(this, IconCollapsedState, true);
        }
    }

    private void OnDescriptionChanged()
    {
        if (self.Description != null)
        {
            VisualStateManager.GoToState(this, DescriptionVisibleState, true);
        }
        else
        {
            VisualStateManager.GoToState(this, DescriptionCollapsedState, true);
        }
    }

    private void OnIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        VisualStateManager.GoToState(this, self.IsEnabled ? NormalState : DisabledState, true);
    }
}
