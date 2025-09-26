using System;
using System.Runtime.InteropServices;
using PlainCEETimer.Modules;

namespace PlainCEETimer.Interop;

public static class Natives
{
    [DllImport(App.User32Dll)]
    public static extern IntPtr SendMessage(HWND hWnd, int msg, IntPtr wParam, IntPtr lParam);

    [DllImport(App.Gdi32Dll)]
    public static extern COLORREF SetTextColor(HDC hdc, COLORREF color);

    /*

    提取 DLL 里的图标参考:

    How can I use the images within shell32.dll in my C# project? - Stack Overflow
    https://stackoverflow.com/a/6873026/21094697

    */

    [DllImport(App.Shell32Dll, CharSet = CharSet.Unicode)]
    public static extern int ExtractIconEx(string lpszFile, int nIconIndex, out HICON phiconLarge, HICON phiconSmall, int nIcons);

    [DllImport(App.User32Dll)]
    public static extern BOOL DestroyIcon(IntPtr hIcon);
}

public delegate IntPtr WNDPROC(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);