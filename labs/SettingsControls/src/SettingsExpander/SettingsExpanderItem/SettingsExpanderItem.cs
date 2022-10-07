// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.Labs.WinUI;

public partial class SettingsExpanderItem : SettingsCard
{
    /// <summary>
    /// Creates a new instance of the <see cref="SettingsExpanderItem"/> class.
    /// </summary>
    public SettingsExpanderItem()
    {
        this.DefaultStyleKey = typeof(SettingsExpanderItem);
    }

    /// <inheritdoc />
    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
    }
}
