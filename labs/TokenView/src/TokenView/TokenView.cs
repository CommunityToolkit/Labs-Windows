// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Windows.System;

namespace CommunityToolkit.Labs.WinUI;

/// <summary>
/// This is an controĺ easily visualize tokens, to create filtering experiences.
/// </summary>

[TemplatePart(Name = TabsScrollViewerName, Type = typeof(ScrollViewer))]
[TemplatePart(Name = TabsScrollBackButtonName, Type = typeof(ButtonBase))]
[TemplatePart(Name = TabsScrollForwardButtonName, Type = typeof(ButtonBase))]
public partial class TokenView : ListViewBase
{
    /// <summary>
    /// Occurs when a tab's Close button is clicked.  Set <see cref="TokenViewItemClosingEventArgs.Cancel"/> to true to prevent automatic Tab Closure.
    /// </summary>
    ///

    private const string? TabsScrollViewerName = "ScrollViewer";
    private const string? TabsScrollBackButtonName = "ScrollBackButton";
    private const string? TabsScrollForwardButtonName = "ScrollForwardButton";
    public event EventHandler<TokenViewItemClosingEventArgs>? TokenViewItemClosing;

    private ScrollViewer? _tabScroller;
    private ButtonBase? _tabScrollBackButton;
    private ButtonBase? _tabScrollForwardButton;

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
        if (_tabScroller is not null)
        {
            if (this.Orientation == FilterOrientation.Horizontal)
            {
                _tabScroller.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
            }
            else if (this.Orientation == FilterOrientation.Wrapped)
            {
                _tabScroller.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
            }
        }
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        PreviewKeyDown -= LiteFilter_PreviewKeyDown;
        PreviewKeyDown += LiteFilter_PreviewKeyDown;

        this.SizeChanged += LiteFilter_SizeChanged;
        if (_tabScroller != null)
        {
            _tabScroller.Loaded -= ScrollViewer_Loaded;
        }

        _tabScroller = GetTemplateChild(TabsScrollViewerName) as ScrollViewer;

        if (_tabScroller != null)
        {
            _tabScroller.Loaded += ScrollViewer_Loaded;
        }

