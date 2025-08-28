using System.Runtime.InteropServices;
using PlainCEETimer.Modules;

namespace PlainCEETimer.Interop
{
    public static class CommonDialogs
    {
        [DllImport(App.NativesDll, EntryPoint = "#24")]
        public static extern BOOL RunColorDialog(HWND hWndOwner, WNDPROC lpfnHookProc, ref COLORREF lpColor, CUSTCOLORS lpCustomColors);

        [DllImport(App.NativesDll, EntryPoint = "#25")]
        public static extern BOOL RunFontDialog(HWND hWndOwner, WNDPROC lpfnHookProc, ref LOGFONT lpLogFont, int nSizeLimit);
    }
}
