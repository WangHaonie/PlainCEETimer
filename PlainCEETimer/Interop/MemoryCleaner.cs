using System;
using System.Diagnostics;
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
            var mem = GetMemoryEx().ToUInt64();

            if (mem == 0)
            {
                WmiSearcher ??= new($"SELECT WorkingSetPrivate FROM Win32_PerfFormattedData_PerfProc_Process WHERE IDProcess = {PID}");

                foreach (var mo in WmiSearcher.Get())
                {
                    mem = (ulong)mo["WorkingSetPrivate"];
                }
            }

            if (mem > Threshold)
            {
                ClearMemory();
            }
        }

        [DllImport(App.NativesDll, EntryPoint = "#2")]
        private static extern void ClearMemory();

        [DllImport(App.NativesDll, EntryPoint = "#13")]
        private static extern UIntPtr GetMemoryEx();
    }
}
