using System.Runtime.InteropServices;
using PlainCEETimer.Modules;

namespace PlainCEETimer.Interop
{
    public static class NativeInterop
    {
        [DllImport(App.User32Dll)]
        public static extern uint GetDpiForSystem();
    }
}
