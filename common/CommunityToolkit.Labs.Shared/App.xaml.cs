using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;

#if !WINAPPSDK
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
#else
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
#endif

namespace CommunityToolkit.Labs.Shared
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public sealed partial class App : Application
    {
        // MacOS and iOS don't know the correct type without a full namespace declaration, confusing it with NSWindow and UIWindow.
        // Using static will not work.
#if WINAPPSDK
        private static Microsoft.UI.Xaml.Window currentWindow = Microsoft.UI.Xaml.Window.Current;
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
            Frame? rootFrame = null;

#if WINAPPSDK
            var window = new Window();
#else
            rootFrame = currentWindow.Content as Frame;
#endif

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

#if WINAPPSDK
                window.Content = rootFrame;
#else
                // Place the frame in the current Window
                currentWindow.Content = rootFrame;
#endif
            }


#if WINAPPSDK
                rootFrame.Navigate(typeof(AppLoadingView), e.Arguments);
                window.Activate();
#else
            if (e.PrelaunchActivated == false)
            {
                if (rootFrame is null)
                    throw new InvalidOperationException("Cannot display app content, root frame is missing.");

                if (rootFrame.Content == null)
                {
                    rootFrame.Navigate(typeof(AppLoadingView), e.Arguments);
                }

                // Ensure the current window is active
                currentWindow.Activate();
            }
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
}
