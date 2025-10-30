using System;
using System.Runtime.InteropServices;
using PlainCEETimer.Modules;

namespace PlainCEETimer.Interop;

public static class Win32UI
{
    [DllImport(App.User32Dll)]
    public static extern void MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

    [DllImport(App.Gdi32Dll)]
    public static extern int SetBkMode(IntPtr hdc, int mode);

    /*

    提取 DLL 里的图标参考:

    How can I use the images within shell32.dll in my C# project? - Stack Overflow
    https://stackoverflow.com/a/6873026/21094697

    */

    [DllImport(App.Shell32Dll, CharSet = CharSet.Unicode)]
    public static extern int ExtractIconEx(string lpszFile, int nIconIndex, out HICON phiconLarge, HICON phiconSmall, int nIcons);

    [DllImport(App.User32Dll)]
    public static extern bool DestroyIcon(IntPtr hIcon);

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

    [DllImport(App.UxThemeDll, CharSet = CharSet.Unicode)]
    public static extern int SetWindowTheme(IntPtr hWnd, string pszSubAppName, string pszSubIdList);

    [DllImport(App.User32Dll)]
    public static extern int FillRect(IntPtr hDC, ref RECT lprc, IntPtr hbr);

    [DllImport(App.NativesDll, EntryPoint = "#20")]
    public static extern bool RunColorDialog(IntPtr hWndOwner, WNDPROC lpfnHookProc, ref COLORREF lpColor, LPCUSTCOLORS lpCustomColors);

    [DllImport(App.NativesDll, EntryPoint = "#21")]
    public static extern bool RunFontDialog(IntPtr hWndOwner, WNDPROC lpfnHookProc, ref LOGFONT lpLogFont, int nSizeLimit);

    [DllImport(App.NativesDll, EntryPoint = "#22")]
    public static extern void SetRoundCorner(IntPtr hWnd, int width, int height, int radius);

    [DllImport(App.NativesDll, EntryPoint = "#23")]
    public static extern void SetRoundCornerEx(IntPtr hWnd, bool isSmall);

    [DllImport(App.NativesDll, EntryPoint = "#24")]
    public static extern void EnableDarkModeForApp();

    [DllImport(App.NativesDll, EntryPoint = "#25")]
    public static extern void EnableDarkModeForWindowFrame(IntPtr hWnd, bool after20h1);

    [DllImport(App.NativesDll, EntryPoint = "#26")]
    public static extern void SetBorderColor(IntPtr hWnd, COLORREF color, bool enabled);

    [DllImport(App.NativesDll, EntryPoint = "#27")]
    public static extern int GetSystemAccentColor();

    [DllImport(App.NativesDll, EntryPoint = "#28")]
    public static extern void ListViewSelectAllItems(IntPtr hLV, bool selected);

    [DllImport(App.NativesDll, EntryPoint = "#29")]
    public static extern void SetTopMostWindow(IntPtr hWnd);

    [DllImport(App.NativesDll, EntryPoint = "#30")]
    public static extern bool MenuGetItemCheckStateByPosition(IntPtr hMenu, int item);

    [DllImport(App.NativesDll, EntryPoint = "#31")]
    public static extern bool MenuCheckRadioItemByPosition(IntPtr hMenu, int item);

    [DllImport(App.NativesDll, EntryPoint = "#32", CharSet = CharSet.Unicode)]
    public static extern string GetWindowText(IntPtr hWnd);

    [DllImport(App.NativesDll, EntryPoint = "#33", CharSet = CharSet.Unicode)]
    public static extern string GetClassName(IntPtr hWnd);

    [DllImport(App.NativesDll, EntryPoint = "#34")]
    public static extern void CommonHookSysColor(COLORREF crFore, COLORREF crBack);

    [DllImport(App.NativesDll, EntryPoint = "#35")]
    public static extern void CommonUnhookSysColor();

    [DllImport(App.NativesDll, EntryPoint = "#36")]
    public static extern void RemoveWindowExStyles(IntPtr hWnd, long dwExStyles);
}
