// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace AdornersExperiment.Samples;

[ToolkitSampleBoolOption("IsNotificationVisible", true, Title = "Is Notification Visible")]

[ToolkitSample(id: nameof(InfoBadgeWithoutAdorner), "InfoBadge w/o Adorner", description: "A sample for showing how one adds an infobadge to a component without an Adorner (from WinUI Gallery app).")]
public sealed partial class InfoBadgeWithoutAdorner : Page
{
    public InfoBadgeWithoutAdorner()
    {
        this.InitializeComponent();
    }
}
