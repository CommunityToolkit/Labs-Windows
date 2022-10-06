// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.Labs.WinUI;

/// <summary>
/// <see cref="StyleSelector"/> used by <see cref=" SettingsExpanderItem"/> to choose the proper <see cref=" SettingsExpanderItem"/> container style (clickable or not).
/// </summary>
public class SettingsExpanderItemStyleSelector : StyleSelector
{
    /// <summary>
    /// Gets or sets the <see cref="Style"/> of a token item.
    /// </summary>
    public Style DefaultStyle { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="Style"/> of a text entry item.
    /// </summary>
    public Style ClickableStyle { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SettingsExpanderItemStyleSelector"/> class.
    /// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public SettingsExpanderItemStyleSelector()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {
    }

    /// <inheritdoc/>
    protected override Style SelectStyleCore(object item, DependencyObject container)
    {
        if (((SettingsExpanderItem)item).IsClickEnabled)
        {
            return ClickableStyle;
        }
        else
        {
            return DefaultStyle;
        }
    }
}
