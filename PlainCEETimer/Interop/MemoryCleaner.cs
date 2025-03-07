using PlainCEETimer.Modules;
using System.Runtime.InteropServices;

namespace PlainCEETimer.Interop
{
    public static class MemoryCleaner
    {
        [DllImport(App.AppNativesDll, CallingConvention = CallingConvention.StdCall)]
        public static extern void CleanMemory(int threshold);
    }
}
