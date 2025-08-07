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
                WmiSearcher ??= new($"select WorkingSetPrivate from Win32_PerfFormattedData_PerfProc_Process where IDProcess={PID}");
                mem = (ulong)WmiSearcher.Get().Cast<ManagementBaseObject>().First().GetPropertyValue("WorkingSetPrivate");
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
