using System;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Threading;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Fody;

namespace PlainCEETimer.Interop;

[NoConstants]
public class MemoryCleaner : IDisposable
{
    private bool IsRunning;
    private Timer MainTimer;
    private ManagementObjectSearcher WmiSearcher;
    private const int MemCleanerInterval = 300_000;
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

        GC.SuppressFinalize(this);
    }

    private void Clean()
    {
        var mem = GetProcessPrivateWS();

        if (mem == 0)
        {
            mem = GetMemory();
        }

        if (mem > Threshold)
        {
            ClearProcessWS();
        }
    }

    private ulong GetMemory()
    {
        WmiSearcher ??= new("select WorkingSetPrivate from Win32_PerfFormattedData_PerfProc_Process where IDProcess=" + GetCurrentProcessId());
        var obj = WmiSearcher.Get().Cast<ManagementBaseObject>().FirstOrDefault();
        return obj == default ? default : (ulong)obj.GetPropertyValue("WorkingSetPrivate");
    }

    ~MemoryCleaner()
    {
        Dispose();
    }

    [DllImport(App.NativesDll, EntryPoint = "#1")]
    private static extern ulong GetProcessPrivateWS();

    [DllImport(App.NativesDll, EntryPoint = "#2")]
    private static extern void ClearProcessWS();

    [DllImport(App.Kernel32Dll)]
    private static extern uint GetCurrentProcessId();
}