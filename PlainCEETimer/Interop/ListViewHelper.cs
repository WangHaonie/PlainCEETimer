using System;
using System.Drawing;
using System.Runtime.InteropServices;
using PlainCEETimer.Modules;

namespace PlainCEETimer.Interop
{
    public static class ListViewHelper
    {
        public static void FlushTheme(IntPtr hLV)
        {
            FlushHeaderTheme(hLV, ColorTranslator.ToWin32(ThemeManager.DarkFore));
            ThemeManager.FlushDarkControl(GetHeader(hLV), NativeStyle.ItemsView);
            ThemeManager.FlushDarkControl(hLV, NativeStyle.ItemsView);
        }

        [DllImport(App.NativesDll, EntryPoint = "#1")]
        public static extern void SelectAllItems(IntPtr hLV, int selected);

        [DllImport(App.NativesDll, EntryPoint = "#10")]
        private static extern IntPtr GetHeader(IntPtr hLV);

        [DllImport(App.NativesDll, EntryPoint = "#11")]
        private static extern void FlushHeaderTheme(IntPtr hLV, int hFColor);
    }
}
