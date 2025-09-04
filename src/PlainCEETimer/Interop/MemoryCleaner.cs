using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using PlainCEETimer.Modules;

namespace PlainCEETimer.Interop;

public static class MemoryCleaner
{
    private static ManagementObjectSearcher WmiSearcher;

    private const ulong Threshold = 9UL * 1024 * 1024;

    public static void Clean()
    {
        var mem = GetMemoryEx();

        if (mem == 0)
        {
            mem = GetMemory();
        }

        if (mem > Threshold)
        {
            ClearMemory();
        }
    }

    private static ulong GetMemory()
    {
        WmiSearcher ??= new("select WorkingSetPrivate from Win32_PerfFormattedData_PerfProc_Process where IDProcess=" + GetCurrentProcessId());
        var obj = WmiSearcher.Get().Cast<ManagementBaseObject>().FirstOrDefault();
        return obj == default ? default : (ulong)obj.GetPropertyValue("WorkingSetPrivate");
    }

    [DllImport(App.NativesDll, EntryPoint = "#4")]
    private static extern ulong GetMemoryEx();

    [DllImport(App.NativesDll, EntryPoint = "#5")]
    private static extern void ClearMemory();

    [DllImport(App.Kernel32Dll)]
    private static extern uint GetCurrentProcessId();
}