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
        WS_SYSMENU = 0x80000
    }

    [Flags]
    public enum WindowLongIndexFlags : int
    {
        GWL_WNDPROC = -4,
        GWL_STYLE = -16
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

    [DllImport("user32.dll", EntryPoint = "GetWindowLongW", SetLastError = false)]
    public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll", EntryPoint = "GetWindowLongPtrW", SetLastError = false)]
    public static extern int GetWindowLongPtr(IntPtr hWnd, int nIndex);

    public static int GetWindowLongAuto(IntPtr hWnd, int nIndex)
    {
        if (IntPtr.Size is 8)
        {
            return GetWindowLongPtr(hWnd, nIndex);
        }
        else
        {
            return GetWindowLong(hWnd, nIndex);
        }
    }

    [DllImport("user32.dll", EntryPoint = "FindWindowExW", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern IntPtr FindWindowEx(IntPtr hWndParent, IntPtr hWndChildAfter, string lpszClass, string lpszWindow);


    [DllImport("user32.dll", EntryPoint = "SetWindowLongW", SetLastError = false)]
    public static extern IntPtr SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

    [DllImport("user32.dll", EntryPoint = "SetWindowLongPtrW", SetLastError = false)]
    public static extern IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

    public static IntPtr SetWindowLongAuto(IntPtr hWnd, int nIndex, IntPtr dwNewLong)
    {
        if (IntPtr.Size is 8)
        {
            return SetWindowLongPtr(hWnd, nIndex, dwNewLong);
        }
        else
        {
            return SetWindowLong(hWnd, nIndex, dwNewLong);
        }
    }

    [DllImport("user32.dll")]
    public static extern IntPtr CallWindowProc(IntPtr lpPrevWndFunc, IntPtr hWnd, WindowMessage Msg, IntPtr wParam, IntPtr lParam);
}
