using System;
using System.Runtime.InteropServices;
using PlainCEETimer.Modules;

namespace PlainCEETimer.Interop
{
    public static class ListViewHelper
    {
        private const int WM_USER = 0x0400;
        private const int LV_INITNOW = WM_USER + 13;

        public static void FlushTheme(IntPtr hLV, IntPtr hLVH)
        {
            FlushHeaderTheme(hLV, hLVH);
            ThemeManager.FlushDarkControl(hLVH, NativeStyle.ItemsView);
            ThemeManager.FlushDarkControl(hLV, NativeStyle.ItemsView);
            CommonDialogHelper.PostMessage(hLV, LV_INITNOW, 0, 0);
        }

        [DllImport(App.NativesDll, EntryPoint = "#1")]
        public static extern void SelectAllItems(IntPtr hLV, int selected);

        [DllImport(App.NativesDll, EntryPoint = "#10")]
        public static extern IntPtr GetHeader(IntPtr hLV);

        [DllImport(App.NativesDll, EntryPoint = "#11")]
        private static extern void FlushHeaderTheme(IntPtr hLV, IntPtr hLVH);
    }
}
