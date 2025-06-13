using System;
using System.Runtime.InteropServices;
using PlainCEETimer.Modules;

namespace PlainCEETimer.Interop
{
    public static class ListViewHelper
    {
        [DllImport(App.NativesDll, EntryPoint = "#1")]
        public static extern void SelectAllItems(IntPtr hLV, int selected);

        [DllImport(App.NativesDll, EntryPoint = "#10")]
        public static extern void FlushTheme(IntPtr hLV, int colorHFore, int enable);
    }
}
