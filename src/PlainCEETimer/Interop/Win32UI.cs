using System;
using System.Runtime.InteropServices;
using PlainCEETimer.Modules;

namespace PlainCEETimer.Interop;

public static class Win32UI
{
    [DllImport(App.User32Dll)]
    public static extern void MoveWindow(HWND hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

    [DllImport(App.Gdi32Dll)]
    public static extern int SetBkMode(HDC hdc, int mode);

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
    public static extern bool EnumChildWindows(HWND hWndParent, EnumChildProc lpEnumFunc, IntPtr lParam);

    [DllImport(App.User32Dll)]
    public static extern bool GetWindowRect(HWND hWnd, out RECT lpRect);

    [DllImport(App.User32Dll)]
    public static extern bool GetClientRect(HWND hWnd, out RECT lpRect);

    [DllImport(App.User32Dll, CharSet = CharSet.Unicode)]
    public static extern bool SetWindowText(HWND hWnd, string lpString);

    [DllImport(App.Gdi32Dll)]
    public static extern COLORREF SetBkColor(HDC hdc, COLORREF color);

    [DllImport(App.Gdi32Dll)]
    public static extern COLORREF SetTextColor(HDC hdc, COLORREF color);

    [DllImport(App.Gdi32Dll)]
    public static extern IntPtr CreateSolidBrush(COLORREF color);

    [DllImport(App.User32Dll)]
    public static extern HWND GetDlgItem(HWND hDlg, int nIDDlgItem);

    [DllImport(App.User32Dll)]
    public static extern IntPtr SendMessage(HWND hWnd, int msg, int wParam, int lParam);

    [DllImport(App.UxThemeDll, CharSet = CharSet.Unicode)]
    public static extern int SetWindowTheme(HWND hWnd, string pszSubAppName, string pszSubIdList);

    [DllImport(App.NativesDll, EntryPoint = "#20")]
    public static extern bool RunColorDialog(HWND hWndOwner, WNDPROC lpfnHookProc, ref COLORREF lpColor, CUSTCOLORS lpCustomColors);

    [DllImport(App.NativesDll, EntryPoint = "#21")]
    public static extern bool RunFontDialog(HWND hWndOwner, WNDPROC lpfnHookProc, ref LOGFONT lpLogFont, int nSizeLimit);

    [DllImport(App.NativesDll, EntryPoint = "#22")]
    public static extern void SetRoundCorner(HWND hWnd, int width, int height, int radius);

    [DllImport(App.NativesDll, EntryPoint = "#23")]
    public static extern void SetRoundCornerEx(HWND hWnd, bool isSmall);

    [DllImport(App.NativesDll, EntryPoint = "#24")]
    public static extern void EnableDarkModeForApp();

    [DllImport(App.NativesDll, EntryPoint = "#25")]
    public static extern void EnableDarkModeForWindowFrame(HWND hWnd, bool after20h1);

    [DllImport(App.NativesDll, EntryPoint = "#26")]
    public static extern void SetBorderColor(HWND hWnd, COLORREF color, bool enabled);

    [DllImport(App.NativesDll, EntryPoint = "#27")]
    public static extern int GetSystemAccentColor();

    [DllImport(App.NativesDll, EntryPoint = "#28")]
    public static extern void ListViewSelectAllItems(HWND hLV, bool selected);

    [DllImport(App.NativesDll, EntryPoint = "#29")]
    public static extern void SetTopMostWindow(HWND hWnd);

    [DllImport(App.NativesDll, EntryPoint = "#30")]
    public static extern bool MenuGetItemCheckStateByPosition(IntPtr hMenu, int item);

    [DllImport(App.NativesDll, EntryPoint = "#31")]
    public static extern bool MenuCheckRadioItemByPosition(IntPtr hMenu, int item);

    [DllImport(App.NativesDll, EntryPoint = "#32", CharSet = CharSet.Unicode)]
    public static extern string GetWindowText(HWND hWnd);

    [DllImport(App.NativesDll, EntryPoint = "#33", CharSet = CharSet.Unicode)]
    public static extern string GetClassName(HWND hWnd);
}
