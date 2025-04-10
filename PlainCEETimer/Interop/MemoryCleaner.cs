using PlainCEETimer.Modules;
using System.Runtime.InteropServices;

namespace PlainCEETimer.Interop
{
    public static class MemoryCleaner
    {
        [DllImport(App.NativesDll, EntryPoint = "#2")]
        public static extern void CleanMemory(int threshold);
    }
}
