// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if !WINAPPSDK
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.System;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
#else
using Microsoft.UI.Dispatching;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.VisualStudio.TestTools.UnitTesting.AppContainer;
#endif

namespace CommunityToolkit.Labs.Tests;

/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public sealed partial class App : Application
{
    // MacOS and iOS don't know the correct type without a full namespace declaration, confusing it with NSWindow and UIWindow.
    // Using static will not work.
#if WINAPPSDK
    private static Microsoft.UI.Xaml.Window currentWindow = Microsoft.UI.Xaml.Window.Current;

    private static AppWindow GetAppWindow(Microsoft.UI.Xaml.Window window)
    {
        // From: https://docs.microsoft.com/en-us/windows/apps/windows-app-sdk/windowing/windowing-overview#code-example
        // Retrieve the window handle (HWND) of the current (XAML) WinUI 3 window.
        var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(window);

        // Retrieve the WindowId that corresponds to hWnd.
        var windowId = Win32Interop.GetWindowIdFromWindow(hWnd);

        // Lastly, retrieve the AppWindow for the current (XAML) WinUI 3 window.
        return AppWindow.GetFromWindowId(windowId);
    }

    public static Rect Bounds { get
        {
            var appWindow = GetAppWindow(currentWindow);
            var position = appWindow.Position;
            var size = appWindow.Size;
            return new Rect(position.X, position.Y, size.Width, size.Height);
        }
    }
#else
    private static Windows.UI.Xaml.Window currentWindow = Windows.UI.Xaml.Window.Current;

    public static Rect Bounds => currentWindow.Bounds;
#endif

    // Holder for test content to abstract Window.Current.Content
    public static FrameworkElement? ContentRoot
    {
        get => currentWindow.Content as FrameworkElement;
        set => currentWindow.Content = value;
    }

    // Abstract CoreApplication.MainView.DispatcherQueue
    public static DispatcherQueue DispatcherQueue
    {
        get
        {
#if !WINAPPSDK
            return CoreApplication.MainView.DispatcherQueue;
#else
            return currentWindow.DispatcherQueue;
#endif
        }
    }

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

        ////Microsoft.VisualStudio.TestPlatform.TestExecutor.UnitTestClient.CreateDefaultUI();

        // Ensure the current window is active
        currentWindow.Activate();

#if !WINAPPSDK
        Microsoft.VisualStudio.TestPlatform.TestExecutor.UnitTestClient.Run(e.Arguments);
#else
        UITestMethodAttribute.DispatcherQueue = DispatcherQueue;

        // Replace back with e.Arguments when https://github.com/microsoft/microsoft-ui-xaml/issues/3368 is fixed
        Microsoft.VisualStudio.TestPlatform.TestExecutor.UnitTestClient.Run(Environment.CommandLine);
#endif
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
}
