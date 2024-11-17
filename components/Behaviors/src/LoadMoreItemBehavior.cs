#region Copyright

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#endregion

#if WINAPPSDK

using System.Collections;
using System.Collections.Specialized;
using Microsoft.Xaml.Interactivity;

namespace CommunityToolkit.WinUI.Controls;

/// <summary>
/// A behavior that makes <see cref="ItemsView"/> support <see cref="ISupportIncrementalLoading"/>.
/// </summary>
public class LoadMoreItemBehavior : Behavior<ItemsView>
{
    /// <summary>
    /// Identifies the <see cref="LoadingOffset"/> property.
    /// </summary>
    public static readonly DependencyProperty LoadingOffsetProperty = DependencyProperty.Register(
        nameof(LoadingOffset),
        typeof(double),
        typeof(LoadMoreItemBehavior),
        new PropertyMetadata(100d));

    /// <summary>
    /// Gets or sets Distance of <see cref="ItemsView.ScrollView"/> content from scrolling to bottom.
    /// </summary>
    public double LoadingOffset
    {
        get => (double)this.GetValue(LoadingOffsetProperty);
        set => this.SetValue(LoadingOffsetProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="IsActive"/> property.
    /// </summary>
    public static readonly DependencyProperty IsActiveProperty = DependencyProperty.Register(
        nameof(IsActive),
        typeof(bool),
        typeof(LoadMoreItemBehavior),
        new PropertyMetadata(true));

    /// <summary>
    /// Gets a value indicating whether the behavior is active.
    /// </summary>
    public bool IsActive
    {
        get => (bool)this.GetValue(IsActiveProperty);
        set => this.SetValue(IsActiveProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="IsLoadingMore"/> property.
    /// </summary>
    public static readonly DependencyProperty IsLoadingMoreProperty = DependencyProperty.Register(
        nameof(IsLoadingMore),
        typeof(bool),
        typeof(LoadMoreItemBehavior),
        new PropertyMetadata(false));

    /// <summary>
    /// Gets or sets if more items are being loaded.
    /// </summary>
    public bool IsLoadingMore
    {
        get => (bool)this.GetValue(IsLoadingMoreProperty);
        set => this.SetValue(IsLoadingMoreProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="LoadCount"/> property.
    /// </summary>
    public static readonly DependencyProperty LoadCountProperty = DependencyProperty.Register(
        nameof(LoadCount),
        typeof(int),
        typeof(LoadMoreItemBehavior),
        new PropertyMetadata(20));

    /// <summary>
    /// Gets or sets the "count" parameter when triggering <see cref="ISupportIncrementalLoading.LoadMoreItemsAsync"/>.
    /// </summary>
    public int LoadCount
    {
        get => (int)this.GetValue(LoadCountProperty);
        set => this.SetValue(LoadCountProperty, value);
    }

    /// <summary>
    /// Raised when more items need to be loaded.
    /// </summary>
    public event Func<ItemsView, EventArgs, Task<bool>>? LoadMoreRequested;

    private MUXC.ItemsRepeater? ItemsRepeater => (MUXC.ItemsRepeater?)this.ScrollView?.Content;

    private ScrollView? ScrollView => this.AssociatedObject.ScrollView;

    private long _scrollViewOnPropertyChangedToken;
    private long _itemsSourceOnPropertyChangedToken;

    private INotifyCollectionChanged? _lastObservableCollection;

    /// <inheritdoc/>
    protected override void OnAttached()
    {
        this.LoadMoreRequested += async (sender, args) =>
        {
            if (sender.ItemsSource is ISupportIncrementalLoading sil)
            {
                _ = await sil.LoadMoreItemsAsync((uint)this.LoadCount);
                return sil.HasMoreItems;
            }

            return false;
        };
        this._scrollViewOnPropertyChangedToken = this.AssociatedObject.RegisterPropertyChangedCallback(ItemsView.ScrollViewProperty, this.ScrollViewOnPropertyChanged);
        this._itemsSourceOnPropertyChangedToken = this.RegisterPropertyChangedCallback(ItemsView.ItemsSourceProperty, this.ItemsSourceOnPropertyChanged);
    }

    /// <inheritdoc/>
    protected override void OnDetaching()
    {
        this.AssociatedObject.UnregisterPropertyChangedCallback(ItemsView.ScrollViewProperty, this._scrollViewOnPropertyChangedToken);
        this.AssociatedObject.UnregisterPropertyChangedCallback(ItemsView.ItemsSourceProperty, this._itemsSourceOnPropertyChangedToken);
        if (this._lastObservableCollection is not null)
            this._lastObservableCollection.CollectionChanged -= this.TryRaiseLoadMoreRequested;
        if (this.ItemsRepeater is not null)
            this.ItemsRepeater.SizeChanged -= this.TryRaiseLoadMoreRequested;
        if (this.ScrollView is not null)
            this.ScrollView.ViewChanged -= this.TryRaiseLoadMoreRequested;
    }

    /// <summary>
    /// When the data source changes or <see cref="NotifyCollectionChangedAction.Reset"/>, <see cref="NotifyCollectionChangedAction.Remove"/>.
    /// This method reloads the data.
    /// This method is intended to solve the problem of reloading data when the data source changes and the <see cref="ItemsRepeater"/>'s <see cref="FrameworkElement.ActualHeight"/> does not change
    /// </summary>
    private async void ItemsSourceOnPropertyChanged(DependencyObject sender, DependencyProperty dp)
    {
        if (sender is ItemsView { ItemsSource: ISupportIncrementalLoading sil })
        {
            if (sil is INotifyCollectionChanged ncc)
            {
                if (this._lastObservableCollection is not null)
                    this._lastObservableCollection.CollectionChanged -= this.TryRaiseLoadMoreRequested;

                this._lastObservableCollection = ncc;
                ncc.CollectionChanged += this.TryRaiseLoadMoreRequested;
            }

            // On the first load, the `ScrollView` is not yet initialized and can be given to `AdvancedItemsView_OnSizeChanged` to trigger.
            if (this.ScrollView is not null)
                await this.TryRaiseLoadMoreRequestedAsync();
        }
    }

    private async void TryRaiseLoadMoreRequested(object? sender, object e) => await this.TryRaiseLoadMoreRequestedAsync();

    /// <summary>
    /// After this method, the <see cref="TryRaiseLoadMoreRequestedAsync"/> will be triggered
    /// </summary>
    /// <remarks>
    /// <see cref="ItemsRepeater.SizeChanged"/> is the key to continuous loading.
    /// When new data is loaded, it makes the <see cref="Microsoft.UI.Xaml.Controls.ItemsRepeater"/> of the <see cref="FrameworkElement.ActualHeight"/> larger,
    /// Or when the <see cref="ItemsView.ItemsSource"/> is changed to a different source or set for the first time, <see cref="FrameworkElement.ActualHeight"/> becomes 0.
    /// This method can reload the data.
    /// </remarks>
    private void ScrollViewOnPropertyChanged(DependencyObject sender, DependencyProperty dp)
    {
        this.AssociatedObject.UnregisterPropertyChangedCallback(ItemsView.ScrollViewProperty, this._scrollViewOnPropertyChangedToken);
        if (this.ScrollView is null)
            return;
        this.ScrollView.ViewChanged += this.TryRaiseLoadMoreRequested;
        if (this.ItemsRepeater is not null)
            this.ItemsRepeater.SizeChanged += this.TryRaiseLoadMoreRequested;
    }

    /// <summary>
    /// Determines if the scroll view has scrolled to the bottom, and if so triggers the <see cref="LoadMoreRequested"/>.
    /// This event will only cause the source to load at most once
    /// </summary>
    public async Task TryRaiseLoadMoreRequestedAsync()
    {
        if (this.AssociatedObject is null || this.ScrollView is null)
            return;

        var loadMore = true;
        // Load until a new item is loaded in
        while (loadMore)
        {
            if (!this.IsActive || this.IsLoadingMore)
                return;

            // LoadMoreRequested is only triggered when the view is not filled.
            if ((this.ScrollView.ScrollableHeight is 0 && this.ScrollView.ScrollableWidth is 0) ||
                (this.ScrollView.ScrollableHeight > 0 &&
                 this.ScrollView.ScrollableHeight - this.LoadingOffset < this.ScrollView.VerticalOffset) ||
                (this.ScrollView.ScrollableWidth > 0 &&
                 this.ScrollView.ScrollableWidth - this.LoadingOffset < this.ScrollView.HorizontalOffset))
            {
                this.IsLoadingMore = true;
                var before = this.GetItemsCount();
                if (this.LoadMoreRequested is not null && await this.LoadMoreRequested(this.AssociatedObject, EventArgs.Empty))
                {
                    var after = this.GetItemsCount();
                    // This can be set to the count of items in a row,
                    // so that it can continue to load even if the count of items loaded is too small.
                    // Generally, 20 items will be loaded at a time,
                    // and the count of items in a row is usually less than 10, so it is set to 10 here.
                    if (before + 10 <= after)
                        loadMore = false;
                }
                // No more items or ItemsSource is null
                else
                    loadMore = false;

                this.IsLoadingMore = false;
            }
            else
            {
                // There is no need to continue loading if it fills up the view
                loadMore = false;
            }
        }
    }

    private int GetItemsCount()
    {
        return this.AssociatedObject.ItemsSource switch
        {
            ICollection list => list.Count,
            IEnumerable enumerable => enumerable.Cast<object>().Count(),
            null => 0,
            _ => throw new ArgumentOutOfRangeException(nameof(this.AssociatedObject.ItemsSource))
        };
    }
}

#endif
