using System;
using System.Runtime.InteropServices;
using PlainCEETimer.Modules;

namespace PlainCEETimer.Interop;

public static class Win32UI
{
    [DllImport(App.NativesDll, EntryPoint = "#28")]
    public static extern void ListViewSelectAllItems(HWND hLV, BOOL selected);

    [DllImport(App.NativesDll, EntryPoint = "#29")]
    public static extern BOOL MenuGetItemCheckStateByPosition(IntPtr hMenu, int item);

    [DllImport(App.NativesDll, EntryPoint = "#30")]
    public static extern BOOL MenuCheckRadioItemByPosition(IntPtr hMenu, int item);

    [DllImport(App.NativesDll, EntryPoint = "#31")]
    public static extern void SetTopMostWindow(HWND hWnd);

    [DllImport(App.NativesDll, EntryPoint = "#32", CharSet = CharSet.Unicode)]
    public static extern string GetWindowTextEx(HWND hWnd);
}
