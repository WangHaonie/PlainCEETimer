using System.Runtime.InteropServices;
using PlainCEETimer.Modules;

namespace PlainCEETimer.Interop
{
    public static class CommonDialogs
    {
        [DllImport(App.NativesDll, EntryPoint = "#24")]
        public static extern BOOL RunColorDialog(HWND hWndOwner, ref COLORREF lpColor, WNDPROC lpfnHookProc, COLORREFS lpCustomColors);

        [DllImport(App.NativesDll, EntryPoint = "#25")]
        public static extern BOOL RunFontDialog(HWND hWndOwner, ref LOGFONT lpLogFont, WNDPROC lpfnHookProc, int nSizeMin, int nSizeMax);
    }
}
