// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.Labs.WinUI;
public partial class TokenView : ListViewBase
{
    /// <summary>
    /// The backing <see cref="DependencyProperty"/> for the <see cref="Orientation"/> property.
    /// </summary>
    public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(
        nameof(Orientation),
        typeof(TokenViewOrientation),
        typeof(TokenView),
        new PropertyMetadata(defaultValue: TokenViewOrientation.Horizontal, (d, e) => ((TokenView)d).OnOrientationPropertyChanged((TokenViewOrientation)e.OldValue, (TokenViewOrientation)e.NewValue)));

    public static readonly DependencyProperty CanRemoveTokensProperty = DependencyProperty.Register(
      nameof(CanRemoveTokens),
      typeof(bool),
      typeof(TokenView),
      new PropertyMetadata(defaultValue: false, (d, e) => ((TokenView)d).OnCanRemoveTokensPropertyChanged((bool)e.OldValue, (bool)e.NewValue)));

    /// <summary>
    /// Gets or sets the icon.
    /// </summary>
    public bool CanRemoveTokens
    {
        get => (bool)GetValue(CanRemoveTokensProperty);
        set => SetValue(CanRemoveTokensProperty, value);
    }

    /// <summary>
    /// Gets or sets the icon.
    /// </summary>
    public TokenViewOrientation Orientation
    {
        get => (TokenViewOrientation)GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }

    protected virtual void OnOrientationPropertyChanged(TokenViewOrientation oldValue, TokenViewOrientation newValue)
    {
        OnOrientationChanged();
    }

    protected virtual void OnCanRemoveTokensPropertyChanged(bool oldValue, bool newValue)
    {
        OnCanRemoveTokensChanged();
    }
    private void OnCanRemoveTokensChanged()
    {

    }

    public enum TokenViewOrientation
    {
        Horizontal,
        Wrapped
    }
}
