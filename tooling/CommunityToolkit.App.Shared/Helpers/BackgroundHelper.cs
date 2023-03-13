// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.Tooling.SampleGen;
using Microsoft.UI.Xaml.Controls;

namespace CommunityToolkit.App.Shared.Helpers;

public static class BackgroundHelper
{
    public static void SetBackground(Page page)
    {
#if !WINAPPSDK
        BackdropMaterial.SetApplyToRootOrPageBackground(page, true);
#else
        // TO DO: SET MICA THE WINAPPSDK WAY, FALLING BACK TO DEFAULT BACKGROUND FOR NOW
        page.Background = (SolidColorBrush)Application.Current.Resources["BackgroundColorBrush"];
#endif
    }
}

