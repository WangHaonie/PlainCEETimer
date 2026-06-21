using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Extensions;

namespace PlainCEETimer.Interop;

internal static class Win32UI
{
    private static List<IntPtr> UnmanagedWindows;

    [DllImport(App.Gdi32Dll)]
    public static extern int SetBkMode(IntPtr hdc, int mode);

    [DllImport(App.Gdi32Dll)]
    public static extern bool DeleteObject(IntPtr hObject);

    [DllImport(App.Gdi32Dll)]
    public static extern COLORREF SetBkColor(IntPtr hdc, COLORREF color);

    [DllImport(App.Gdi32Dll)]
    public static extern COLORREF SetTextColor(IntPtr hdc, COLORREF color);

    [DllImport(App.Gdi32Dll)]
    public static extern IntPtr CreateSolidBrush(COLORREF color);

    [DllImport(App.Gdi32Dll)]
    public static extern int GetDeviceCaps(IntPtr hdc, int index);

    [DllImport(App.User32Dll)]
    public static extern void MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

    [DllImport(App.User32Dll)]
    public static extern bool EnumChildWindows(IntPtr hWndParent, EnumChildProc lpEnumFunc, IntPtr lParam);

    [DllImport(App.User32Dll)]
    public static extern bool RedrawWindow(IntPtr hWnd, IntPtr lprcUpdate, IntPtr hrgnUpdate, uint flags);

    [DllImport(App.User32Dll)]
    public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

