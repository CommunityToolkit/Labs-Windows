// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.Labs.WinUI;
public partial class SettingsExpander : ItemsControl
{
    SettingsExpander self;

    /// <summary>
    /// Creates a new instance of the <see cref="SettingsExpander"/> class.
    /// </summary>
    public SettingsExpander()
    {
        this.DefaultStyleKey = typeof(SettingsExpander);

        self = this;
    }

    /// <inheritdoc />
    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        RegisterAutomation();
    }

    private void RegisterAutomation()
    {
        if (!string.IsNullOrEmpty(self.Header))
        {
            AutomationProperties.SetName(this, self.Header);
        }

        if (self.Content != null && self.Content.GetType() != typeof(Button))
        {
            // We do not want to override the default AutomationProperties.Name of a button. Its Content property already describes what it does.
            if (!string.IsNullOrEmpty(self.Header))
            {
                AutomationProperties.SetName((UIElement)self.Content, self.Header);
            }
        }
    }

    /// <summary>
    /// Creates AutomationPeer
    /// </summary>
    /// <returns>An automation peer for this <see cref="SettingsExpander"/>.</returns>
    protected override AutomationPeer OnCreateAutomationPeer()
    {
        return new SettingsExpanderAutomationPeer(this);
    }
}
