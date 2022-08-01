// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.Labs.WinUI;

[TemplateVisualState(Name = RightState, GroupName = ContentAlignmentStates)]
[TemplateVisualState(Name = LeftState, GroupName = ContentAlignmentStates)]
[TemplateVisualState(Name = VerticalState, GroupName = ContentAlignmentStates)]
public partial class SettingsExpanderItem : ContentControl
{
    SettingsExpanderItem self;
    private const string NormalState = "Normal";
    private const string DisabledState = "Disabled";

    private const string ContentAlignmentStates = "ContentAlignmentStates";
    private const string RightState = "Right";
    private const string LeftState = "Left";
    private const string VerticalState = "Vertical";
    /// <summary>
    /// Creates a new instance of the <see cref="SettingsExpanderItem"/> class.
    /// </summary>
    public SettingsExpanderItem()
    {
        this.DefaultStyleKey = typeof(SettingsExpanderItem);
        self = this;
    }

    /// <inheritdoc />
    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        IsEnabledChanged -= OnIsEnabledChanged;
        RegisterAutomation();
        VisualStateManager.GoToState(this, self.IsEnabled ? NormalState : DisabledState, true);
        OnContentAlignmentChanged();
        IsEnabledChanged += OnIsEnabledChanged;
    }
    private void OnIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        VisualStateManager.GoToState(this, self.IsEnabled ? NormalState : DisabledState, true);
    }

    private void OnContentAlignmentChanged()
    {
        switch (self.ContentAlignment)
        {
            case ContentAlignment.Right: VisualStateManager.GoToState(this, RightState, true); break;
            case ContentAlignment.Left: VisualStateManager.GoToState(this, LeftState, true); break;
            case ContentAlignment.Vertical: VisualStateManager.GoToState(this, VerticalState, true); break;
        }
    }

    private void RegisterAutomation()
    {
        if (!string.IsNullOrEmpty(Header))
        {
            AutomationProperties.SetName(this, Header);
            // TO DO: SET DESCRIPTION AS HELPTEXT
        }

        if (self.Content != null && self.Content.GetType() != typeof(Button))
        {
            // We do not want to override the default AutomationProperties.Name of a button. Its Content property already describes what it does.
            if (!string.IsNullOrEmpty(Header))
            {
                AutomationProperties.SetName((UIElement)self.Content, Header);
                // TO DO: SET DESCRIPTION AS HELPTEXT?
            }
        }
    }
}
