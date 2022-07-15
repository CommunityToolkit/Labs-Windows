// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;

namespace CommunityToolkit.Labs.WinUI;
public partial class SettingsCard_ClassicBinding : ButtonBase
{
    /// <summary>
    /// The backing <see cref="DependencyProperty"/> for the <see cref="Title"/> property.
    /// </summary>
    public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
        nameof(Title),
        typeof(string),
        typeof(SettingsCard_ClassicBinding),
        new PropertyMetadata(defaultValue: string.Empty, (d, e) => ((SettingsCard_ClassicBinding)d).OnTitlePropertyChanged((string)e.OldValue, (string)e.NewValue)));

    /// <summary>
    /// The backing <see cref="DependencyProperty"/> for the <see cref="Description"/> property.
    /// </summary>
    public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register(
        nameof(Description),
        typeof(object),
        typeof(SettingsCard_ClassicBinding),
        new PropertyMetadata(defaultValue: null, (d, e) => ((SettingsCard_ClassicBinding)d).OnDescriptionPropertyChanged((object)e.OldValue, (object)e.NewValue)));

    /// <summary>
    /// The backing <see cref="DependencyProperty"/> for the <see cref="Icon"/> property.
    /// </summary>
    public static readonly DependencyProperty IconProperty = DependencyProperty.Register(
        nameof(Icon),
        typeof(object),
        typeof(SettingsCard_ClassicBinding),
        new PropertyMetadata(defaultValue: null, (d, e) => ((SettingsCard_ClassicBinding)d).OnIconPropertyChanged((string)e.OldValue, (string)e.NewValue)));

    /// <summary>
    /// The backing <see cref="DependencyProperty"/> for the <see cref="IsClickEnabled"/> property.
    /// </summary>
    public static readonly DependencyProperty IsClickEnabledProperty = DependencyProperty.Register(
        nameof(IsClickEnabled),
        typeof(bool),
        typeof(SettingsCard_ClassicBinding),
        new PropertyMetadata(defaultValue: false, (d, e) => ((SettingsCard_ClassicBinding)d).OnIsClickEnabledPropertyChanged((bool)e.OldValue, (bool)e.NewValue)));


    /// <summary>
    /// Gets or sets an example string. A basic DependencyProperty example.
    /// </summary>
    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    /// <summary>
    /// Gets or sets an example string. A basic Description example.
    /// </summary>
    public object Description
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


    protected virtual void OnTitlePropertyChanged(string oldValue, string newValue)
    {
        // Do something with the changed value.
    }

    protected virtual void OnIconPropertyChanged(string oldValue, string newValue)
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
