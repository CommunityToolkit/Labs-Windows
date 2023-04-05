// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.Labs.WinUI;

public partial class TokenView : ListViewBase
{
    // Temporary tracking of previous collections for removing events.
    private MethodInfo? _removeItemsSourceMethod;
    protected override void OnItemsChanged(object e)
    {
        IVectorChangedEventArgs args = (IVectorChangedEventArgs)e;

        base.OnItemsChanged(e);
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
