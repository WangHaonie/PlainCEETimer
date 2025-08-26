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
        /*
            
        使用 WMI 获取进程 Private WS 参考：

        Is it possible to get 'private working set' of memory ? · Issue #15 · sindresorhus/tasklist
        https://github.com/sindresorhus/tasklist/issues/15#issuecomment-402712280

        */

        WmiSearcher ??= new("select WorkingSetPrivate from Win32_PerfFormattedData_PerfProc_Process where IDProcess=" + GetCurrentProcessId());
        return (ulong)WmiSearcher.Get().Cast<ManagementBaseObject>().First().GetPropertyValue("WorkingSetPrivate");
    }

    [DllImport(App.NativesDll, EntryPoint = "#4")]
    private static extern ulong GetMemoryEx();

    [DllImport(App.NativesDll, EntryPoint = "#5")]
    private static extern void ClearMemory();

    [DllImport("kernel32.dll")]
    private static extern uint GetCurrentProcessId();
}