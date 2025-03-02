// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace CommunityToolkit.Notifications;

internal sealed class Element_ToastSelection : IElement_ToastInputChild, IHaveXmlName, IHaveXmlNamedProperties
{
    /// <summary>
    /// Gets or sets the id attribute for apps to retrieve back the user selected input after the app is activated. Required
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// Gets or sets the text to display for this selection element.
    /// </summary>
    public string Content { get; set; }

    /// <inheritdoc/>
    string IHaveXmlName.Name => "selection";

    /// <inheritdoc/>
    IEnumerable<KeyValuePair<string, object>> IHaveXmlNamedProperties.EnumerateNamedProperties()
    {
        yield return new("id", Id);
        yield return new("content", Content);
    }
}