// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.WinUI.Controls;
#if WINDOWS_UWP
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.WindowManagement;
using Windows.UI.Xaml.Hosting;
#endif
namespace TitleBarExperiment.Samples;

/// <summary>
/// An example sample page of a custom control inheriting from Panel.
/// </summary>
[ToolkitSampleTextOption("TitleText", "This is a title", Title = "Input the text")]
[ToolkitSampleMultiChoiceOption("LayoutOrientation", "Horizontal", "Vertical", Title = "Orientation")]

[ToolkitSample(id: nameof(TitleBarCustomSample), "Custom control", description: $"A sample for showing how to create and use a {nameof(TitleBar)} custom control.")]
public sealed partial class TitleBarCustomSample : Page
{
    public TitleBarCustomSample()
    {
        this.InitializeComponent();
    }

    // TODO: See https://github.com/CommunityToolkit/Labs-Windows/issues/149
    public static Orientation ConvertStringToOrientation(string orientation) => orientation switch
    {
        "Vertical" => Orientation.Vertical,
        "Horizontal" => Orientation.Horizontal,
        _ => throw new System.NotImplementedException(),
    };

    private async void Button_Click(object sender, RoutedEventArgs e)
    {
#if WINDOWS_UWP
        CoreApplicationView newView = CoreApplication.CreateNewView();
        int newViewId = 0;
        await newView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
        {
            Frame frame = new Frame();
            frame.Navigate(typeof(BlankPage1), null);
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
        newWindow.Content = new BlankPage1(newWindow);
        newWindow.Activate();
#endif
    }
}
