using System;
using System.Runtime.InteropServices;
using PlainCEETimer.Modules;

namespace PlainCEETimer.Interop;

public static class Win32UI
{
    [DllImport(App.User32Dll)]
    public static extern void MoveWindow(HWND hWnd, int X, int Y, int nWidth, int nHeight, BOOL bRepaint);

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
    public static extern BOOL DestroyIcon(IntPtr hIcon);

    [DllImport(App.Gdi32Dll)]
    public static extern BOOL DeleteObject(IntPtr hObject);

    [DllImport(App.User32Dll)]
    public static extern BOOL EnumChildWindows(HWND hWndParent, EnumChildProc lpEnumFunc, IntPtr lParam);

    [DllImport(App.User32Dll)]
    public static extern BOOL GetWindowRect(HWND hWnd, out RECT lpRect);

    [DllImport(App.User32Dll)]
    public static extern BOOL GetClientRect(HWND hWnd, out RECT lpRect);

    [DllImport(App.User32Dll, CharSet = CharSet.Unicode)]
    public static extern BOOL SetWindowText(HWND hWnd, string lpString);

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

    [DllImport(App.NativesDll, EntryPoint = "#10")]
    public static extern BOOL RunColorDialog(HWND hWndOwner, WNDPROC lpfnHookProc, ref COLORREF lpColor, CUSTCOLORS lpCustomColors);

    [DllImport(App.NativesDll, EntryPoint = "#11")]
    public static extern BOOL RunFontDialog(HWND hWndOwner, WNDPROC lpfnHookProc, ref LOGFONT lpLogFont, int nSizeLimit);

    [DllImport(App.NativesDll, EntryPoint = "#28")]
    public static extern void ListViewSelectAllItems(HWND hLV, BOOL selected);

    [DllImport(App.NativesDll, EntryPoint = "#29")]
    public static extern BOOL MenuGetItemCheckStateByPosition(IntPtr hMenu, int item);

    [DllImport(App.NativesDll, EntryPoint = "#30")]
    public static extern BOOL MenuCheckRadioItemByPosition(IntPtr hMenu, int item);

    [DllImport(App.NativesDll, EntryPoint = "#31")]
    public static extern void SetTopMostWindow(HWND hWnd);

    [DllImport(App.NativesDll, EntryPoint = "#32", CharSet = CharSet.Unicode)]
    public static extern string GetWindowText(HWND hWnd);

    [DllImport(App.NativesDll, EntryPoint = "#33", CharSet = CharSet.Unicode)]
    public static extern string GetClassName(HWND hWnd);
}
