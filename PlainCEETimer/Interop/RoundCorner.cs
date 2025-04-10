using PlainCEETimer.Modules;
using System;
using System.Runtime.InteropServices;

namespace PlainCEETimer.Interop
{
    public static class RoundCorner
    {
        [DllImport(App.NativesDll, EntryPoint = "#4")]
        public static extern void SetRoundCornerModern(IntPtr hWnd);

        [DllImport(App.NativesDll, EntryPoint = "#3")]
        public static extern void SetRoundCornerRegion(IntPtr hWnd, int wndWidth, int wndHeight, int radius);
    }
}
