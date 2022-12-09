// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if WINAPPSDK
using Windows.Win32;
using Windows.Win32.Foundation;
using Win32Rect = Windows.Win32.Foundation.RECT;
using Win32Point = System.Drawing.Point;
#endif

namespace CommunityToolkit.Labs.Tests;

//// This polyfill is needed as the WindowsAppSDK doesn't provide client based coordinates. See Issue: TODO: File bug currentWindow.Bounds should be the same between the two platforms. Should AppWindow also have Bounds?

public partial class InputSimulator
{
#if WINAPPSDK    
    private Rect Bounds
    {
        get
        {
            if (_currentWindowRef.TryGetTarget(out var currentWindow))
            {
                var hWnd = (HWND)WinRT.Interop.WindowNative.GetWindowHandle(currentWindow);

                // Get client area position
                Win32Point[] points = new Win32Point[1];
                PInvoke.MapWindowPoints(hWnd, HWND.Null, points);

                // TODO: Check LastError?

                // And size
                if (points.Length == 1 && PInvoke.GetClientRect(hWnd, out Win32Rect size))
                {
                    return new Rect(points[0].X, points[0].Y, size.right - size.left, size.bottom - size.top);
                }
            }

            return default;
        }
    }
#else
    private Rect Bounds => _currentWindowRef.TryGetTarget(out Window window) ? window.Bounds : default;
#endif

    private Point TranslatePointForWindow(Point point)
    {
        // TODO: Do we want a ToPoint extension in the Toolkit? (is there an existing enum we can use to specify which corner/point of the rect? e.g. topleft, center, middleright, etc...?

        // Get the top left screen coordinates of the app window rect.
        var bounds = Bounds; // Don't double-retrieve the calculated property, TODO: Just make some point helpers (or use ones in Toolkit)
        Point appBoundsTopLeft = new Point(bounds.Left, bounds.Top);

        // Create the point for input injection and calculate its screen location.
        return new Point(
                appBoundsTopLeft.X + point.X,
                appBoundsTopLeft.Y + point.Y);

    }
}
