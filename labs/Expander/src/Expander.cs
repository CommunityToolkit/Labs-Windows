// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using Windows.Foundation;

// TODO: remove any unneeded namespaces before creating a PR
#if !WINAPPSDK
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
#else
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Media;
#endif
using MUXC = Microsoft.UI.Xaml.Controls;

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
