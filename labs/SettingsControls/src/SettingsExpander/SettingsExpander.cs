// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.Labs.WinUI.Internal;

namespace CommunityToolkit.Labs.WinUI;

//// TODO: Ideally would use ItemsRepeater here, but it has a layout issue
//// trying to request all the available horizontal space: https://github.com/microsoft/microsoft-ui-xaml/issues/3842
public partial class SettingsExpander : ItemsControl
{
    /// <summary>
    /// The SettingsExpander is a collapsable control to host multiple SettingsCards.
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
        if (Header is string headerString && headerString != string.Empty)
        {
            if (!string.IsNullOrEmpty(headerString) && string.IsNullOrEmpty(AutomationProperties.GetName(this)))
            {
                AutomationProperties.SetName(this, headerString);
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

    //// Our <see cref="ItemsControl"/> is to set its effective container to a <see cref="SettingsCard"/>.
    //// We need this to be able to custom style the container within the <see cref="SettingsExpander"/>.
    //// We can't use <see cref="ItemsControl"/> directly as-is because the <see cref="ItemsPresenter"/> automatically
    //// injects data content into the container, creating nested SettingsCards, which we don't want.
    //// It means we can't template the whole <see cref="SettingsCard"/> 'container' similar to the new WinUI patterns
    //// for things like <see cref="MUXC.NavigationView"/> and <see cref="MUXC.TabView"/>.
    //// We can't use <see cref="MUXC.ItemsRepeater"/> due to an issue where it tries to use all horizontal width
    //// within an <see cref="MUXC.Expander"/>. See https://github.com/microsoft/microsoft-ui-xaml/issues/3842.

    protected override bool IsItemItsOwnContainerOverride(object item)
    {
        // Mainly for the Items scenario, if we're already a SettingsCard, we don't have to do anything.
        // And for ItemsSource, if we're a StyledContentPresenter, we're already done our work below.
        return item is SettingsCard or StyledContentPresenter;
    }

    /// <inheritdoc />
    protected override DependencyObject GetContainerForItemOverride()
    {
        //// Note: We can't just use SettingsCard here, as otherwise we get nested SettingsCards with how ItemsControl works,
        //// as it's not built to support the template syntax containing the container.

        // We need to return a ContentPresenter which knows how to Style our inner SettingsCards.
        StyledContentPresenter presenter = new();

        // We want to bind the style selector that we're using here in our control to these new presenters.
        Binding binding = new()
        {
            Source = this,
            Path = new PropertyPath("ItemContainerStyleSelector"),
            Mode = BindingMode.OneWay,
        };
        presenter.SetBinding(StyledContentPresenter.ContentStyleSelectorProperty, binding);

        return presenter;
    }

    private void OnIsExpandedChanged(bool oldValue, bool newValue)
    {
        var peer = FrameworkElementAutomationPeer.FromElement(this) as SettingsExpanderAutomationPeer;
        peer?.RaiseExpandedChangedEvent(newValue);
    }
}
