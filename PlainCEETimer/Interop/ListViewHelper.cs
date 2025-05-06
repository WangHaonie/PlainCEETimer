using System;
using System.Drawing;
using System.Runtime.InteropServices;
using PlainCEETimer.Modules;

namespace PlainCEETimer.Interop
{
    public static class ListViewHelper
    {
        public static void FlushTheme(IntPtr hLV, bool useDark)
        {
            if (useDark)
            {
                FlushHeaderTheme(hLV, ThemeManager.DarkFore.ToWin32());
                ThemeManager.FlushDarkControl(hLV, NativeStyle.Explorer);
                ThemeManager.FlushDarkControl(GetToolTips(hLV), NativeStyle.Explorer);
                ThemeManager.FlushDarkControl(GetHeader(hLV), NativeStyle.ItemsView);
            }
            else
            {
                ThemeManager.FlushDarkControl(hLV, NativeStyle.ExplorerLight);
                ThemeManager.FlushDarkControl(GetToolTips(hLV), NativeStyle.ExplorerLight);
            }
        }

        [DllImport(App.NativesDll, EntryPoint = "#10")]
        private static extern IntPtr GetHeader(IntPtr hLV);

        [DllImport(App.NativesDll, EntryPoint = "#12")]
        private static extern IntPtr GetToolTips(IntPtr hLV);

        [DllImport(App.NativesDll, EntryPoint = "#11")]
        private static extern void FlushHeaderTheme(IntPtr hLV, int hFColor);

        [DllImport(App.NativesDll, EntryPoint = "#1")]
        public static extern void SelectAllItems(IntPtr hLV, int selected);
    }
}
