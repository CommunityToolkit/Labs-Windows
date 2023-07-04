// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace TitleBarExperiment.Samples;
/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class BlankPage1 : Page
{
#if WINAPPSDK
    public BlankPage1(Window window)
    {
        this.InitializeComponent();
        appTitleBar.Window = window;
    }
#else
    public BlankPage1()
    {
        this.InitializeComponent();
        Microsoft.UI.Xaml.Controls.BackdropMaterial.SetApplyToRootOrPageBackground(this, true);
    }
#endif
}
