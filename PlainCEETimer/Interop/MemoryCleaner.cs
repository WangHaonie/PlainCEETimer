using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using PlainCEETimer.Modules;

namespace PlainCEETimer.Interop
{
    public static class MemoryCleaner
    {
        private static readonly int PID = Process.GetCurrentProcess().Id;
        private static ManagementObjectSearcher WmiSearcher;

        private const ulong Threshold = 9UL * 1024 * 1024;

        public static void Clean()
        {
            var mem = GetMemoryEx();

            if (mem == 0)
            {
                WmiSearcher ??= new($"SELECT WorkingSetPrivate FROM Win32_PerfFormattedData_PerfProc_Process WHERE IDProcess = {PID}");
                mem = (ulong)WmiSearcher.Get().Cast<ManagementBaseObject>().FirstOrDefault().GetPropertyValue("WorkingSetPrivate");
            }

            if (mem > Threshold)
            {
                ClearMemory();
            }
        }

        [DllImport(App.NativesDll, EntryPoint = "#4")]
        private static extern ulong GetMemoryEx();

        [DllImport(App.NativesDll, EntryPoint = "#5")]
        private static extern void ClearMemory();
    }
}
