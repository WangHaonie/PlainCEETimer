using System;
using System.Runtime.InteropServices;
using PlainCEETimer.Modules;

namespace PlainCEETimer.Interop
{
    public static class MemoryCleaner
    {
        [DllImport(App.NativesDll, EntryPoint = "#2")]
        public static extern void CleanMemory(UIntPtr threshold);
    }
}
