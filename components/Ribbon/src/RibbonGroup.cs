// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.WinUI.Controls;

/// <summary>
/// A basic group displayed in a <see cref="Ribbon"/> control.
/// It adds a <see cref="Label"/> to the wrapped <see cref="Content"/>.
/// </summary>
[ContentProperty(Name = nameof(Content))]
public partial class RibbonGroup : Control
{
    /// <summary>
    /// The DP to store the <see cref="Content"/> property value.
    /// </summary>
    public static readonly DependencyProperty ContentProperty = DependencyProperty.Register(
        nameof(Content),
        typeof(UIElement),
        typeof(RibbonGroup),
        new PropertyMetadata(null));

    /// <summary>
    /// The content of the group.
    /// </summary>
    public UIElement Content
    {
        get => (UIElement)GetValue(ContentProperty);
        set => SetValue(ContentProperty, value);
    }

    /// <summary>
    /// The DP to store the <see cref="Label"/> property value.
    /// </summary>
    public static readonly DependencyProperty LabelProperty = DependencyProperty.Register(
        nameof(Label),
        typeof(string),
        typeof(RibbonGroup),
        new PropertyMetadata(""));

    /// <summary>
    /// The label of the group.
    /// </summary>
    public string Label
    {
        get => (string)GetValue(LabelProperty);
        set => SetValue(LabelProperty, value);
    }

    public RibbonGroup() => DefaultStyleKey = typeof(RibbonGroup);
}
