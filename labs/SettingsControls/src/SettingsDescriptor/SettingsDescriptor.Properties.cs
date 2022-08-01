// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.Labs.WinUI;

public partial class SettingsDescriptor : ContentControl
{

    /// <summary>
    /// The backing <see cref="DependencyProperty"/> for the <see cref="Header"/> property.
    /// </summary>
    public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(
        nameof(Header),
        typeof(string),
        typeof(SettingsDescriptor),
        new PropertyMetadata(defaultValue: string.Empty));

    /// <summary>
    /// The backing <see cref="DependencyProperty"/> for the <see cref="Description"/> property.
    /// </summary>
    public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register(
        nameof(Description),
        typeof(object),
        typeof(SettingsDescriptor),
        new PropertyMetadata(defaultValue: null, (d, e) => ((SettingsDescriptor)d).OnDescriptionPropertyChanged((object)e.OldValue, (object)e.NewValue)));

    /// <summary>
    /// The backing <see cref="DependencyProperty"/> for the <see cref="Icon"/> property.
    /// </summary>
    public static readonly DependencyProperty IconProperty = DependencyProperty.Register(
        nameof(Icon),
        typeof(object),
        typeof(SettingsDescriptor),
        new PropertyMetadata(defaultValue: null, (d, e) => ((SettingsDescriptor)d).OnIconPropertyChanged((string)e.OldValue, (string)e.NewValue)));

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

    protected virtual void OnIconPropertyChanged(string oldValue, string newValue)
    {
        OnIconChanged();
    }

    protected virtual void OnDescriptionPropertyChanged(object oldValue, object newValue)
    {
        OnDescriptionChanged();
    }
}
