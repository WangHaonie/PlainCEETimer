using System;
using System.Collections.Generic;
using PlainCEETimer.Interop;

namespace PlainCEETimer.Modules;

public static class TimeSpanFormat
{
    private readonly static Dictionary<StringHashCodeProvider, IntPtr> m_cache;

    static TimeSpanFormat()
    {
        m_cache = [];
        App.Current.AppExit += Cleanup;
    }

    public unsafe static string Format(TimeSpan value, string format)
    {
        if (!m_cache.TryGetValue(format, out IntPtr hFormat))
        {
            var h = Win32Controls.PlainTimeSpanPick_ParseFormat(format);

            if (h == IntPtr.Zero)
            {
                return string.Empty;
            }

            m_cache[format] = h;
            hFormat = h;
        }

        int count = 0;
        long ticks = value.Ticks;

        if (Win32Controls.PlainTimeSpanPick_Format(hFormat, ref ticks, null, ref count))
        {
            string result = StringInternals.FastAllocateString(count);
            ++count;

            fixed (char* lpBuffer = result)
            {
                if (Win32Controls.PlainTimeSpanPick_Format(hFormat, ref ticks, lpBuffer, ref count))
                {
                    return result;
                }
            }
        }

        return string.Empty;
    }

    private static void Cleanup()
    {
        foreach (var handle in m_cache.Values)
        {
            Win32Controls.PlainTimeSpanPick_FreeMemory(handle);
        }
    }
}
