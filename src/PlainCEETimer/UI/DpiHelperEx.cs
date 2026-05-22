using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using PlainCEETimer.Interop;
using PlainCEETimer.Interop.Extensions;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Fody;
using PlainCEETimer.UI.Extensions;

namespace PlainCEETimer.UI;

[NoConstants]
public static class DpiHelperEx
{
    /*
    
    混合 DPI 感知上下文 参考：

    Mixed-Mode DPI Scaling and DPI-aware APIs - Win32 apps | Microsoft Learn
    https://learn.microsoft.com/en-us/windows/win32/hidpi/high-dpi-improvements-for-desktop-applications

    DPI_AWARENESS_CONTEXT handle (windef.h) - Win32 apps | Microsoft Learn
    https://learn.microsoft.com/en-us/windows/win32/hidpi/dpi-awareness-context

    SetThreadDpiAwarenessContext function (winuser.h) - Win32 apps | Microsoft Learn
    https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-setthreaddpiawarenesscontext

    SetProcessDpiAwareness function (shellscalingapi.h) - Win32 apps | Microsoft Learn
    https://learn.microsoft.com/en-us/windows/win32/api/shellscalingapi/nf-shellscalingapi-setprocessdpiawareness

    */

    private const int PROCESS_DPI_UNAWARE = 0;
    private const int PROCESS_SYSTEM_DPI_AWARE = 1;
    private const int PROCESS_PER_MONITOR_DPI_AWARE = 2;

    private static readonly bool isDpiAwareSupported = SystemVersion.Current.AtLeast(WindowsVersions.NT6);
    private static readonly bool isDpiAwarenessSupported = SystemVersion.Current.AtLeast(WindowsVersions.Windows81);
    private static readonly bool isApiSupported = SystemVersion.Current.AtLeast(WindowsVersions.Windows10_1607);
    private static readonly bool isPMv2Supported = SystemVersion.Current.AtLeast(WindowsVersions.Windows10_1703);
    private static readonly bool isGdiScaledSupported = SystemVersion.Current.AtLeast(WindowsVersions.Windows10_1809);

    public static DpiAwarenessContext Current
    {
        get
        {
            if (isApiSupported)
            {
                return Win32UI.GetThreadDpiAwarenessContext()
                    .AsDpiAwarenessContextHandle().Value;
            }

            if (isDpiAwarenessSupported && TryGetProcessDpiAwareness(out var processContext))
            {
                return processContext;
            }

            if (isDpiAwareSupported)
            {
                return Win32UI.IsProcessDPIAware()
                    ? DpiAwarenessContext.System
                    : DpiAwarenessContext.Unaware;
            }

            return DpiAwarenessContext.Unknown;
        }
    }

    public static int GetFriendlyScale(int dpi)
    {
        return (int)Math.Round(dpi / 96D * 100D);
    }

    public static DpiAwarenessContext SetDpiContext(DpiAwarenessContext dpiContext)
    {
        if (isApiSupported)
        {
            return SetDpiContextCore(dpiContext);
        }

        return LegacySetDpiContext(dpiContext);
    }

    public static int GetDpiForWindow(IAppWindow window)
    {
        var hwnd = window.Handle;

        if (isApiSupported)
        {
            return (int)Win32UI.GetDpiForWindow(hwnd);
        }
        else
        {
            using var g = Graphics.FromHwnd(hwnd);
            return (int)g.DpiX;
        }
    }

    public static int GetSystemMetricsForDpi(int nIndex, int dpi)
    {
        if (isApiSupported)
        {
            return Win32UI.GetSystemMetricsForDpi(nIndex, (uint)dpi);
        }

        return Win32UI.GetSystemMetrics(nIndex);
    }

    internal static void Initialize()
    {
        DpiHelper.enableHighDpi = true;
        DpiHelper.enableDpiChangedMessageHandling = SystemVersion.Current.AtLeast(WindowsVersions.Windows10_RS2);
        DpiHelper.isInitialized = true;
    }

    internal static void GlobalUpdateDeviceDpi()
    {
        var dpi = GetDeviceDpi();
        DpiHelper.deviceDpi = dpi;
        DpiHelper.logicalToDeviceUnitsScalingFactor = dpi / 96;
    }

    private static bool TryGetProcessDpiAwareness(out DpiAwarenessContext dpiContext)
    {
        if (Win32UI.GetProcessDpiAwareness(IntPtr.Zero, out var value).Succeeded)
        {
            dpiContext = ConvertFromLegacy(value);
            return true;
        }

        dpiContext = DpiAwarenessContext.Unknown;
        return false;
    }

    private static DpiAwarenessContext SetDpiContextCore(DpiAwarenessContext dpiContext)
    {
        if (!isGdiScaledSupported && dpiContext == DpiAwarenessContext.GdiScaled)
        {
            return DpiAwarenessContext.Unknown;
        }

        if (!isPMv2Supported && dpiContext == DpiAwarenessContext.PerMonitorV2)
        {
            dpiContext = DpiAwarenessContext.PerMonitor;
        }

        return Win32UI.SetThreadDpiAwarenessContext(dpiContext)
            .AsDpiAwarenessContextHandle().Value;
    }

    private static DpiAwarenessContext LegacySetDpiContext(DpiAwarenessContext dpiContext)
    {
        var oldContext = Current;

        if (oldContext == dpiContext)
        {
            return oldContext;
        }

        if (isDpiAwarenessSupported)
        {
            return Win32UI.SetProcessDpiAwareness(ConvertToLegacy(dpiContext)).Succeeded
                ? oldContext
                : DpiAwarenessContext.Unknown;
        }

        if (isDpiAwareSupported && dpiContext == DpiAwarenessContext.System)
        {
            return Win32UI.SetProcessDPIAware()
                ? oldContext
                : DpiAwarenessContext.Unknown;
        }

        return DpiAwarenessContext.Unknown;
    }

    private static double GetDeviceDpi()
    {
        if (isDpiAwarenessSupported
            && Win32UI.GetDpiForMonitor(Screen.PrimaryScreen.GetHandle(MemberTypes.Field, "hmonitor"),
                MONITOR_DPI_TYPE.EFFECTIVE, out var dpix, out _).Succeeded)
        {
            return dpix;
        }

        var dc = Win32UI.GetDC(IntPtr.Zero);
        var dpi = 0;

        if (dc != IntPtr.Zero)
        {
            dpi = Win32UI.GetDeviceCaps(dc, WinGdi.LOGPIXELSX);
            Win32UI.ReleaseDC(IntPtr.Zero, dc);
        }

        return dpi;
    }

    private static int ConvertToLegacy(DpiAwarenessContext dpiContext) => dpiContext switch
    {
        DpiAwarenessContext.Unaware => PROCESS_DPI_UNAWARE,
        DpiAwarenessContext.PerMonitor or DpiAwarenessContext.PerMonitorV2 => PROCESS_PER_MONITOR_DPI_AWARE,
        DpiAwarenessContext.System => PROCESS_SYSTEM_DPI_AWARE,
        _ => PROCESS_DPI_UNAWARE
    };

    private static DpiAwarenessContext ConvertFromLegacy(int value) => value switch
    {
        PROCESS_DPI_UNAWARE => DpiAwarenessContext.Unaware,
        PROCESS_SYSTEM_DPI_AWARE => DpiAwarenessContext.System,
        PROCESS_PER_MONITOR_DPI_AWARE => DpiAwarenessContext.PerMonitor,
        _ => DpiAwarenessContext.Unknown,
    };
}
