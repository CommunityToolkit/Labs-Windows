// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Windows.UI.Input.Preview.Injection;
using Windows.Foundation;

#if !WINAPPSDK
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
#else
using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
#endif

namespace CommunityToolkit.Labs.UnitTests;

public static class SimulateInput
{
    private static InputInjector _input = InputInjector.TryCreate();
    private static uint _currentPointerId = 0;

    public static void StartTouch()
    {
        Assert.IsNotNull(_input);

        _input.InitializeTouchInjection(
                InjectedInputVisualizationMode.Default);
    }

    /// <summary>
    /// Simulates a touch press on screen at the coordinates provided, in app-local coordinates. For instance use <code>App.ContentRoot.CoordinatesTo(element)</code>.
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    public static uint TouchDown(Point point)
    {
        // Create a unique pointer ID for the injected touch pointer.
        // Multiple input pointers would require more robust handling.
        uint pointerId = _currentPointerId++;

        var injectionPoint = TranslatePointForWindow(point);

        // Create a touch data point for pointer down.
        // Each element in the touch data list represents a single touch contact. 
        // For this example, we're mirroring a single mouse pointer.
        List<InjectedInputTouchInfo> touchData = new()
            {
                new()
                {
                    Contact = new InjectedInputRectangle
                    {
                        Left = 30, Top = 30, Bottom = 30, Right = 30
                    },
                    PointerInfo = new InjectedInputPointerInfo
                    {
                        PointerId = pointerId,
                        PointerOptions =
                        InjectedInputPointerOptions.PointerDown |
                        InjectedInputPointerOptions.InContact |
                        InjectedInputPointerOptions.New,
                        TimeOffsetInMilliseconds = 0,
                        PixelLocation = new InjectedInputPoint
                        {
                            PositionX = (int)injectionPoint.X ,
                            PositionY = (int)injectionPoint.Y
                        }
                },
                Pressure = 1.0,
                TouchParameters =
                    InjectedInputTouchParameters.Pressure |
                    InjectedInputTouchParameters.Contact
                }
            };

        // Inject the touch input. 
        _input.InjectTouchInput(touchData);

        return pointerId;
    }

    public static void TouchMove(uint pointerId, int cX, int cY)
    {
        // Create a touch data point for pointer up.
        List<InjectedInputTouchInfo> touchData = new()
            {
                new()
                {
                    Contact = new InjectedInputRectangle
                    {
                        Left = 30, Top = 30, Bottom = 30, Right = 30
                    },
                    PointerInfo = new InjectedInputPointerInfo
                    {
                        PointerId = pointerId,
                        PointerOptions =
                        InjectedInputPointerOptions.InRange |
                        InjectedInputPointerOptions.InContact,
                        TimeOffsetInMilliseconds = 0,
                        PixelLocation = new InjectedInputPoint
                        {
                            PositionX = (int)cX ,
                            PositionY = (int)cY
                        }
                    },
                    Pressure = 1.0,
                    TouchParameters =
                        InjectedInputTouchParameters.Pressure |
                        InjectedInputTouchParameters.Contact
                }
            };

        // Inject the touch input. 
        _input.InjectTouchInput(touchData);
    }

    public static void TouchUp(uint pointerId)
    {
        Assert.IsNotNull(_input);

        // Create a touch data point for pointer up.
        List<InjectedInputTouchInfo> touchData = new()
        {
            new()
            {
                PointerInfo = new InjectedInputPointerInfo
                {
                    PointerId = pointerId,
                    PointerOptions = InjectedInputPointerOptions.PointerUp
                }
            }
        };

        // Inject the touch input. 
        _input.InjectTouchInput(touchData);
    }

    public static void StopTouch()
    {
        // Shut down the virtual input device.
        _input.UninitializeTouchInjection();
    }

    private static Point TranslatePointForWindow(Point point)
    {
        // TODO: Do we want a ToPoint extension in the Toolkit? (is there an existing enum we can use to specify which corner/point of the rect? e.g. topleft, center, middleright, etc...?

        // Get the top left screen coordinates of the app window rect.
        Point appBoundsTopLeft = new Point(App.Bounds.Left, App.Bounds.Top);

        // Create the point for input injection and calculate its screen location.
        return new Point(
                appBoundsTopLeft.X + point.X,
                appBoundsTopLeft.Y + point.Y);

    }
}
