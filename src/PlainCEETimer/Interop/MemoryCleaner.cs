using System;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Annotations.Fody;
using PlainCEETimer.Modules.Extensions;

namespace PlainCEETimer.Interop;

[NoConstants]
[SuppressUnmanagedCodeSecurity]
public class MemoryCleaner : IDisposable
{
    public bool Enabled
    {
        get;
        set
        {
            if (field != value)
            {
                if (value)
                {
                    Start();
                }
                else
                {
                    Dispose();
                }
            }
        }
    }

    public static MemoryCleaner Instance => field ??= new();

    private Timer MainTimer;
    private ManagementObjectSearcher WmiSearcher;
    private const int MemCleanerInterval = 300_000;
    private const ulong Threshold = 9UL * 1024 * 1024;

    private MemoryCleaner()
    {
        return;
    }

    public void Dispose()
    {
        MainTimer.Destroy();
        MainTimer = null;
        GC.SuppressFinalize(this);
    }

    private void Clean()
    {
        var mem = PnGetProcessPrivateWS();

        if (mem == 0)
        {
            mem = GetMemory();
        }

        if (mem > Threshold)
        {
            PnClearProcessWS();
        }
    }

    private ulong GetMemory()
    {
        WmiSearcher ??= new("select WorkingSetPrivate from Win32_PerfFormattedData_PerfProc_Process where IDProcess=" + Win32.GetCurrentProcessId());
        var obj = WmiSearcher.Get().Cast<ManagementBaseObject>().FirstOrDefault();
        return obj == default ? default : (ulong)obj.GetPropertyValue("WorkingSetPrivate");
    }

    private void Start()
    {
        MainTimer ??= new(_ => Clean(), null, 3000, MemCleanerInterval);
    }

    ~MemoryCleaner()
    {
        Dispose();
    }

    [DllImport(App.NativesDll, EntryPoint = "#19")]
    private static extern ulong PnGetProcessPrivateWS();

    [DllImport(App.NativesDll, EntryPoint = "#18")]
    private static extern void PnClearProcessWS();
}