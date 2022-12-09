// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Windows.UI.Input.Preview.Injection;

#if WINAPPSDK
using MainWindow = Microsoft.UI.Xaml.Window;
#else
using MainWindow = Windows.UI.Xaml.Window;
#endif

namespace CommunityToolkit.Labs.Tests;

public partial class InputSimulator
{
    private static InputInjector _input = InputInjector.TryCreate();
    private static uint _currentPointerId = 0;

    private WeakReference<MainWindow> _currentWindowRef;

    /// <summary>
    /// Create a new <see cref="InputSimulator"/> helper class for the current window. All positions provided to this API will use the client space of the provided window's top-left as an origin point.
    /// </summary>
    /// <param name="currentWindow">Window to simulate input on, used as a reference for client-space coordinates.</param>
    public InputSimulator(MainWindow currentWindow)
    {
        _currentWindowRef = new(currentWindow);
    }
}
