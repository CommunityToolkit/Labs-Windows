// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.App.Shared;

/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public sealed partial class App : Application
{
    // MacOS and iOS don't know the correct type without a full namespace declaration, confusing it with NSWindow and UIWindow.
    // Using static will not work.
#if WINAPPSDK
    public static Microsoft.UI.Xaml.Window currentWindow = Microsoft.UI.Xaml.Window.Current;
#else
    private static Windows.UI.Xaml.Window currentWindow = Windows.UI.Xaml.Window.Current;
#endif

    /// <summary>
    /// Initializes the singleton application object.  This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App()
    {
        this.InitializeComponent();
    }

    /// <summary>
    /// Invoked when the application is launched normally by the end user.  Other entry points
    /// will be used such as when the application is launched to open a specific file.
    /// </summary>
    /// <param name="e">Details about the launch request and process.</param>
    protected override void OnLaunched(LaunchActivatedEventArgs e)
    {
#if WINAPPSDK
        currentWindow = new Window();
#endif

        // Do not repeat app initialization when the Window already has content,
        // just ensure that the window is active
        if (currentWindow.Content is not Frame rootFrame)
        {
            // Create a Frame to act as the navigation context and navigate to the first page
            currentWindow.Content = rootFrame = new Frame();

            rootFrame.NavigationFailed += OnNavigationFailed;
        }

#if !WINAPPSDK
        if (e.PrelaunchActivated == false)
#endif
            rootFrame.Navigate(typeof(AppLoadingView), e.Arguments);

        SetTitleBar();
        // Ensure the current window is active
        currentWindow.Activate();
    }

    /// <summary>
    /// Invoked when Navigation to a certain page fails
    /// </summary>
    /// <param name="sender">The Frame which failed navigation</param>
    /// <param name="e">Details about the navigation failure</param>
    void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
    {
        throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
    }

    private void SetTitleBar()
    {
#if WINDOWS_UWP
        var viewTitleBar = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().TitleBar;
        viewTitleBar.ButtonBackgroundColor = Windows.UI.Colors.Transparent;
        viewTitleBar.ButtonInactiveBackgroundColor = Windows.UI.Colors.Transparent;
#endif
    }
}
