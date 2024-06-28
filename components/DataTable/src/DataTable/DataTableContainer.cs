// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.Versioning;

namespace CommunityToolkit.WinUI.Controls;

[SupportedOSPlatform("osx10.14")]
[SupportedOSPlatform("maccatalyst13.1")]
public partial class DataTableContainer : Grid
{
#pragma warning disable CA2213
    private Panel? _header;
#pragma warning restore CA2213
    public Panel? Header
    {
        get
        {
            if (_header != null)
            {
                return _header;
            }

            if (this.FindDescendant<Panel>(static (element) => element is Grid or DataTable) is Panel panel)
            {
                _header = panel;
                return _header;
            }

            return null;
        }
    }
}
