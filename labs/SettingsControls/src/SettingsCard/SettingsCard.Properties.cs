// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;

namespace CommunityToolkit.Labs.WinUI;
public partial class SettingsCard : ButtonBase
{
    /// <summary>
    /// The backing <see cref="DependencyProperty"/> for the <see cref="Header"/> property.
    /// </summary>
    public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(
        nameof(Header),
        typeof(object),
        typeof(SettingsCard),
        new PropertyMetadata(defaultValue: null, (d, e) => ((SettingsCard)d).OnHeaderPropertyChanged((object)e.OldValue, (object)e.NewValue)));

    /// <summary>
    /// The backing <see cref="DependencyProperty"/> for the <see cref="Description"/> property.
    /// </summary>
    public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register(
        nameof(Description),
        typeof(object),
        typeof(SettingsCard),
        new PropertyMetadata(defaultValue: null, (d, e) => ((SettingsCard)d).OnDescriptionPropertyChanged((object)e.OldValue, (object)e.NewValue)));

    /// <summary>
    /// The backing <see cref="DependencyProperty"/> for the <see cref="Icon"/> property.
    /// </summary>
    public static readonly DependencyProperty HeaderIconProperty = DependencyProperty.Register(
        nameof(HeaderIcon),
        typeof(IconElement),
        typeof(SettingsCard),
        new PropertyMetadata(defaultValue: null, (d, e) => ((SettingsCard)d).OnHeaderIconPropertyChanged((IconElement)e.OldValue, (IconElement)e.NewValue)));

    /// <summary>
    /// The backing <see cref="DependencyProperty"/> for the <see cref="ButtonIcon"/> property.
    /// </summary>
    public static readonly DependencyProperty ButtonIconProperty = DependencyProperty.Register(
        nameof(ButtonIcon),
        typeof(object),
        typeof(SettingsCard),
        new PropertyMetadata(defaultValue: "\ue974"));

    /// <summary>
    /// The backing <see cref="DependencyProperty"/> for the <see cref="ButtonIconToolTip"/> property.
    /// </summary>
    public static readonly DependencyProperty ButtonIconToolTipProperty = DependencyProperty.Register(
        nameof(ButtonIconToolTip),
        typeof(string),
        typeof(SettingsCard),
        new PropertyMetadata(defaultValue: "More"));


    /// <summary>
    /// The backing <see cref="DependencyProperty"/> for the <see cref="IsClickEnabled"/> property.
    /// </summary>
    public static readonly DependencyProperty IsClickEnabledProperty = DependencyProperty.Register(
        nameof(IsClickEnabled),
        typeof(bool),
        typeof(SettingsCard),
        new PropertyMetadata(defaultValue: false, (d, e) => ((SettingsCard)d).OnIsClickEnabledPropertyChanged((bool)e.OldValue, (bool)e.NewValue)));


    /// <summary>
    /// Gets or sets the Header.
    /// </summary>
    public object Header
    {
        get => (object)GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    /// <summary>
    /// Gets or sets the description.
    /// </summary>
#pragma warning disable CS0109 // Member does not hide an inherited member; new keyword is not required
    public new object Description
#pragma warning restore CS0109 // Member does not hide an inherited member; new keyword is not required
    {
        get => (object)GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }

    /// <summary>
    /// Gets or sets the main icon.
    /// </summary>
    public IconElement HeaderIcon
    {
        get => (IconElement)GetValue(HeaderIconProperty);
        set => SetValue(HeaderIconProperty, value);
    }

    /// <summary>
    /// Gets or sets the icon that is shown when IsClickEnabled is set to true.
    /// </summary>
    public IconElement ButtonIcon
    {
        get => (IconElement)GetValue(ButtonIconProperty);
        set => SetValue(ButtonIconProperty, value);
    }

    /// <summary>
    /// Gets or sets the tooltip of the ButtonIcon.
    /// </summary>
    public string ButtonIconToolTip
    {
        get => (string)GetValue(ButtonIconToolTipProperty);
        set => SetValue(ButtonIconToolTipProperty, value);
    }

    /// <summary>
    /// Gets or sets if the card can be clicked.
    /// </summary>
    public bool IsClickEnabled
    {
        get => (bool)GetValue(IsClickEnabledProperty);
        set => SetValue(IsClickEnabledProperty, value);
    }

    protected virtual void OnIsClickEnabledPropertyChanged(bool oldValue, bool newValue)
    {
        OnIsClickEnabledChanged();
    }
    protected virtual void OnHeaderIconPropertyChanged(IconElement oldValue, IconElement newValue)
    {
        OnHeaderIconChanged();
    }

    protected virtual void OnHeaderPropertyChanged(object oldValue, object newValue)
    {
        OnHeaderChanged();
    }

    protected virtual void OnDescriptionPropertyChanged(object oldValue, object newValue)
    {
        OnDescriptionChanged();
    }
}
