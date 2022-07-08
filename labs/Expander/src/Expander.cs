// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.Labs.WinUI;

public class Expander : MUXC.Expander
{
    public Expander()
    {
        this.DefaultStyleKey = typeof(Expander);
#if !HAS_UNO
        // Workaround for https://github.com/microsoft/microsoft-ui-xaml/issues/3502
        this.DefaultStyleResourceUri = new Uri("ms-appx:///CommunityToolkit.Labs.WinUI.Expander/Themes/Generic.xaml");
#endif
    }
}
