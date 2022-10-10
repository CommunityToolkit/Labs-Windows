// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.Labs.WinUI;

[ContentProperty(Name = nameof(Content))]
public partial class SettingsExpander : ItemsControl
{

    /// <summary>
    /// The backing <see cref="DependencyProperty"/> for the <see cref="Header"/> property.
    /// </summary>
    public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(
        nameof(Header),
        typeof(object),
        typeof(SettingsExpander),
        new PropertyMetadata(defaultValue: null));

    /// <summary>
    /// The backing <see cref="DependencyProperty"/> for the <see cref="Description"/> property.
    /// </summary>
    public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register(
        nameof(Description),
        typeof(object),
        typeof(SettingsExpander),
        new PropertyMetadata(defaultValue: null));

    /// <summary>
    /// The backing <see cref="DependencyProperty"/> for the <see cref="HeaderIcon"/> property.
    /// </summary>
    public static readonly DependencyProperty HeaderIconProperty = DependencyProperty.Register(
        nameof(HeaderIcon),
        typeof(IconElement),
        typeof(SettingsExpander),
        new PropertyMetadata(defaultValue: null));


    /// <summary>
    /// The backing <see cref="DependencyProperty"/> for the <see cref="Content"/> property.
    /// </summary>
    public static readonly DependencyProperty ContentProperty = DependencyProperty.Register(
        nameof(Content),
        typeof(UIElement),
        typeof(SettingsExpander),
        new PropertyMetadata(defaultValue: null));


    public static readonly DependencyProperty IsExpandedProperty = DependencyProperty.Register(
     nameof(IsExpanded),
     typeof(bool),
     typeof(SettingsExpander),
     new PropertyMetadata(defaultValue: false, (d, e) => ((SettingsExpander)d).OnIsExpandedChanged((bool)e.OldValue, (bool)e.NewValue)));

    /// <summary>
    /// The backing <see cref="DependencyProperty"/> for the <see cref="WrapThreshold"/> property.
    /// </summary>
    public static readonly DependencyProperty WrapThresholdProperty = DependencyProperty.Register(
        nameof(WrapThreshold),
        typeof(double),
        typeof(SettingsExpander),
        new PropertyMetadata(defaultValue: 0.0));

    /// <summary>
    /// 
    /// <summary>
    /// Gets or sets the Header.
    /// </summary>
    public object Header
    {
        get => (object)GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    /// <summary>
    /// Gets or sets the Description.
    /// </summary>
#pragma warning disable CS0109 // Member does not hide an inherited member; new keyword is not required
    public new object Description
#pragma warning restore CS0109 // Member does not hide an inherited member; new keyword is not required
    {
        get => (object)GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }

    /// <summary>
    /// Gets or sets the HeaderIcon.
    /// </summary>
    public IconElement HeaderIcon
    {
        get => (IconElement)GetValue(HeaderIconProperty);
        set => SetValue(HeaderIconProperty, value);
    }

    /// <summary>
    /// Gets or sets the Content.
    /// </summary>
    public UIElement Content
    {
        get => (UIElement)GetValue(ContentProperty);
        set => SetValue(ContentProperty, value);
    }

    /// <summary>
    /// Gets or sets the IsExpanded state.
    /// </summary>
    public bool IsExpanded
    {
        get => (bool)GetValue(IsExpandedProperty);
        set => SetValue(IsExpandedProperty, value);
    }

    /// <summary>
    /// Gets or sets the WrapThreshold of when the content should vertically align
    /// </summary>
    public double WrapThreshold
    {
        get => (double)GetValue(WrapThresholdProperty);
        set => SetValue(WrapThresholdProperty, value);
    }
    protected virtual void OnIsExpandedChanged(bool oldValue, bool newValue)
    {
    }
}