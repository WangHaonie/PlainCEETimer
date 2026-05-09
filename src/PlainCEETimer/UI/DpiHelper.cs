using System;
using PlainCEETimer.Interop;
using PlainCEETimer.Interop.Extensions;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Fody;

namespace PlainCEETimer.UI;

[NoConstants]
public static class DpiHelper
{
    /*
    
    混合 DPI 感知上下文 参考：

    Mixed-Mode DPI Scaling and DPI-aware APIs - Win32 apps | Microsoft Learn
    https://learn.microsoft.com/en-us/windows/win32/hidpi/high-dpi-improvements-for-desktop-applications

    */

    private const int S_OK = 0;
    private const int PROCESS_DPI_UNAWARE = 0;
    private const int PROCESS_SYSTEM_DPI_AWARE = 1;
    private const int PROCESS_PER_MONITOR_DPI_AWARE = 2;

    private static readonly bool flag = SystemVersion.Current.AtLeast(new(10, 0, WindowsBuilds.Windows10_1607));
    private static readonly bool isDpiAwareSupported = SystemVersion.Current.AtLeast(new(6, 0));
    private static readonly bool isDpiAwarenessSupported = SystemVersion.Current.AtLeast(new(6, 3, WindowsBuilds.Windows81));
    private static readonly bool isGdiScaledSupported = SystemVersion.Current.AtLeast(new(10, 0, WindowsBuilds.Windows10_1809));

    public static DpiAwarenessContext Current
    {
        get
        {
            if (flag)
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

    public static DpiAwarenessContext SetDpiContext(DpiAwarenessContext dpiContext)
    {
        if (flag)
        {
            if (dpiContext == DpiAwarenessContext.GdiScaled && !isGdiScaledSupported)
            {
                return DpiAwarenessContext.Unknown;
            }

            return Win32UI.SetThreadDpiAwarenessContext(dpiContext)
                .AsDpiAwarenessContextHandle().Value;
        }

        var oldContext = Current;

        if (oldContext == dpiContext)
        {
            return oldContext;
        }

        if (isDpiAwarenessSupported)
        {
            return TryConvertToProcessDpiAwareness(dpiContext, out var value)
                && TrySetProcessDpiAwareness(value)
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

    private static bool TryGetProcessDpiAwareness(out DpiAwarenessContext dpiContext)
    {
        if (Win32UI.GetProcessDpiAwareness(IntPtr.Zero, out var value) == S_OK)
        {
            dpiContext = ConvertFromProcessDpiAwareness(value);
            return true;
        }

        dpiContext = DpiAwarenessContext.Unknown;
        return false;
    }

    private static bool TrySetProcessDpiAwareness(int value)
    {
        return Win32UI.SetProcessDpiAwareness(value) == S_OK;
    }

    private static bool TryConvertToProcessDpiAwareness(DpiAwarenessContext dpiContext, out int value)
    {
        switch (dpiContext)
        {
            case DpiAwarenessContext.Unaware:
                value = PROCESS_DPI_UNAWARE;
                return true;
            case DpiAwarenessContext.System:
                value = PROCESS_SYSTEM_DPI_AWARE;
                return true;
            case DpiAwarenessContext.PerMonitor:
                value = PROCESS_PER_MONITOR_DPI_AWARE;
                return true;
            default:
                value = default;
                return false;
        }
    }

    private static DpiAwarenessContext ConvertFromProcessDpiAwareness(int value)
    {
        switch (value)
        {
            case PROCESS_DPI_UNAWARE:
                return DpiAwarenessContext.Unaware;
            case PROCESS_SYSTEM_DPI_AWARE:
                return DpiAwarenessContext.System;
            case PROCESS_PER_MONITOR_DPI_AWARE:
                return DpiAwarenessContext.PerMonitor;
            default:
                return DpiAwarenessContext.Unknown;
        }
    }
}
