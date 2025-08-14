using System.Runtime.InteropServices;
using PlainCEETimer.Modules;

namespace PlainCEETimer.Interop
{
    public static class RoundCorner
    {
        [DllImport(App.NativesDll, EntryPoint = "#1")]
        public static extern void SetModern(HWND hWnd, BOOL isSmall);

        [DllImport(App.NativesDll, EntryPoint = "#2")]
        public static extern void SetRegion(HWND hWnd, int width, int height, int radius);
    }
}
