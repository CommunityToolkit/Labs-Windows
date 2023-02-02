// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if WINAPPSDK
using ToolkitFWE = CommunityToolkit.WinUI.UI.FrameworkElementExtensions;
#else
using ToolkitFWE = Microsoft.Toolkit.Uwp.UI.FrameworkElementExtensions;
#endif

namespace CommunityToolkit.Labs.WinUI;
/// <summary>
/// Provide an abstraction around the Toolkit FrameworkElementExtensions for both UWP and WinUI 3 in the same namespace (until 8.0).
/// </summary>
public partial class FrameworkElementExtensions : ToolkitFWE
{
}
