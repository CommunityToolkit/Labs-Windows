using System;
using System.Collections.Generic;
using System.Text;
using Windows.Storage;

namespace CommunityToolkit.Labs.Shared.Helpers
{
    public static class ThemeHelper
    {
        public static void InitializeTheme(Window currentWindow)
        {
            int? selectedTheme = ApplicationData.Current.LocalSettings.Values["Theme"] as int?;
            if (selectedTheme is not null && currentWindow.Content is FrameworkElement rootElement)
            {
                switch (selectedTheme)
                {
                    case 0: return;
                    case 1: rootElement.RequestedTheme = ElementTheme.Light; break;
                    case 2: rootElement.RequestedTheme = ElementTheme.Dark; break;
                }
            }
            else
            {
                return;
            }

            IntPtr hWnd = WindowNative.GetWindowHandle(this);
            WindowId wndId = Win32Interop.GetWindowIdFromWindow(hWnd);
            return AppWindow.GetFromWindowId(wndId);

            var titleBar = currentWindow.TitleBar;
            titleBar.ForegroundColor = Colors.White;
            titleBar.BackgroundColor = Colors.Green;
        }
    }
}
