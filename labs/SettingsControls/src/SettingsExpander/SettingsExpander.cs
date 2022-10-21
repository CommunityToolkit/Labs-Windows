// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.Labs.WinUI;

[TemplatePart(Name = PART_ItemsRepeater, Type = typeof(MUXC.ItemsRepeater))]
public partial class SettingsExpander : Control
{
    private const string PART_ItemsRepeater = "PART_ItemsRepeater";

    private MUXC.ItemsRepeater? _itemsRepeater;

    /// <summary>
    /// The SettingsExpander is a collapsable control to host multiple SettingsCards.
    /// </summary>
    public SettingsExpander()
    {
        this.DefaultStyleKey = typeof(SettingsExpander);

        Items = new List<object>();
    }

    /// <inheritdoc />
    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        RegisterAutomation();

        if (_itemsRepeater != null)
        {
            _itemsRepeater.ElementPrepared -= this.ItemsRepeater_ElementPrepared;
        }

        _itemsRepeater = GetTemplateChild(PART_ItemsRepeater) as MUXC.ItemsRepeater;

        if (_itemsRepeater != null)
        {
            _itemsRepeater.ElementPrepared += this.ItemsRepeater_ElementPrepared;

            // Update it's source based on our current items properties.
            OnItemsConnectedPropertyChanged(this, null!); // Can't get it to accept type here? (DependencyPropertyChangedEventArgs)EventArgs.Empty
        }
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

    /// <summary>
    /// Creates AutomationPeer
    /// </summary>
    /// <returns>An automation peer for <see cref="SettingsExpander"/>.</returns>
    protected override AutomationPeer OnCreateAutomationPeer()
    {
        return new SettingsExpanderAutomationPeer(this);
    }

    private void OnIsExpandedChanged(bool oldValue, bool newValue)
    {
        var peer = FrameworkElementAutomationPeer.FromElement(this) as SettingsExpanderAutomationPeer;
        peer?.RaiseExpandedChangedEvent(newValue);
    }
}
