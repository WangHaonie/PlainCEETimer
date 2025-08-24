using System;
using System.Runtime.InteropServices;
using PlainCEETimer.Modules;

namespace PlainCEETimer.Interop;

public static class ListViewHelper
{
    [DllImport(App.NativesDll, EntryPoint = "#3")]
    public static extern void SelectAllItems(IntPtr hLV, BOOL selected);

    [DllImport(App.User32Dll)]
    public static extern BOOL SetWindowPos(HWND hWnd, HWND hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
}
