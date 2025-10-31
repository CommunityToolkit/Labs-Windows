// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if WINAPPSDK
using System.Runtime.InteropServices;

namespace CommunityToolkit.WinUI.Controls;
internal static class NativeMethods
{
    public enum WindowMessage : int
    {
        WM_NCLBUTTONDOWN = 0x00A1,
        WM_NCRBUTTONDOWN = 0x00A4,
        WM_SYSCOMMAND = 0x0112,
        WM_SYSMENU = 0x0313,
        WM_GETMINMAXINFO = 0x0024
    }
    [Flags]
    public enum WindowStyle : uint
    {
        WS_SYSMENU = 0x80000,
    }

    [Flags]
    public enum WindowStyleExtended : ulong
    {
        WS_EX_LAYOUTRTL= 0x00400000L,
    }

    [Flags]
    public enum WindowLongIndexFlags : int
    {
        GWL_WNDPROC = -4,
        GWL_STYLE = -16,
        GWL_EXSTYLE = -20,
    }

    [Flags]
    public enum SetWindowPosFlags : uint
    {
        /// <summary>
        ///     Retains the current position (ignores X and Y parameters).
        /// </summary>
        SWP_NOMOVE = 0x0002
    }

    public enum SystemCommand
    {
        SC_MOUSEMENU = 0xF090,
        SC_KEYMENU = 0xF100
    }

    // TODO: Check for typing online. IntPtr, int, or long?

    [DllImport("user32.dll", EntryPoint = "GetWindowLongW", SetLastError = false)]
    public static extern int GetWindowLongW(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr", SetLastError = false)]
    public static extern IntPtr GetWindowLongPtr(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll", EntryPoint = "GetWindowLongPtrW", SetLastError = false)]
    public static extern int GetWindowLongPtrW(IntPtr hWnd, int nIndex);

    public static int GetWindowLongAuto(IntPtr hWnd, int nIndex)
    {
        if (IntPtr.Size is 8)
        {
            return GetWindowLongPtrW(hWnd, nIndex);
        }
        else
        {
            return GetWindowLongW(hWnd, nIndex);
        }
    }

    [DllImport("user32.dll", EntryPoint = "FindWindowExW", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern IntPtr FindWindowEx(IntPtr hWndParent, IntPtr hWndChildAfter, string lpszClass, string lpszWindow);

    [DllImport("user32.dll", EntryPoint = "SetWindowLongW", SetLastError = false)]
    public static extern IntPtr SetWindowLongW(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

    [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr", SetLastError = false)]
    public static extern IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

    [DllImport("user32.dll", EntryPoint = "SetWindowLongPtrW", SetLastError = false)]
    public static extern IntPtr SetWindowLongPtrW(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

    public static IntPtr SetWindowLongAuto(IntPtr hWnd, int nIndex, IntPtr dwNewLong)
    {
        if (IntPtr.Size is 8)
        {
            return SetWindowLongPtrW(hWnd, nIndex, dwNewLong);
        }
        else
        {
            return SetWindowLongW(hWnd, nIndex, dwNewLong);
        }
    }

    [DllImport("user32.dll")]
    public static extern IntPtr CallWindowProc(IntPtr lpPrevWndFunc, IntPtr hWnd, WindowMessage Msg, IntPtr wParam, IntPtr lParam);
}
#endif
