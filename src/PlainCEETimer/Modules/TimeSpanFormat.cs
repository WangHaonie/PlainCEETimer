using System;
using System.Collections.Generic;
using PlainCEETimer.Interop;

namespace PlainCEETimer.Modules;

public static class TimeSpanFormat
{
    public unsafe static string DefaultFormat
    {
        get
        {
            if (field == null)
            {
                var length = Win32.LoadStringInternal(201, out var str);
                field = length > 0 ? new((char*)str, 0, length) : string.Empty;
            }

            return field;
        }
    }

    private readonly static Dictionary<StringHashCodeProvider, IntPtr> m_cache;

    static TimeSpanFormat()
    {
        m_cache = [];
        App.Current.AppExit += Cleanup;
    }

    public static bool ValidateFormat(string format)
    {
        return !string.IsNullOrEmpty(format)
            && Win32Controls.PlainTimeSpanPick_ValidateFormat(format);
    }

    public unsafe static string Format(TimeSpan value, string format)
    {
        var hFormat = GetParsedFormat(format);

        if (hFormat == IntPtr.Zero)
        {
            return string.Empty;
        }

        int count = 0;

        if (Win32Controls.PlainTimeSpanPick_Format(hFormat, ref value, null, ref count))
        {
            string result = StringInternals.FastAllocateString(count++);

            fixed (char* lpBuffer = result)
            {
                if (Win32Controls.PlainTimeSpanPick_Format(hFormat, ref value, lpBuffer, ref count))
                {
                    return result;
                }
            }
        }

        return string.Empty;
    }

    private static IntPtr GetParsedFormat(string format)
    {
        if (!m_cache.TryGetValue(format, out var hFormat))
        {
            var h = Win32Controls.PlainTimeSpanPick_ParseFormat(format);

            if (h == IntPtr.Zero)
            {
                return h;
            }

            m_cache[format] = h;
            hFormat = h;
        }

        return hFormat;
    }

    private static void Cleanup()
    {
        foreach (var handle in m_cache.Values)
        {
            Win32Controls.PlainTimeSpanPick_FreeMemory(handle);
        }
    }
}
