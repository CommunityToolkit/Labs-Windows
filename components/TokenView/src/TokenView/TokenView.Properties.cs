// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.Labs.WinUI;

public partial class TokenView : ListViewBase
{
    /// <summary>
    /// The backing <see cref="DependencyProperty"/> for the <see cref="IsWrapped"/> property.
    /// </summary>
    public static readonly DependencyProperty IsWrappedProperty = DependencyProperty.Register(
        nameof(IsWrapped),
        typeof(bool),
        typeof(TokenView),
        new PropertyMetadata(defaultValue: false, (d, e) => ((TokenView)d).OnIsWrappedPropertyChanged((bool)e.OldValue, (bool)e.NewValue)));

    public static readonly DependencyProperty CanRemoveTokensProperty = DependencyProperty.Register(
      nameof(CanRemoveTokens),
      typeof(bool),
      typeof(TokenView),
      new PropertyMetadata(defaultValue: false, (d, e) => ((TokenView)d).OnCanRemoveTokensPropertyChanged((bool)e.OldValue, (bool)e.NewValue)));

    /// <summary>
    /// Gets or sets if tokens can be removed.
    /// </summary>
    public bool CanRemoveTokens
    {
        get => (bool)GetValue(CanRemoveTokensProperty);
        set => SetValue(CanRemoveTokensProperty, value);
    }

    /// <summary>
    /// Gets or sets if tokens are wrapped.
    /// </summary>
    public bool IsWrapped
    {
        get => (bool)GetValue(IsWrappedProperty);
        set => SetValue(IsWrappedProperty, value);
    }

    protected virtual void OnIsWrappedPropertyChanged(bool oldValue, bool newValue)
    {
        OnIsWrappedChanged();
    }

    protected virtual void OnCanRemoveTokensPropertyChanged(bool oldValue, bool newValue)
    {
        OnCanRemoveTokensChanged();
    }
    private void OnCanRemoveTokensChanged()
    {

    }
}
