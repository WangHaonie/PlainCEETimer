using System;
using PlainCEETimer.Interop;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Extensions;

namespace PlainCEETimer.UI;

public class SystemBackdrop
{
    public int BackdropType
    {
        get => m_value;
        set
        {
            if (m_value != value)
            {
                if (value == Natives.ASBT_NONE)
                {
                    Clear();
                }

                m_value = value;

                if (applied)
                {
                    Apply();
                }
            }
        }
    }

    private int m_value;
    private bool applied;
    private readonly IntPtr m_hWnd;

    private static readonly int s_suggested;

    public SystemBackdrop(IntPtr hWnd) : this(hWnd, s_suggested)
    {
        return;
    }

    public SystemBackdrop(IntPtr hWnd, int type)
    {
        if (hWnd != IntPtr.Zero && Win32UI.IsWindow(hWnd))
        {
            m_hWnd = hWnd;
            m_value = type;
        }
    }

    static SystemBackdrop()
    {
        s_suggested = SuggestSBT();
    }

    public bool Apply()
    {
        if (!applied) applied = true;
        return ApplyCore(m_hWnd, true, m_value);
    }

    public bool Clear()
    {
        if (ApplyCore(m_hWnd, false, m_value))
        {
            m_value = Natives.ASBT_NONE;
            return true;
        }

        return false;
    }

    private static int SuggestSBT()
    {
        if (SystemVersion.IsWindows11)
        {
            return Natives.ASBT_ACRYLIC;
        }

        return Natives.ASBT_NONE;
    }

    private unsafe static bool ApplyCore(IntPtr hWnd, bool enabled, int type)
    {
        if (!SystemVersion.IsWindows11 || !ThemeManager.TransparencyEnabled)
        {
            return false; // 非 W11 之后有时间了再研究
        }

        int flags = (enabled.ToWin32() << 8) | 0b00000100 << 4 | (type & 0xF);
        return Win32UI.ApplySystemBackdrop(hWnd, flags, null);
    }
}
