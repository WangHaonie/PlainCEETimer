using System;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Threading;
using PlainCEETimer.Modules;

namespace PlainCEETimer.Interop;

public class MemoryCleaner : IDisposable
{
    private bool IsRunning;
    private Timer MainTimer;
    private ManagementObjectSearcher WmiSearcher;

    private const int MemCleanerInterval = 300_000; // 5 min
    private const ulong Threshold = 9UL * 1024 * 1024;

    public void Start()
    {
        if (!IsRunning)
        {
            MainTimer = new(_ => Clean(), null, 3000, MemCleanerInterval);
            IsRunning = true;
        }
    }

    public void Dispose()
    {
        if (IsRunning)
        {
            MainTimer.Dispose();
            IsRunning = false;
        }
    }

    private void Clean()
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

    private ulong GetMemory()
    {
        WmiSearcher ??= new("select WorkingSetPrivate from Win32_PerfFormattedData_PerfProc_Process where IDProcess=" + GetCurrentProcessId());
        var obj = WmiSearcher.Get().Cast<ManagementBaseObject>().FirstOrDefault();
        return obj == default ? default : (ulong)obj.GetPropertyValue("WorkingSetPrivate");
    }

    [DllImport(App.NativesDll, EntryPoint = "#1")]
    private static extern ulong GetMemoryEx();

    [DllImport(App.NativesDll, EntryPoint = "#2")]
    private static extern void ClearMemory();

    [DllImport(App.Kernel32Dll)]
    private static extern uint GetCurrentProcessId();
}