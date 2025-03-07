using PlainCEETimer.Modules;
using System.Runtime.InteropServices;

namespace PlainCEETimer.Interop
{
    public static class DpiAwareness
    {
        [DllImport(App.AppNativesDll, CallingConvention = CallingConvention.StdCall)]
        public static extern void SetProcessDpiAwarenessEx(int windowsid);
    }
}


