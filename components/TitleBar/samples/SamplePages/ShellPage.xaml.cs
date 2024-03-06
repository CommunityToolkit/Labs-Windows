// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace TitleBarExperiment.Samples;
/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class ShellPage : Page
{
#if WINAPPSDK
    public ShellPage(Window window)
    {
        this.InitializeComponent();
        appTitleBar.Window = window;
    }
#else
    public ShellPage()
    {
        this.InitializeComponent();
        Microsoft.UI.Xaml.Controls.BackdropMaterial.SetApplyToRootOrPageBackground(this, true);
    }
#endif
    private void appTitleBar_BackButtonClick(object sender, RoutedEventArgs e)
    {
        if (NavFrame.CanGoBack)
        {
            NavFrame.GoBack();
        }
    }

    private void appTitleBar_PaneButtonClick(object sender, RoutedEventArgs e)
    {
        NavView.IsPaneOpen = !NavView.IsPaneOpen;
    }

    private void NavView_SelectionChanged(MUXC.NavigationView sender, MUXC.NavigationViewSelectionChangedEventArgs args)
    {
        switch ((string)((MUXC.NavigationViewItem)NavView.SelectedItem).Tag)
        {
            case "1": NavFrame.Navigate(typeof(FirstPage)); break;
            case "2": NavFrame.Navigate(typeof(SecondPage)); break;
        }
  
    }

    private void NavView_Loaded(object sender, RoutedEventArgs e)
    {
        NavView.SelectedItem = NavView.MenuItems[0];
    }

    private void NavFrame_Navigated(object sender, NavigationEventArgs e)
    {
        appTitleBar.IsBackButtonVisible = NavFrame.CanGoBack;
    }
}
