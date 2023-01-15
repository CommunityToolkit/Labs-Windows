// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Windows.System;

namespace CommunityToolkit.Labs.WinUI;
public partial class TokenView : ListViewBase
{
    private void TokenView_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        UpdateScrollButtonsVisibility();
    }

    private void ScrollTabBackButton_Click(object sender, RoutedEventArgs e)
    {
        if (_tokenViewScroller != null)
        {
            _tokenViewScroller.ChangeView(_tokenViewScroller.HorizontalOffset - _tokenViewScroller.ViewportWidth, null, null);
        }
    }
    private void ScrollTabForwardButton_Click(object sender, RoutedEventArgs e)
    {
        if (_tokenViewScroller != null)
        {
            _tokenViewScroller.ChangeView(_tokenViewScroller.HorizontalOffset + _tokenViewScroller.ViewportWidth, null, null);
        }
    }

    private void _tokenViewScroller_ViewChanging(object? sender, ScrollViewerViewChangingEventArgs e)
    {
        if (_tokenViewScrollBackButton != null)
        {
            if (e.FinalView.HorizontalOffset < 1)
            {
                _tokenViewScrollBackButton.Visibility = Visibility.Collapsed;
            }
            else if (e.FinalView.HorizontalOffset > 1)
            {
                _tokenViewScrollBackButton.Visibility = Visibility.Visible;
            }
        }

        if (_tokenViewScrollForwardButton != null)
        {
            if (_tokenViewScroller != null)
            {
                if (e.FinalView.HorizontalOffset > _tokenViewScroller.ScrollableWidth - 1)
                {
                    _tokenViewScrollForwardButton.Visibility = Visibility.Collapsed;
                }
                else if (e.FinalView.HorizontalOffset < _tokenViewScroller.ScrollableWidth - 1)
                {
                    _tokenViewScrollForwardButton.Visibility = Visibility.Visible;
                }
            }
        }
    }

    private void ScrollViewer_Loaded(object sender, RoutedEventArgs e)
    {
        if (_tokenViewScroller != null)
        {
            _tokenViewScroller.Loaded -= ScrollViewer_Loaded;
        }
        if (_tokenViewScrollBackButton != null)
        {
            _tokenViewScrollBackButton.Click -= ScrollTabBackButton_Click;
        }

        if (_tokenViewScrollForwardButton != null)
        {
            _tokenViewScrollForwardButton.Click -= ScrollTabForwardButton_Click;
        }

        if (_tokenViewScroller != null)
        {
            _tokenViewScroller.ViewChanging += _tokenViewScroller_ViewChanging;
            _tokenViewScrollBackButton = _tokenViewScroller.FindDescendantByName(TokenViewScrollBackButtonName) as ButtonBase;
            _tokenViewScrollForwardButton = _tokenViewScroller.FindDescendantByName(TokenViewScrollForwardButtonName) as ButtonBase;
        }

        if (_tokenViewScrollBackButton != null)
        {
            _tokenViewScrollBackButton.Click += ScrollTabBackButton_Click;
        }

        if (_tokenViewScrollForwardButton != null)
        {
            _tokenViewScrollForwardButton.Click += ScrollTabForwardButton_Click;
        }

        UpdateScrollButtonsVisibility();
    }

    private void Token_Removing(object? sender, TokenItemRemovingEventArgs e)
    {
        var item = ItemFromContainer(e.TokenItem);

        var args = new TokenItemRemovingEventArgs(item, e.TokenItem);
        TokenItemRemoving?.Invoke(this, args);

        if (ItemsSource != null)
        {
            _removeItemsSourceMethod?.Invoke(ItemsSource, new object[] { item });
        }
        else
        {
            if (_tokenViewScroller != null)
            {
                _tokenViewScroller.UpdateLayout();
            }
            Items.Remove(item);
        }

        UpdateScrollButtonsVisibility();
    }

    private void Token_Loaded(object sender, RoutedEventArgs e)
    {
        var token = sender as TokenItem;

        if (token != null)
        {
            token.Loaded -= Token_Loaded;
        }

        // Only need to do this once.
        if (!_hasLoaded)
        {
            _hasLoaded = true;

            // Need to set a tab's selection on load, otherwise ListView resets to null.
            SetInitialSelection();
        }
    }

    private void OnOrientationChanged()
    {
        if (_tokenViewScroller != null)
        {
            if (Orientation == TokenViewOrientation.Horizontal)
            {
                _tokenViewScroller.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
            }
            else if (this.Orientation == TokenViewOrientation.Wrapped)
            {
                _tokenViewScroller.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
            }
        }
    }

    private void TokenView_PreviewKeyDown(object sender, KeyRoutedEventArgs e)
    {
        switch (e.Key)
        {
            case VirtualKey.Left: e.Handled = MoveFocus(MoveDirection.Previous); break;
            case VirtualKey.Right: e.Handled = MoveFocus(MoveDirection.Next); break;
            case VirtualKey.Back:
            case VirtualKey.Delete: e.Handled = RemoveItem(); break;
        }
    }
}
