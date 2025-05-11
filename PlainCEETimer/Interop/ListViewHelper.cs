using System;
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
                ThemeManager.FlushControl(hLV, NativeStyle.ItemsView);
                FlushHeaderTheme(hLV, ThemeManager.DarkFore.ToWin32(), useDark.ToWin32());
            }
            else
            {
                ThemeManager.FlushControl(hLV, NativeStyle.ExplorerLight);
            }

        }

        [DllImport(App.NativesDll, EntryPoint = "#10")]
        private static extern void FlushHeaderTheme(IntPtr hLV, int hFColor, int enable);

        [DllImport(App.NativesDll, EntryPoint = "#1")]
        public static extern void SelectAllItems(IntPtr hLV, int selected);
    }
}
