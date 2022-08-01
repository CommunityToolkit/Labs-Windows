// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.Labs.WinUI;

public partial class SettingsExpanderItem : ContentControl
{

    /// <summary>
    /// The backing <see cref="DependencyProperty"/> for the <see cref="Header"/> property.
    /// </summary>
    public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(
        nameof(Header),
        typeof(string),
        typeof(SettingsExpanderItem),
        new PropertyMetadata(defaultValue: string.Empty));

    /// <summary>
    /// The backing <see cref="DependencyProperty"/> for the <see cref="Header"/> property.
    /// </summary>
    public static readonly DependencyProperty ContentAlignmentProperty = DependencyProperty.Register(
        nameof(ContentAlignment),
        typeof(ContentAlignment),
        typeof(SettingsExpanderItem),
        new PropertyMetadata(defaultValue: ContentAlignment.Right, (d, e) => ((SettingsExpanderItem)d).OnContentAlignmentPropertyChanged((ContentAlignment)e.OldValue, (ContentAlignment)e.NewValue)));

    /// <summary>
    /// Gets or sets an example string. A basic DependencyProperty example.
    /// </summary>
    public string Header
    {
        get => (string)GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    /// Gets or sets an example string. A basic DependencyProperty example.
    /// </summary>
    public ContentAlignment ContentAlignment
    {
        get => (ContentAlignment)GetValue(ContentAlignmentProperty);
        set => SetValue(ContentAlignmentProperty, value);
    }


    protected virtual void OnContentAlignmentPropertyChanged(ContentAlignment oldValue, ContentAlignment newValue)
    {
        OnContentAlignmentChanged();
    }
}

public enum ContentAlignment
{
    Right,
    Left,
    Vertical
}
