// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if WINAPPSDK
using MainWindow = Microsoft.UI.Xaml.Window;
#else
using MainWindow = Windows.UI.Xaml.Window;
#endif

namespace CommunityToolkit.Labs.Tests;

public static class InputHelpers
{
    /// <summary>
    /// Helper extension method to create a new <see cref="InputSimulator"/> chain for the current window.
    /// </summary>
    /// <param name="window"><see cref="Window"/> class for your application.</param>
    /// <returns>A new <see cref="InputSimulator"/> instance for that window.</returns>
    public static InputSimulator InjectInput(this MainWindow window)
    {
        return new InputSimulator(window);
    }

    public static Point CoordinatesToCenter(this UIElement parent, UIElement target)
    {
        var location = target.TransformToVisual(parent).TransformPoint(default(Point));
        return new(location.X + target.ActualSize.X / 2, location.Y + target.ActualSize.Y / 2);
    }
}
