// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.Labs.WinUI;
public partial class TokenView : ListViewBase
{
    private enum MoveDirection
    {
        Next,
        Previous
    }
    private bool MoveFocus(MoveDirection direction)
    {
        bool retVal = false;

        if (ItemFromContainer(GetCurrentContainerItem()) is DependencyObject currentItem)
        {
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
                if (ContainerFromIndex(index) is TokenItem newItem)
                {
                    newItem.Focus(FocusState.Keyboard);
                }
                retVal = true;
            }
        }
        return retVal;
    }
}
