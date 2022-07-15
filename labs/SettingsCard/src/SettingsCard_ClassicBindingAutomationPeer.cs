// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;

namespace CommunityToolkit.Labs.WinUI;
public partial class SettingsCard_ClassicBindingAutomationPeer : FrameworkElementAutomationPeer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SettingsCard_ClassicBindingAutomationPeer"/> class.
    /// </summary>
    /// <param name="owner">
    /// The <see cref="SettingsCard" /> that is associated with this <see cref="T:Windows.UI.Xaml.Automation.Peers.SettingsCardAutomationPeer" />.
    /// </param>
    public SettingsCard_ClassicBindingAutomationPeer(SettingsCard_ClassicBinding owner)
        : base(owner)
    {
    }

    private SettingsCard_ClassicBinding? OwnerSettingsCard_ClassicBinding
    {
        get { return this.Owner as SettingsCard_ClassicBinding; }
    }

    /// <summary>
    /// Gets the control type for the element that is associated with the UI Automation peer.
    /// </summary>
    /// <returns>The control type.</returns>
    protected override AutomationControlType GetAutomationControlTypeCore()
    {
        return AutomationControlType.Button;
    }

    /// <summary>
    /// Called by GetClassName that gets a human readable name that, in addition to AutomationControlType,
    /// differentiates the control represented by this AutomationPeer.
    /// </summary>
    /// <returns>The string that contains the name.</returns>
    protected override string GetClassNameCore()
    {
        return Owner.GetType().Name;
    }

    /// <summary>
    /// Called by GetName.
    /// </summary>
    /// <returns>
    /// Returns the first of these that is not null or empty:
    /// - Value returned by the base implementation
    /// - Name of the owning BladeItem
    /// - BladeItem class name
    /// </returns>
    protected override string GetNameCore()
    {
        string name = AutomationProperties.GetName(this.OwnerSettingsCard_ClassicBinding);
        if (!string.IsNullOrEmpty(name))
        {
            return name;
        }

        return string.Empty;
    }
}
