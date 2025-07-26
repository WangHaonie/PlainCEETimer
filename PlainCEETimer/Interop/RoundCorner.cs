using System;
using System.Runtime.InteropServices;
using PlainCEETimer.Modules;

namespace PlainCEETimer.Interop
{
    public static class RoundCorner
    {
        [DllImport(App.NativesDll, EntryPoint = "#1")]
        public static extern void SetModern(IntPtr hWnd);

        [DllImport(App.NativesDll, EntryPoint = "#2")]
        public static extern void SetRegion(IntPtr hWnd, int wndWidth, int wndHeight, int radius);
    }
}
