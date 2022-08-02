// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.Labs.WinUI;
public partial class SettingsExpander : ItemsControl
{

    /// <summary>
    /// The backing <see cref="DependencyProperty"/> for the <see cref="Header"/> property.
    /// </summary>
    public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(
        nameof(Header),
        typeof(string),
        typeof(SettingsExpander),
        new PropertyMetadata(defaultValue: string.Empty));

    /// <summary>
    /// The backing <see cref="DependencyProperty"/> for the <see cref="Description"/> property.
    /// </summary>
    public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register(
        nameof(Description),
        typeof(object),
        typeof(SettingsExpander),
        new PropertyMetadata(defaultValue: null));

    /// <summary>
    /// The backing <see cref="DependencyProperty"/> for the <see cref="Icon"/> property.
    /// </summary>
    public static readonly DependencyProperty IconProperty = DependencyProperty.Register(
        nameof(Icon),
        typeof(object),
        typeof(SettingsExpander),
        new PropertyMetadata(defaultValue: null));


    /// <summary>
    /// The backing <see cref="DependencyProperty"/> for the <see cref="Content"/> property.
    /// </summary>
    public static readonly DependencyProperty ContentProperty = DependencyProperty.Register(
        nameof(Content),
        typeof(object),
        typeof(SettingsExpander),
        new PropertyMetadata(defaultValue: null));


    public static readonly DependencyProperty IsExpandedProperty = DependencyProperty.Register(
     nameof(IsExpanded),
     typeof(bool),
     typeof(SettingsExpander),
     new PropertyMetadata(defaultValue: false));


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
#pragma warning disable CS0109 // Member does not hide an inherited member; new keyword is not required
    public new object Description
#pragma warning restore CS0109 // Member does not hide an inherited member; new keyword is not required
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
    /// Gets or sets an example string. A basic Content example.
    /// </summary>
    public object Content
    {
        get => (object)GetValue(ContentProperty);
        set => SetValue(ContentProperty, value);
    }

    /// <summary>
    /// Gets or sets an example string. A basic Content example.
    /// </summary>
    public bool IsExpanded
    {
        get => (bool)GetValue(IsExpandedProperty);
        set => SetValue(IsExpandedProperty, value);
    }
}
