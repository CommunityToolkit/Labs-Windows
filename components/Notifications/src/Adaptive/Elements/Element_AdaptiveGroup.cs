// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace CommunityToolkit.Notifications.Adaptive.Elements;

internal sealed class Element_AdaptiveGroup : IElement_TileBindingChild, IElement_ToastBindingChild, IElementWithDescendants, IHaveXmlName, IHaveXmlChildren
{
    public IList<Element_AdaptiveSubgroup> Children { get; private set; } = new List<Element_AdaptiveSubgroup>();

    public IEnumerable<object> Descendants()
    {
        foreach (Element_AdaptiveSubgroup subgroup in Children)
        {
            // Return the subgroup
            yield return subgroup;

            // And also return its descendants
            foreach (object descendant in subgroup.Descendants())
            {
                yield return descendant;
            }
        }
    }

    /// <inheritdoc/>
    string IHaveXmlName.Name => "group";

    /// <inheritdoc/>
    IEnumerable<object> IHaveXmlChildren.Children => Children;
}