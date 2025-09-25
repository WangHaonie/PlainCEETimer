using System.Runtime.InteropServices;
using PlainCEETimer.Modules;

namespace PlainCEETimer.Interop;

public static class RoundCorner
{
    [DllImport(App.NativesDll, EntryPoint = "#8")]
    public static extern void SetRoundCorner(HWND hWnd, int width, int height, int radius);

    [DllImport(App.NativesDll, EntryPoint = "#9")]
    public static extern void SetRoundCornerEx(HWND hWnd, BOOL isSmall);
}