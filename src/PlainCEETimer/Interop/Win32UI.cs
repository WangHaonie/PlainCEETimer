using System;
using System.Runtime.InteropServices;
using PlainCEETimer.Modules;

namespace PlainCEETimer.Interop;

public static class Win32UI
{
    [DllImport(App.NativesDll, EntryPoint = "#3")]
    public static extern void ListViewSelectAllItems(HWND hLV, BOOL selected);

    [DllImport(App.NativesDll, EntryPoint = "#28")]
    public static extern BOOL MenuGetItemCheckStateByPosition(IntPtr hMenu, int iItemIndex);
}
