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
        typeof(string),
        typeof(SettingsCard),
        new PropertyMetadata(defaultValue: string.Empty, (d, e) => ((SettingsCard)d).OnHeaderPropertyChanged((string)e.OldValue, (string)e.NewValue)));

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
    public static readonly DependencyProperty IconProperty = DependencyProperty.Register(
        nameof(Icon),
        typeof(object),
        typeof(SettingsCard),
        new PropertyMetadata(defaultValue: null, (d, e) => ((SettingsCard)d).OnIconPropertyChanged((object)e.OldValue, (object)e.NewValue)));

    /// <summary>
    /// The backing <see cref="DependencyProperty"/> for the <see cref="IsClickEnabled"/> property.
    /// </summary>
    public static readonly DependencyProperty IsClickEnabledProperty = DependencyProperty.Register(
        nameof(IsClickEnabled),
        typeof(bool),
        typeof(SettingsCard),
        new PropertyMetadata(defaultValue: false, (d, e) => ((SettingsCard)d).OnIsClickEnabledPropertyChanged((bool)e.OldValue, (bool)e.NewValue)));


    /// <summary>
    /// Gets or sets an example string. A basic DependencyProperty example.
    /// </summary>
    public string Header
    {
        get => (string)GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    /// <summary>
    /// Gets or sets an example string. A basic Description example.
    /// </summary>
    public new object Description
    {
        get => (object)GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }

    /// <summary>
    /// Gets or sets an example string. A basic DependencyProperty example.
    /// </summary>
    public object Icon
    {
        get => (object)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    /// <summary>
    /// Gets or sets an example string. A basic Description example.
    /// </summary>
    public bool IsClickEnabled
    {
        get => (bool)GetValue(IsClickEnabledProperty);
        set => SetValue(IsClickEnabledProperty, value);
    }


    protected virtual void OnHeaderPropertyChanged(string oldValue, string newValue)
    {
        // Do something with the changed value.
    }

    protected virtual void OnIconPropertyChanged(object oldValue, object newValue)
    {
        OnIconChanged();
    }

    protected virtual void OnDescriptionPropertyChanged(object oldValue, object newValue)
    {
        OnDescriptionChanged();
    }

    protected virtual void OnIsClickEnabledPropertyChanged(bool oldValue, bool newValue)
    {
        OnIsClickEnabledChanged();
    }
}
