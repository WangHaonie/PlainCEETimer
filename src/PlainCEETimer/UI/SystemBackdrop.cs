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

    public uint Tint
    {
        get => m_tint;
        set
        {
            if (m_tint != value)
            {
                m_tint = value;

                if (applied)
                {
                    Apply();
                }
            }
        }
    }

    public static bool BasicSupported => s_supported;

    public static int SuggestedBackdrop => s_suggested;

    public const uint TintAlpha = 0x20000000;
    public const uint DefaultTint = TintAlpha | 0x202020;

    private int m_value;
    private uint m_tint = DefaultTint;
    private bool applied;
    private readonly IntPtr m_hWnd;
    private static readonly int s_suggested;
    private static readonly bool s_supported;

    public SystemBackdrop(IntPtr hWnd, int type, uint tint)
    {
        if (hWnd != IntPtr.Zero && Win32UI.IsWindow(hWnd))
        {
            m_hWnd = hWnd;
            m_value = type;
            m_tint = tint;
        }
    }

    static SystemBackdrop()
    {
        s_suggested = SystemVersion.IsWindows11 ? Natives.ASBT_ACRYLIC : Natives.ASBT_AERO;
        s_supported = SystemVersion.Current.AtLeast(WindowsVersions.Windows10_1903);
    }

    public bool Apply()
    {
        applied = ApplyCore(m_hWnd, true, m_value);
        return applied;
    }

    public bool Clear()
    {
        if (ApplyCore(m_hWnd, false, m_value))
        {
            m_value = Natives.ASBT_NONE;
            applied = false;
            return true;
        }

        return false;
    }

    private unsafe bool ApplyCore(IntPtr hWnd, bool enabled, int type)
    {
        if (BasicSupported && (!enabled || SystemSettings.EnableTransparency.Value))
        {
            int flags = (enabled.ToWin32() << 8) | (Natives.ASBF_ACCENT_POLICY << 4) | (type & 0xF);
            uint tint = m_tint;
            return Win32UI.PnApplySystemBackdrop(hWnd, flags, &tint);
        }

        return false;
    }
}
