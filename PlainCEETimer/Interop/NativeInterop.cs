using PlainCEETimer.Modules;
using System.Runtime.InteropServices;

namespace PlainCEETimer.Interop
{
    public static class NativeInterop
    {
        [DllImport(App.User32Dll)]
        public static extern uint GetDpiForSystem();
    }
}
