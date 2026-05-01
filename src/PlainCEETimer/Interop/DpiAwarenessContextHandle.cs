using System;

namespace PlainCEETimer.Interop;

public class DpiAwarenessContextHandle(IntPtr ptr)
{
    public DpiAwarenessContext Value
    {
        get
        {
            foreach (var context in m_values)
            {
                if (Win32UI.AreDpiAwarenessContextsEqual(context, ptr))
                {
                    return context;
                }
            }

            return DpiAwarenessContext.Unknown;
        }
    }

    private static readonly DpiAwarenessContext[] m_values =
    [
        DpiAwarenessContext.Unaware,
        DpiAwarenessContext.System,
        DpiAwarenessContext.PerMonitor,
        DpiAwarenessContext.PerMonitorV2,
        DpiAwarenessContext.GdiScaled
    ];
}