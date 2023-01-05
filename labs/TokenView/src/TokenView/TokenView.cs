// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Windows.System;

namespace CommunityToolkit.Labs.WinUI;

/// <summary>
/// This is an controĺ easily visualize tokens, to create filtering experiences.
/// </summary>

[TemplatePart(Name = TokenViewScrollViewerName, Type = typeof(ScrollViewer))]
[TemplatePart(Name = TokenViewScrollBackButtonName, Type = typeof(ButtonBase))]
[TemplatePart(Name = TokenViewScrollForwardButtonName, Type = typeof(ButtonBase))]
public partial class TokenView : ListViewBase
{
    /// <summary>
    /// Occurs when a tab's Close button is clicked.  Set <see cref="TokenRemovingEventArgs.Cancel"/> to true to prevent automatic Tab Closure.
    /// </summary>
    ///

    private const string? TokenViewScrollViewerName = "ScrollViewer";
    private const string? TokenViewScrollBackButtonName = "ScrollBackButton";
    private const string? TokenViewScrollForwardButtonName = "ScrollForwardButton";
    public event EventHandler<TokenRemovingEventArgs>? TokenRemoving;

    private ScrollViewer? _tokenViewScroller;
    private ButtonBase? _tokenViewScrollBackButton;
    private ButtonBase? _tokenViewScrollForwardButton;

    /// <summary>
    /// Creates a new instance of the <see cref="TokenView"/> class.
    /// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public TokenView()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {
        this.DefaultStyleKey = typeof(TokenView);
    }

    /// <inheritdoc />
    ///
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

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        PreviewKeyDown -= TokenView_PreviewKeyDown;
        PreviewKeyDown += TokenView_PreviewKeyDown;

        this.SizeChanged += TokenView_SizeChanged;
        if (_tokenViewScroller != null)
        {
            _tokenViewScroller.Loaded -= ScrollViewer_Loaded;
        }

        _tokenViewScroller = GetTemplateChild(TokenViewScrollViewerName) as ScrollViewer;

        if (_tokenViewScroller != null)
        {
            _tokenViewScroller.Loaded += ScrollViewer_Loaded;
        }

        OnOrientationChanged();
    }

    private void TokenView_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        SetButtons();
    }

    protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
    {
        base.PrepareContainerForItemOverride(element, item);

        if (element is Token Token)
        {
            Token.Loaded += Token_Loaded;
            Token.Removing += Token_Removing;

            if (Token.IsRemoveable != true && Token.ReadLocalValue(Token.IsRemoveableProperty) == DependencyProperty.UnsetValue)
            {
                var iscloseablebinding = new Binding()
                {
                    Source = this,
                    Path = new PropertyPath(nameof(CanRemoveTokens)),
                    Mode = BindingMode.OneWay,
                };
                Token.SetBinding(Token.IsRemoveableProperty, iscloseablebinding);
            }
        }
    }

    private void TokenView_PreviewKeyDown(object sender, KeyRoutedEventArgs e)
    {
        switch (e.Key)
        {
            case VirtualKey.Left: e.Handled = MoveFocus(MoveDirection.Previous); break;
            case VirtualKey.Right: e.Handled = MoveFocus(MoveDirection.Next); break;
            case VirtualKey.Delete: e.Handled = RemoveItem(); break;
        }
    }
    private bool RemoveItem()
    {
        var currentContainerItem = GetCurrentContainerItem();
        if (currentContainerItem != null && currentContainerItem.IsRemoveable)
        {
            Items.Remove(currentContainerItem);
            return true;
        }
        else
        {
            return false;
        }
    }
    private bool _hasLoaded;
    private void Token_Loaded(object sender, RoutedEventArgs e)
    {
        var Token = sender as Token;

        if (Token != null)
        {
            Token.Loaded -= Token_Loaded;
        }

        // Only need to do this once.
        if (!_hasLoaded)
        {
            _hasLoaded = true;

            // Need to set a tab's selection on load, otherwise ListView resets to null.
            SetInitialSelection();
        }
    }

    private void SetInitialSelection()
    {
        if (SelectedItem == null)
        {
            // If we have an index, but didn't get the selection, make the selection
            if (SelectedIndex >= 0 && SelectedIndex < Items.Count)
            {
                SelectedItem = Items[SelectedIndex];
            }
        }
        else
        {
            if (SelectedIndex >= 0 && SelectedIndex < Items.Count)
            {
                SelectedItem = Items[SelectedIndex];
            }
        }
    }

    // Temporary tracking of previous collections for removing events.
    private MethodInfo _removeItemsSourceMethod;

    private void Token_Removing(object? sender, TokenRemovingEventArgs e)
    {
        var item = ItemFromContainer(e.Token);

        var args = new TokenRemovingEventArgs(item, e.Token);
        TokenRemoving?.Invoke(this, args);

        if (!args.Cancel)
        {
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
        }
        SetButtons();
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

        SetButtons();
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
                if (e.FinalView.HorizontalOffset > _tokenViewScroller.ScrollableWidth -1)
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

    private void SetButtons()
    {
        if (_tokenViewScrollForwardButton != null)
        {
            if (_tokenViewScroller != null)
            {
                if (_tokenViewScroller.ScrollableWidth > 0)
                {
                    _tokenViewScrollForwardButton.Visibility = Visibility.Visible;
                }
                else
                {
                    _tokenViewScrollForwardButton.Visibility = Visibility.Collapsed;
                }
            }
        }
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

    private Token? GetCurrentContainerItem()
    {
        if (ControlHelpers.IsXamlRootAvailable && XamlRoot != null)
        {
            return FocusManager.GetFocusedElement(XamlRoot) as Token;
        }
        else
        {
            return FocusManager.GetFocusedElement() as Token;
        }
    }
}
