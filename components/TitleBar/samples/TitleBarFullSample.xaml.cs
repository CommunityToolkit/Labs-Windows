// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.WinUI.Controls;
#if WINDOWS_UWP
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
#endif
namespace TitleBarExperiment.Samples;

[ToolkitSample(id: nameof(TitleBarFullSample), "Full titlebar sample", description: $"A sample for showing how to create and use a {nameof(TitleBar)} in a window.")]
public sealed partial class TitleBarFullSample : Page
{
    public TitleBarFullSample()
    {
        this.InitializeComponent();
    }

    private async void Button_Click(object sender, RoutedEventArgs e)
    {
#if WINDOWS_UWP
        CoreApplicationView newView = CoreApplication.CreateNewView();
        int newViewId = 0;
        await newView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
        {
            Frame frame = new Frame();
            frame.Navigate(typeof(ShellPage), null);
            Window.Current.Content = frame;
            // You have to activate the window in order to show it later.
            Window.Current.Activate();

            newViewId = ApplicationView.GetForCurrentView().Id;
        });

        bool viewShown = await ApplicationViewSwitcher.TryShowAsStandaloneAsync(newViewId);
#endif
#if WINAPPSDK
        Window newWindow = new Window
        {
            SystemBackdrop = new MicaBackdrop()
        };
        newWindow.Content = new ShellPage(newWindow);
        newWindow.Activate();
#endif
    }
}
