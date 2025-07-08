using System;
using System.Runtime.InteropServices;
using PlainCEETimer.Modules;

namespace PlainCEETimer.Interop
{
    public static class ListViewHelper
    {
        [DllImport(App.NativesDll, EntryPoint = "#1")]
        public static extern void SelectAllItems(IntPtr hLV, BOOL selected);

        [DllImport(App.NativesDll, EntryPoint = "#10")]
        public static extern void FlushTheme(IntPtr hLV, COLORREF crHeaderFore, BOOL enabled);

        [DllImport(App.NativesDll, EntryPoint = "#14")]
        public static extern BOOL HasScrollBar(IntPtr hWnd, BOOL isVertical);
    }
}
