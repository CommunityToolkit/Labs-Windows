// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.Labs.WinUI;
public partial class TokenItem : ListViewItem
{
    /// <summary>
    /// Identifies the <see cref="IsRemoveable"/> dependency property.
    /// </summary>
    /// <returns>The identifier for the <see cref="IsRemoveable"/> dependency property.</returns>
    public static readonly DependencyProperty IsRemoveableProperty =
        DependencyProperty.Register(nameof(IsRemoveable), typeof(bool), typeof(TokenItem), new PropertyMetadata(defaultValue: false, (d, e) => ((TokenItem)d).OnIsRemoveablePropertyChanged((bool)e.OldValue, (bool)e.NewValue)));

    /// <summary>
    /// The backing <see cref="DependencyProperty"/> for the <see cref="Icon"/> property.
    /// </summary>
    public static readonly DependencyProperty IconProperty = DependencyProperty.Register(
        nameof(Icon),
        typeof(IconElement),
        typeof(TokenItem),
        new PropertyMetadata(defaultValue: null, (d, e) => ((TokenItem)d).OnIconPropertyChanged((IconElement)e.OldValue, (IconElement)e.NewValue)));

    /// <summary>
    /// Gets or sets a value indicating whether the tab can be closed by the user with the close button.
    /// </summary>
    public bool IsRemoveable
    {
        get { return (bool)GetValue(IsRemoveableProperty); }
        set { SetValue(IsRemoveableProperty, value); }
    }

    /// <summary>
    /// Gets or sets the icon.
    /// </summary>
    public IconElement Icon
    {
        get => (IconElement)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }
}