        OnOrientationChanged();
    }

    private void LiteFilter_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        SetButtons();
    }

    protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
    {
        base.PrepareContainerForItemOverride(element, item);

        if (element is TokenViewItem tokenViewItem)
        {
            tokenViewItem.Loaded += TokenViewItem_Loaded;
            tokenViewItem.Closing += TokenViewItem_Closing;

            if (tokenViewItem.IsClosable != true && tokenViewItem.ReadLocalValue(TokenViewItem.IsClosableProperty) == DependencyProperty.UnsetValue)
            {
                var iscloseablebinding = new Binding()
                {
                    Source = this,
                    Path = new PropertyPath(nameof(CanRemoveTokens)),
                    Mode = BindingMode.OneWay,
                };
                tokenViewItem.SetBinding(TokenViewItem.IsClosableProperty, iscloseablebinding);
            }
        }
    }

    private void LiteFilter_PreviewKeyDown(object sender, KeyRoutedEventArgs e)
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
        if (currentContainerItem != null && currentContainerItem.IsClosable)
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
    private void TokenViewItem_Loaded(object sender, RoutedEventArgs e)
    {
        var tokenViewItem = sender as TokenViewItem;

        if (tokenViewItem != null)
        {
            tokenViewItem.Loaded -= TokenViewItem_Loaded;
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

    private void TokenViewItem_Closing(object? sender, TokenViewItemClosingEventArgs e)
    {
        var item = ItemFromContainer(e.TokenViewItem);

        var args = new TokenViewItemClosingEventArgs(item, e.TokenViewItem);
        TokenViewItemClosing?.Invoke(this, args);

        if (!args.Cancel)
        {
            if (ItemsSource != null)
            {
                _removeItemsSourceMethod?.Invoke(ItemsSource, new object[] { item });
            }
            else
            {
                if (_tabScroller != null)
                {
                    _tabScroller.UpdateLayout();
                }
                Items.Remove(item);
            }
        }
    }

    private void ScrollViewer_Loaded(object sender, RoutedEventArgs e)
    {
        if (_tabScrollBackButton is not null)
        {
            _tabScrollBackButton.Click -= ScrollTabBackButton_Click;
        }

        if (_tabScrollForwardButton is not null)
        {
            _tabScrollForwardButton.Click -= ScrollTabForwardButton_Click;
        }

        if (_tabScroller is not null)
        {
            _tabScroller.Loaded -= ScrollViewer_Loaded;
            _tabScroller.ViewChanging += _tabScroller_ViewChanging;

            _tabScrollBackButton = _tabScroller.FindDescendantByName(TabsScrollBackButtonName) as ButtonBase;
            _tabScrollForwardButton = _tabScroller.FindDescendantByName(TabsScrollForwardButtonName) as ButtonBase;
        }

       
    
       
        if (_tabScrollBackButton is not null)
        {
            _tabScrollBackButton.Click += ScrollTabBackButton_Click;
        }

        if (_tabScrollForwardButton is not null)
        {
            _tabScrollForwardButton.Click += ScrollTabForwardButton_Click;
        }


        SetButtons();
    }


    private void _tabScroller_ViewChanging(object? sender, ScrollViewerViewChangingEventArgs e)
    {
        if (_tabScrollBackButton != null)
        {
            if (e.FinalView.HorizontalOffset < 1)
            {
                _tabScrollBackButton.Visibility = Visibility.Collapsed;
            }
            else if (e.FinalView.HorizontalOffset > 1)
            {
                _tabScrollBackButton.Visibility = Visibility.Visible;
            }
        }

        if (_tabScrollForwardButton != null)
        {
            if (_tabScroller != null)
            {
                if (e.FinalView.HorizontalOffset > _tabScroller.ScrollableWidth - 1)
                {
                    _tabScrollForwardButton.Visibility = Visibility.Collapsed;
                }
                else if (e.FinalView.HorizontalOffset < _tabScroller.ScrollableWidth - 1)
                {
                    _tabScrollForwardButton.Visibility = Visibility.Visible;
                }
            }
        }
    }

    private void SetButtons()
    {
        if (_tabScrollForwardButton != null)
        {
            if (_tabScroller != null)
            {
                if (_tabScroller.ScrollableWidth > 0)
                {
                    _tabScrollForwardButton.Visibility = Visibility.Visible;
                }
                else
                {
                    _tabScrollForwardButton.Visibility = Visibility.Collapsed;
                }
            }
        }
    }

    private void ScrollTabBackButton_Click(object sender, RoutedEventArgs e)
    {
        if (_tabScroller != null)
        {
            _tabScroller.ChangeView(_tabScroller.HorizontalOffset - _tabScroller.ViewportWidth, null, null);
        }
    }
    private void ScrollTabForwardButton_Click(object sender, RoutedEventArgs e)
    {
        // this.UpdateLayout();
        if (_tabScroller != null)
        {
            _tabScroller.ChangeView(_tabScroller.HorizontalOffset + _tabScroller.ViewportWidth, null, null);
        }
    }

    private enum MoveDirection
    {
        Next,
        Previous
    }

    private bool MoveFocus(MoveDirection direction)
    {
        bool retVal = false;
        var currentContainerItem = GetCurrentContainerItem();

        if (currentContainerItem != null)
        {
            var currentItem = ItemFromContainer(currentContainerItem);
            var previousIndex = Items.IndexOf(currentItem);
            var index = previousIndex;

            if (direction == MoveDirection.Previous)
            {
                if (previousIndex > 0)
                {
                    index -= 1;
                }
                else
                {
                    retVal = true;
                }
            }
            else if (direction == MoveDirection.Next)
            {
                if (previousIndex < Items.Count - 1)
                {
                    index += 1;
                }
            }

            // Only do stuff if the index is actually changing
            if (index != previousIndex)
            {
                var newItem = ContainerFromIndex(index) as TokenViewItem;
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                newItem.Focus(FocusState.Keyboard);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                retVal = true;
            }
        }

        return retVal;
    }

    private TokenViewItem? GetCurrentContainerItem()
    {
        // TO DO - IS THIS ACTUALLY NEEDED?
        if (ControlHelpers.IsXamlRootAvailable && XamlRoot != null)
        {
            return FocusManager.GetFocusedElement(XamlRoot) as TokenViewItem;
        }
        else
        {
            return FocusManager.GetFocusedElement() as TokenViewItem;
        }
    }
}

