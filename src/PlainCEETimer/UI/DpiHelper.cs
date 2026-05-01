using PlainCEETimer.Interop;
using PlainCEETimer.Interop.Extensions;
using PlainCEETimer.Modules;

namespace PlainCEETimer.UI;

public static class DpiHelper
{
    /*
    
    混合 DPI 感知上下文 参考：

    Mixed-Mode DPI Scaling and DPI-aware APIs - Win32 apps | Microsoft Learn
    https://learn.microsoft.com/en-us/windows/win32/hidpi/high-dpi-improvements-for-desktop-applications

    */

    private static readonly bool flag = SystemVersion.Is1607OrLater;

    public static DpiAwarenessContext Current
    {
        get
        {
            if (flag)
            {
                return Win32UI.GetThreadDpiAwarenessContext()
                    .AsDpiAwarenessContextHandle().Value;
            }

            return DpiAwarenessContext.Unknown;
        }
    }

    public static DpiAwarenessContext SetDpiContext(DpiAwarenessContext dpiContext)
    {
        if (!flag || (dpiContext == DpiAwarenessContext.GdiScaled && !SystemVersion.Is1809OrLater))
        {
            return DpiAwarenessContext.Unknown;
        }

        return Win32UI.SetThreadDpiAwarenessContext(dpiContext)
            .AsDpiAwarenessContextHandle().Value;
    }
}