using System;

namespace PlainCEETimer.Interop;

public struct DpiAwarenessContextHandle(IntPtr ptr)
{
    public readonly DpiAwarenessContext Value
    {
        get
        {
            foreach (var context in s_values)
            {
                if (Win32UI.AreDpiAwarenessContextsEqual(context, ptr))
                {
                    return context;
                }
            }

            return DpiAwarenessContext.Unknown;
        }
    }

    private static readonly DpiAwarenessContext[] s_values =
    [
        DpiAwarenessContext.Unaware,
        DpiAwarenessContext.System,
        DpiAwarenessContext.PerMonitor,
        DpiAwarenessContext.PerMonitorV2,
        DpiAwarenessContext.GdiScaled
    ];
}