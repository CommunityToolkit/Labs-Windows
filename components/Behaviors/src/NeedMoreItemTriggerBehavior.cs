#region Copyright

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#endregion

#if WINAPPSDK

using Microsoft.Xaml.Interactivity;

namespace CommunityToolkit.WinUI.Controls;

/// <summary>
/// A behavior that triggers actions when the <see cref="ItemsView"/> is scrolled to the bottom.
/// </summary>
[TypeConstraint(typeof(ItemsView))]
public class NeedMoreItemTriggerBehavior : Trigger<ItemsView>
{
    /// <summary>
    /// Identifies the <see cref="LoadingOffset"/> property.
    /// </summary>
    public static readonly DependencyProperty LoadingOffsetProperty = DependencyProperty.Register(
        nameof(LoadingOffset),
        typeof(double),
        typeof(NeedMoreItemTriggerBehavior),
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
        typeof(NeedMoreItemTriggerBehavior),
        new PropertyMetadata(true));

    /// <summary>
    /// Gets a value indicating whether the trigger is active.
    /// </summary>
    public bool IsActive
    {
        get => (bool)this.GetValue(IsActiveProperty);
        set => this.SetValue(IsActiveProperty, value);
    }

    private ItemsRepeater? ItemsRepeater => (ItemsRepeater?)this.ScrollView?.Content;

    private ScrollView? ScrollView => this.AssociatedObject.ScrollView;

    private long _scrollViewOnPropertyChangedToken;

    /// <inheritdoc/>
    protected override void OnAttached()
    {
        this._scrollViewOnPropertyChangedToken = this.AssociatedObject.RegisterPropertyChangedCallback(ItemsView.ScrollViewProperty, this.ScrollViewOnPropertyChanged);
    }

    /// <inheritdoc/>
    protected override void OnDetaching()
    {
        this.AssociatedObject.UnregisterPropertyChangedCallback(ItemsView.ScrollViewProperty, this._scrollViewOnPropertyChangedToken);
        if (this.ItemsRepeater is not null)
            this.ItemsRepeater.SizeChanged -= this.TryRaiseLoadMoreRequested;
        if (this.ScrollView is not null)
            this.ScrollView.ViewChanged -= TryRaiseLoadMoreRequested;
    }

    private void TryRaiseLoadMoreRequested(object? sender, object e) => this.TryRaiseLoadMoreRequested();

    /// <summary>
    /// After this method, the <see cref="TryRaiseLoadMoreRequested"/> will be triggered
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
        this.ScrollView.ViewChanged += TryRaiseLoadMoreRequested;
        if (this.ItemsRepeater is not null)
            this.ItemsRepeater.SizeChanged += this.TryRaiseLoadMoreRequested;
    }

    /// <summary>
    /// Determines if the scroll view has scrolled to the bottom, and if so triggers the <see cref="Trigger.Actions"/>.
    /// This event will only cause the source to load at most once
    /// </summary>
    public void TryRaiseLoadMoreRequested()
    {
        if (this.ScrollView is null || !this.IsActive)
            return;
            
        // Is only triggered when the view is not filled.
        if ((this.ScrollView.ScrollableHeight is 0 && this.ScrollView.ScrollableWidth is 0) ||
            (this.ScrollView.ScrollableHeight > 0 &&
             this.ScrollView.ScrollableHeight - this.LoadingOffset < this.ScrollView.VerticalOffset) ||
            (this.ScrollView.ScrollableWidth > 0 &&
             this.ScrollView.ScrollableWidth - this.LoadingOffset < this.ScrollView.HorizontalOffset))
            Interaction.ExecuteActions(this.AssociatedObject, this.Actions, null);
    }
}

#endif
