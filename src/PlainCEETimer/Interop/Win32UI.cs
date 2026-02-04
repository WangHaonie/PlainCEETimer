using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Extensions;

namespace PlainCEETimer.Interop;

public static class Win32UI
{
    private static List<IntPtr> UnmanagedWindows;

    [DllImport(App.User32Dll)]
    public static extern void MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

    [DllImport(App.Gdi32Dll)]
    public static extern int SetBkMode(IntPtr hdc, int mode);

    [DllImport(App.Gdi32Dll)]
    public static extern bool DeleteObject(IntPtr hObject);

    [DllImport(App.User32Dll)]
    public static extern bool EnumChildWindows(IntPtr hWndParent, EnumChildProc lpEnumFunc, IntPtr lParam);

    [DllImport(App.User32Dll)]
    public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

    [DllImport(App.User32Dll)]
    public static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);

    [DllImport(App.User32Dll, CharSet = CharSet.Unicode)]
    public static extern bool SetWindowText(IntPtr hWnd, string lpString);

    [DllImport(App.Gdi32Dll)]
    public static extern COLORREF SetBkColor(IntPtr hdc, COLORREF color);

    [DllImport(App.Gdi32Dll)]
    public static extern COLORREF SetTextColor(IntPtr hdc, COLORREF color);

    [DllImport(App.Gdi32Dll)]
    public static extern IntPtr CreateSolidBrush(COLORREF color);

    [DllImport(App.User32Dll)]
    public static extern IntPtr GetDlgItem(IntPtr hDlg, int nIDDlgItem);

    [DllImport(App.User32Dll)]
    public static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);

    /*

    提取 DLL 里的图标参考:

    How can I use the images within shell32.dll in my C# project? - Stack Overflow
    https://stackoverflow.com/a/6873026/21094697

    */

    [DllImport(App.Shell32Dll, CharSet = CharSet.Unicode)]
    public unsafe static extern int ExtractIconEx(string lpszFile, int nIconIndex, out HICON phiconLarge, HICON* phiconSmall, int nIcons);

    [DllImport(App.User32Dll)]
    public static extern bool DestroyIcon(HICON hIcon);

    [DllImport(App.UxThemeDll, CharSet = CharSet.Unicode)]
    public static extern int SetWindowTheme(IntPtr hWnd, string pszSubAppName, string pszSubIdList);

    [DllImport(App.User32Dll)]
    public static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, Keys vk);

    [DllImport(App.User32Dll)]
    public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

    [DllImport(App.User32Dll)]
    public static extern int FillRect(IntPtr hDC, ref RECT lprc, IntPtr hbr);

    [DllImport(App.User32Dll)]
    public static extern bool IsMenu(IntPtr hMenu);

    [DllImport(App.User32Dll)]
    public static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

    [DllImport(App.User32Dll)]
    public static extern int GetMenuItemCount(IntPtr hMenu);

    [DllImport(App.User32Dll)]
    public static extern bool CheckMenuRadioItem(IntPtr hmenu, int first, int last, int check, MenuFlag flags);

    [DllImport(App.User32Dll, CharSet = CharSet.Unicode)]
    public static extern bool InsertMenu(IntPtr hMenu, int uPosition, MenuFlag uFlags, long uIDNewItem, string lpNewItem);

    [DllImport(App.NativesDll, EntryPoint = "#21")]
    public static extern bool RunColorDialog(IntPtr hWndOwner, WNDPROC lpfnHookProc, ref COLORREF lpColor, LPCUSTCOLORS lpCustomColors);

    [DllImport(App.NativesDll, EntryPoint = "#22")]
    public static extern bool RunFontDialog(IntPtr hWndOwner, WNDPROC lpfnHookProc, ref LOGFONT lpLogFont, int nSizeLimit);

    [DllImport(App.NativesDll, EntryPoint = "#23")]
    public static extern void SetRoundCorner(IntPtr hWnd, int width, int height, int radius);

    [DllImport(App.NativesDll, EntryPoint = "#24")]
    public static extern void SetRoundCornerEx(IntPtr hWnd, bool smallCorner);

    [DllImport(App.NativesDll, EntryPoint = "#25")]
    public static extern void EnableDarkModeForApp();

    [DllImport(App.NativesDll, EntryPoint = "#26")]
    public static extern void EnableDarkModeForWindowFrame(IntPtr hWnd, bool after20h1);

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
    public static extern string GetClassName(IntPtr hWnd);

    [DllImport(App.NativesDll, EntryPoint = "#35")]
    public static extern void ComctlHookSysColor(COLORREF crFore, COLORREF crBack);

    [DllImport(App.NativesDll, EntryPoint = "#36")]
    public static extern void ComctlUnhookSysColor();

    [DllImport(App.NativesDll, EntryPoint = "#37")]
    public static extern void RemoveWindowExStyle(IntPtr hWnd, long dwExStyle);

    [DllImport(App.NativesDll, EntryPoint = "#38")]
    public static extern void ComdlgHookMessageBox(HOOKPROC lpfnCbtHookProc);

    [DllImport(App.NativesDll, EntryPoint = "#39")]
    public static extern void ComdlgUnhookMessageBox();

    [DllImport(App.NativesDll, EntryPoint = "#40")]
    public static extern bool IsDialog(IntPtr lpCreateStruct);

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
