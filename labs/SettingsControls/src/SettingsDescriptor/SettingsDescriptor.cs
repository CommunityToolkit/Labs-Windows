// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.Labs.WinUI;

[TemplateVisualState(Name = IconVisibleState, GroupName = IconStates)]
[TemplateVisualState(Name = IconCollapsedState, GroupName = IconStates)]
[TemplateVisualState(Name = DescriptionVisibleState, GroupName = DescriptionStates)]
[TemplateVisualState(Name = DescriptionCollapsedState, GroupName = DescriptionStates)]
public partial class SettingsDescriptor : ContentControl
{
    SettingsDescriptor self;
    private const string NormalState = "Normal";
    private const string DisabledState = "Disabled";

    private const string IconStates = "IconStates";
    private const string IconVisibleState = "IconVisible";
    private const string IconCollapsedState = "IconCollapsed";

    private const string DescriptionStates = "DescriptionStates";
    private const string DescriptionVisibleState = "DescriptionVisible";
    private const string DescriptionCollapsedState = "DescriptionCollapsed";

    /// <summary>
    /// Creates a new instance of the <see cref="SettingsDescriptor"/> class.
    /// </summary>
    public SettingsDescriptor()
    {
        this.DefaultStyleKey = typeof(SettingsDescriptor);
        self = this;
    }

    /// <inheritdoc />
    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        IsEnabledChanged -= OnIsEnabledChanged;

        OnIconChanged();
        OnDescriptionChanged();
        VisualStateManager.GoToState(this, self.IsEnabled ? NormalState : DisabledState, true);
        IsEnabledChanged += OnIsEnabledChanged;
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
