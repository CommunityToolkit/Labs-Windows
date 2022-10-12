// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if WINAPPSDK
    using ToolkitCST = CommunityToolkit.WinUI.UI.Triggers.ControlSizeTrigger;
#else
    using ToolkitCST = Microsoft.Toolkit.Uwp.UI.Triggers.ControlSizeTrigger;
#endif

namespace CommunityToolkit.Labs.WinUI;
/// <summary>
/// Provide an abstraction around the Toolkit ControlSizeTrigger for both UWP and WinUI 3 in the same namespace (until 8.0).
/// </summary>
public partial class ControlSizeTrigger : ToolkitCST
{
}
