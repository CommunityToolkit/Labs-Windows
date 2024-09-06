// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if WINAPPSDK
using System.Runtime.InteropServices;
using WinRT.Interop;

namespace CommunityToolkit.WinUI.Controls;
internal class WndProcHelper
{
    public delegate IntPtr WNDPROC(IntPtr hWnd, NativeMethods.WindowMessage Msg, IntPtr wParam, IntPtr lParam);

    private IntPtr Handle { get; set; }
    private WNDPROC? newMainWindowWndProc = null;
    private IntPtr oldMainWindowWndProc = IntPtr.Zero;

    private WNDPROC? newInputNonClientPointerSourceWndProc = null;
    private IntPtr oldInputNonClientPointerSourceWndProc = IntPtr.Zero;

    public WndProcHelper(Window window)
    {
        Handle = WindowNative.GetWindowHandle(window);
    }

    public IntPtr CallWindowProc(IntPtr hWnd, NativeMethods.WindowMessage Msg, IntPtr wParam, IntPtr lParam)
    {
        return NativeMethods.CallWindowProc(oldMainWindowWndProc, hWnd, Msg, wParam, lParam);
    }

    public IntPtr CallInputNonClientPointerSourceWindowProc(IntPtr hWnd, NativeMethods.WindowMessage Msg, IntPtr wParam, IntPtr lParam)
    {
        return NativeMethods.CallWindowProc(oldInputNonClientPointerSourceWndProc, hWnd, Msg, wParam, lParam);
    }
    public void RegisterWndProc(WNDPROC wndProc)
    {
        newMainWindowWndProc = wndProc;
        oldMainWindowWndProc = NativeMethods.SetWindowLongAuto(Handle, (int)NativeMethods.WindowLongIndexFlags.GWL_WNDPROC, Marshal.GetFunctionPointerForDelegate(newMainWindowWndProc));
    }

    public void RegisterInputNonClientPointerSourceWndProc(WNDPROC wndProc)
    {
        IntPtr inputNonClientPointerSourceHandle = NativeMethods.FindWindowEx(Handle, IntPtr.Zero, "InputNonClientPointerSource", string.Empty);

        if (inputNonClientPointerSourceHandle != IntPtr.Zero)
        {
            int style = NativeMethods.GetWindowLongAuto(Handle, (int)NativeMethods.WindowLongIndexFlags.GWL_STYLE);
            NativeMethods.SetWindowLongAuto(Handle, (int)NativeMethods.WindowLongIndexFlags.GWL_STYLE, (IntPtr)(style & ~(int)NativeMethods.WindowStyle.WS_SYSMENU));

            newInputNonClientPointerSourceWndProc = wndProc;
            oldInputNonClientPointerSourceWndProc = NativeMethods.SetWindowLongAuto(inputNonClientPointerSourceHandle, (int)NativeMethods.WindowLongIndexFlags.GWL_WNDPROC, Marshal.GetFunctionPointerForDelegate(newInputNonClientPointerSourceWndProc));
        }
    }
}
#endif
