// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.Labs.WinUI;
public partial class SettingsExpander : ItemsControl
{
    /// <summary>
    /// Creates a new instance of the <see cref="SettingsExpander"/> class.
    /// </summary>
    public SettingsExpander()
    {
        this.DefaultStyleKey = typeof(SettingsExpander);
    }

    /// <inheritdoc />
    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        RegisterAutomation();
    }

    private void RegisterAutomation()
    {
        if (Header.GetType() == typeof(string))
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

    /// <summary>
    /// Creates AutomationPeer
    /// </summary>
    /// <returns>An automation peer for this <see cref="SettingsExpander"/>.</returns>
    protected override AutomationPeer OnCreateAutomationPeer()
    {
        return new SettingsExpanderAutomationPeer(this);
    }
    protected override bool IsItemItsOwnContainerOverride(object item)
    {
        return item is SettingsExpanderItem;
    }

    /// <inheritdoc />
    protected override DependencyObject GetContainerForItemOverride()
    {
        return new SettingsExpanderItem();
    }
}
