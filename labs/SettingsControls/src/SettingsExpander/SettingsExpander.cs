// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.Labs.WinUI;

// TODO: Ideally would use ItemsRepeater here, but it has a layout issue trying to request all the available horizontal space. TODO: File WinUI bug.
[TemplatePart(Name = PART_ItemsControl, Type = typeof(ItemsControl))]
public partial class SettingsExpander : Control
{
    private const string PART_ItemsControl = "PART_ItemsControl";

    //// We need a reference to the Items Control to set it's source dynamically between ItemsSource or Items depending on what's defined. ItemsSource takes precedence.
    private ItemsControl? _itemsControl;

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

        _itemsControl = GetTemplateChild(PART_ItemsControl) as ItemsControl;

        if (_itemsControl != null)
        {
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