    [DllImport(App.User32Dll)]
    public static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);

    [DllImport(App.User32Dll)]
    public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    [DllImport(App.User32Dll, CharSet = CharSet.Unicode)]
    public static extern bool SetWindowText(IntPtr hWnd, string lpString);

    [DllImport(App.User32Dll)]
    public static extern IntPtr GetDlgItem(IntPtr hDlg, int nIDDlgItem);

    [DllImport(App.User32Dll)]
    public static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);

    [DllImport(App.User32Dll)]
    public static extern bool DestroyIcon(HICON hIcon);

    [DllImport(App.User32Dll)]
    public static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, Keys vk);

    [DllImport(App.User32Dll)]
    public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

    [DllImport(App.User32Dll)]
    public static extern int FillRect(IntPtr hDC, ref RECT lprc, IntPtr hbr);

    [DllImport(App.User32Dll)]
    public static extern bool IsMenu(IntPtr hMenu);

    [DllImport(App.User32Dll)]
    public static extern bool TrackPopupMenuEx(IntPtr hMenu, int uFlags, int x, int y, IntPtr hwnd, IntPtr lptpm);

    [DllImport(App.User32Dll)]
    public static extern IntPtr GetForegroundWindow();

    [DllImport(App.User32Dll)]
    public static extern bool IsWindowVisible(IntPtr hWnd);

    [DllImport(App.User32Dll)]
    public static extern IntPtr GetAncestor(IntPtr hWnd, uint gaFlags);

    [DllImport(App.User32Dll)]
    public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

    [DllImport(App.User32Dll)]
    public static extern IntPtr GetShellWindow();

    [DllImport(App.User32Dll)]
    public static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

    [DllImport(App.User32Dll)]
    public static extern bool EnableMenuItem(IntPtr hMenu, int uIDEnableItem, int uEnable);

    [DllImport(App.User32Dll)]
    public static extern int GetMenuItemCount(IntPtr hMenu);

    [DllImport(App.User32Dll)]
    public static extern bool CheckMenuRadioItem(IntPtr hmenu, int first, int last, int check, int flags);

    [DllImport(App.User32Dll, CharSet = CharSet.Unicode)]
    public static extern bool InsertMenu(IntPtr hMenu, int uPosition, int uFlags, long uIDNewItem, string lpNewItem);

    [DllImport(App.User32Dll)]
    public static extern uint GetDpiForWindow(IntPtr hWnd);

    [DllImport(App.User32Dll)]
    public static extern int GetSystemMetricsForDpi(int nIndex, uint dpi);

    [DllImport(App.User32Dll)]
    public static extern int GetSystemMetrics(int nIndex);

    [DllImport(App.User32Dll)]
    public static extern IntPtr GetThreadDpiAwarenessContext();

    [DllImport(App.User32Dll)]
    public static extern IntPtr SetThreadDpiAwarenessContext(DpiAwarenessContext dpiContext);

    [DllImport(App.User32Dll)]
    public static extern bool AreDpiAwarenessContextsEqual(DpiAwarenessContext dpiContextA, IntPtr dpiContextB);

    [DllImport(App.User32Dll)]
    public static extern IntPtr GetDC(IntPtr hWnd);

    [DllImport(App.User32Dll)]
    public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

    [DllImport(App.User32Dll)]
    public static extern bool IsProcessDPIAware();

    [DllImport(App.User32Dll)]
    public static extern bool SetProcessDPIAware();

    [DllImport(App.ShcoreDll)]
    public static extern HRESULT GetProcessDpiAwareness(IntPtr hProcess, out int value);

    [DllImport(App.ShcoreDll)]
    public static extern HRESULT SetProcessDpiAwareness(int value);

    [DllImport(App.ShcoreDll)]
    public static extern HRESULT GetDpiForMonitor(IntPtr hmonitor, int dpiType, out uint dpiX, out uint dpiY);

    [DllImport(App.UxThemeDll, CharSet = CharSet.Unicode)]
    public static extern HRESULT SetWindowTheme(IntPtr hWnd, string pszSubAppName, string pszSubIdList);

    /*

    提取 DLL 里的图标参考:

    How can I use the images within shell32.dll in my C# project? - Stack Overflow
    https://stackoverflow.com/a/6873026/21094697

    */

    [DllImport(App.Shell32Dll, CharSet = CharSet.Unicode)]
    public unsafe static extern uint ExtractIconEx(string lpszFile, int nIconIndex, out HICON phiconLarge, HICON* phiconSmall, int nIcons);

    [DllImport(App.NativesDll, EntryPoint = "#21")]
    public unsafe static extern bool RunColorDialog(IntPtr hWndOwner, WNDPROC lpfnHookProc, COLORREF* lpColor, COLORREF* lpCustomColors);

    [DllImport(App.NativesDll, EntryPoint = "#22")]
    public static extern bool RunFontDialog(IntPtr hWndOwner, WNDPROC lpfnHookProc, ref LOGFONT lpLogFont, int nSizeLimit);

    [DllImport(App.NativesDll, EntryPoint = "#23")]
    public static extern void SetRoundCorner(IntPtr hWnd, int width, int height, int radius);

    [DllImport(App.NativesDll, EntryPoint = "#24")]
    public static extern void SetRoundCornerEx(IntPtr hWnd, bool smallCorner);

    [DllImport(App.NativesDll, EntryPoint = "#25")]
    public static extern void EnableDarkModeForApp(bool enabled);

    [DllImport(App.NativesDll, EntryPoint = "#26")]
    public static extern void EnableDarkModeForWindowFrame(IntPtr hWnd, bool after20h1, bool enabled);

    [DllImport(App.NativesDll, EntryPoint = "#27")]
    public static extern void SetBorderColor(IntPtr hWnd, COLORREF color, bool enabled);

    [DllImport(App.NativesDll, EntryPoint = "#28")]
    public static extern int GetSystemAccentColor();

    [DllImport(App.NativesDll, EntryPoint = "#29")]
    public static extern void ListViewSelectAllItems(IntPtr hLV, bool selected);

    [DllImport(App.NativesDll, EntryPoint = "#30")]
    public static extern void SetTopMostWindow(IntPtr hWnd);

    [DllImport(App.NativesDll, EntryPoint = "#31")]
    public static extern bool MenuGetItemCheckState(IntPtr hMenu, int item, bool fByPosition);

    [DllImport(App.NativesDll, EntryPoint = "#32")]
    public static extern bool MenuUncheckItem(IntPtr hMenu, int item, bool fByPosition);

    [DllImport(App.NativesDll, EntryPoint = "#33", CharSet = CharSet.Unicode)]
    public static extern string GetWindowText(IntPtr hWnd);

    [DllImport(App.NativesDll, EntryPoint = "#34", CharSet = CharSet.Unicode)]
    public static extern IntPtr GetClassName(IntPtr hWnd);

    [DllImport(App.NativesDll, EntryPoint = "#35")]
    public static extern void ComctlHookSysColor(COLORREF crFore, COLORREF crBack);

    [DllImport(App.NativesDll, EntryPoint = "#36")]
    public static extern void ComctlUnhookSysColor();

    [DllImport(App.NativesDll, EntryPoint = "#37")]
    public static extern void RemoveWindowExStyle(IntPtr hWnd, long dwExStyle);

    [DllImport(App.NativesDll, EntryPoint = "#46")]
    public static extern bool CheckWindowExStyle(IntPtr hWnd, long dwExStyle);

    [DllImport(App.NativesDll, EntryPoint = "#38")]
    public static extern void ComdlgHookMessageBox(HOOKPROC lpfnCbtProc, FnMessageBoxW lpfnMessageBoxW, int dwHookFlag);

    [DllImport(App.NativesDll, EntryPoint = "#39")]
    public static extern void ComdlgUnhookMessageBox();

    [DllImport(App.NativesDll, EntryPoint = "#40")]
    public static extern bool IsDialog(IntPtr lpCreateStruct);

    [DllImport(App.NativesDll, EntryPoint = "#43")]
    public static extern void RemoveWindowIcon(IntPtr hWnd);

    [DllImport(App.NativesDll, EntryPoint = "#44")]
    public static extern void HookGetMessage(WHGETMESSAGE lpfnGetMsgProc, int dwThreadId);

    [DllImport(App.NativesDll, EntryPoint = "#45")]
    public static extern void UnhookGetMessage();

    public static void MakeCenter(Rectangle target, Rectangle parent, out Rectangle targetNew)
    {
        var screen = Screen.GetWorkingArea(target);
        var w = target.Width;
        var h = target.Height;
        var x = parent.Left + (parent.Width / 2) - (w / 2);
        var y = parent.Top + (parent.Height / 2) - (h / 2);
        x = x.Clamp(screen.X, screen.Right - w);
        y = y.Clamp(screen.Y, screen.Bottom - h);
        targetNew = new(x, y, w, h);
    }

    public static void RegisterUnmanagedWindow(IntPtr hWnd)
    {
        (UnmanagedWindows ??= []).Add(hWnd);
    }

    public static void UnregisterUnmanagedWindow(IntPtr hWnd)
    {
        UnmanagedWindows?.Remove(hWnd);
    }

    public static void ActivateUnmanagedWindows()
    {
        if (UnmanagedWindows != null)
        {
            for (int i = 0; i < UnmanagedWindows.Count; i++)
            {
                var hWnd = UnmanagedWindows[i];

                if (GetWindowRect(hWnd, out var rc))
                {
                    Rectangle rcWnd = rc;
                    var rcScreen = Screen.GetWorkingArea(rcWnd);
                    var x = rcWnd.X.Clamp(rcScreen.X, rcScreen.Right - rcWnd.Width);
                    var y = rcWnd.Y.Clamp(rcScreen.Y, rcScreen.Bottom - rcWnd.Height);
                    MoveWindow(hWnd, x, y, rcWnd.Width, rcWnd.Height, false);
                }
            }
        }
    }
}
