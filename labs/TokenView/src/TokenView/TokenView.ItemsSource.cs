// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.UI.Xaml.Controls;

namespace CommunityToolkit.Labs.WinUI;
public partial class TokenView : ListViewBase
{
    // Temporary tracking of previous collections for removing events.
    private MethodInfo? _removeItemsSourceMethod;
    protected override void OnItemsChanged(object e)
    {
        IVectorChangedEventArgs args = (IVectorChangedEventArgs)e;

        base.OnItemsChanged(e);

        //if (args.CollectionChange == CollectionChange.ItemRemoved && SelectedIndex == -1)
        //{
        //    // If we remove the selected item we should select the previous item
        //    int startIndex = (int)args.Index + 1;
        //    if (startIndex > Items.Count)
        //    {
        //        startIndex = 0;
        //    }

        //    SelectedIndex = FindNextTabIndex(startIndex, -1);
        //}

        //// Update Sizing (in case there are less items now)
        //TabView_SizeChanged(this, null);
    }

    private void ItemContainerGenerator_ItemsChanged(object sender, ItemsChangedEventArgs e)
    {
        var action = (CollectionChange)e.Action;
        if (action == CollectionChange.Reset)
        {
            // Reset collection to reload later.
            _hasLoaded = false;
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


    private void ItemsSource_PropertyChanged(DependencyObject sender, DependencyProperty dp)
    {

        // Use reflection to store a 'Remove' method of any possible collection in ItemsSource
        // Cache for efficiency later.
        if (ItemsSource != null)
        {
            _removeItemsSourceMethod = ItemsSource.GetType().GetMethod("Remove");
        }
        else
        {

            _removeItemsSourceMethod = null;
        }
    }
}

