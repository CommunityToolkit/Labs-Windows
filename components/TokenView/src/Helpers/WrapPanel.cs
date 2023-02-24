// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if WINAPPSDK
    using ToolkitWP = CommunityToolkit.WinUI.UI.Controls.WrapPanel;
#else
    using ToolkitWP = Microsoft.Toolkit.Uwp.UI.Controls.WrapPanel;
#endif

namespace CommunityToolkit.Labs.WinUI;
/// <summary>
/// Provide an abstraction around the Toolkit WrapPanel for both UWP and WinUI 3 in the same namespace (until 8.0).
/// </summary>
public partial class WrapPanel : ToolkitWP
{
}
