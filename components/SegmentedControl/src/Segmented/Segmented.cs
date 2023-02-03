// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Windows.System;

namespace CommunityToolkit.Labs.WinUI;

public partial class Segmented : ListViewBase
{
    public Segmented()
    {
        this.DefaultStyleKey = typeof(Segmented);

        RegisterPropertyChangedCallback(Segmented.SelectionModeProperty, OnSelectionModeChanged);
#if !HAS_UNO
        ItemContainerGenerator.ItemsChanged += ItemContainerGenerator_ItemsChanged;
#endif
    }

    #if !HAS_UNO
    private void ItemContainerGenerator_ItemsChanged(object sender, ItemsChangedEventArgs e)
    {
        var action = (CollectionChange)e.Action;
        if (action == CollectionChange.Reset)
        {
            // Reset collection to reload later.
            _hasLoaded = false;
        }
    }
#endif

    protected override DependencyObject GetContainerForItemOverride() => new SegmentedItem();

    protected override bool IsItemItsOwnContainerOverride(object item)
    {
        return item is SegmentedItem;
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        PreviewKeyDown -= Segmented_PreviewKeyDown;
        PreviewKeyDown += Segmented_PreviewKeyDown;
    }

    protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
    {
        base.PrepareContainerForItemOverride(element, item);
        var SegmentedItem = element as SegmentedItem;
#pragma warning disable CS8602 // Dereference of a possibly null reference.
        SegmentedItem.Loaded += SegmentedItem_Loaded;
#pragma warning restore CS8602 // Dereference of a possibly null reference.

    }

    private void Segmented_PreviewKeyDown(object sender, KeyRoutedEventArgs e)
    {
        switch (e.Key)
        {
            case VirtualKey.Left: e.Handled = MoveFocus(MoveDirection.Previous); break;
            case VirtualKey.Right: e.Handled = MoveFocus(MoveDirection.Next); break;
        }
    }

    private bool _hasLoaded;
    private void SegmentedItem_Loaded(object sender, RoutedEventArgs e)
    {
        var SegmentedItem = sender as SegmentedItem;
#pragma warning disable CS8602 // Dereference of a possibly null reference.
        SegmentedItem.Loaded -= SegmentedItem_Loaded;
#pragma warning restore CS8602 // Dereference of a possibly null reference.

        //// Only need to do this once.
        if (!_hasLoaded)
        {
            _hasLoaded = true;
            // Need to set a the selection on load, otherwise ListView resets to null.
            SetInitialSelection();
        }
    }

    protected override void OnItemsChanged(object e)
    {
        IVectorChangedEventArgs args = (IVectorChangedEventArgs)e;
        base.OnItemsChanged(e);
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

            //  Otherwise, select the first item by default
            else if (Items.Count >= 1 && SelectionMode == ListViewSelectionMode.Single)
            {
                SelectedItem = Items[0];
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

    private enum MoveDirection
    {
        Next,
        Previous
    }

    /// <summary>
    /// Adjust the selected item and range based on keyboard input.
    /// This is used to override the ListView behaviors for up/down arrow manipulation vs left/right for a horizontal control
    /// </summary>
    /// <param name="direction">direction to move the selection</param>
    /// <returns>True if the focus was moved, false otherwise</returns>
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
                var newItem = ContainerFromIndex(index) as SegmentedItem;
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                newItem.Focus(FocusState.Keyboard);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                retVal = true;
            }
        }

        return retVal;
    }

    private SegmentedItem GetCurrentContainerItem()
    {
        if (ControlHelpers.IsXamlRootAvailable && XamlRoot != null)
        {
#pragma warning disable CS8603 // Possible null reference return.
            return FocusManager.GetFocusedElement(XamlRoot) as SegmentedItem;
#pragma warning restore CS8603 // Possible null reference return.
        }
        else
        {
#pragma warning disable CS8603 // Possible null reference return.
            return FocusManager.GetFocusedElement() as SegmentedItem;
#pragma warning restore CS8603 // Possible null reference return.
        }
    }
    private void OnSelectionModeChanged(DependencyObject sender, DependencyProperty dp)
    {
        SetInitialSelection();
    }
}
