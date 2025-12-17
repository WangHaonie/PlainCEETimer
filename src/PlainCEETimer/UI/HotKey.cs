using System;
using System.Diagnostics;
using System.Windows.Forms;
using Newtonsoft.Json;
using PlainCEETimer.Interop;
using PlainCEETimer.Modules.JsonConverters;

namespace PlainCEETimer.UI;

[DebuggerDisplay("{Modifiers} | {Key}")]
[JsonConverter(typeof(HotKeyConverter))]
public readonly struct HotKey : IEquatable<HotKey>
{
    public bool IsValid => m_value != 0;

    public HotKeyModifiers Modifiers { get; }

    public Keys Key { get; }

    private readonly ushort m_value;

    public HotKey(ushort value)
    {
        Modifiers = (HotKeyModifiers)value.LoByte;
        Key = (Keys)value.HiByte;
        m_value = value;
    }

    public HotKey(IntPtr lParam)
    {
        var lparam = lParam.ToInt32();
        var m = (HotKeyModifiers)lparam.LoWord;
        var k = (Keys)(byte)lparam.HiWord;

        Modifiers = m;
        Key = k;
        m_value = MakeValue(m, k);
    }

    public HotKey(Hotkey h)
    {
        var hmod = h.Modifiers;
        var hkmod = HotKeyModifiers.None;
        var k = h.Key;

        if ((hmod & HotkeyF.Shit) == HotkeyF.Shit)
        {
            hkmod |= HotKeyModifiers.Shit;
        }

        if ((hmod & HotkeyF.Ctrl) == HotkeyF.Ctrl)
        {
            hkmod |= HotKeyModifiers.Ctrl;
        }

        if ((hmod & HotkeyF.Alt) == HotkeyF.Alt)
        {
            hkmod |= HotKeyModifiers.Alt;
        }

        if ((hmod & HotkeyF.Ext) == HotkeyF.Ext)
        {
            hkmod |= HotKeyModifiers.Win;
        }

        Modifiers = hkmod;
        Key = k;
        m_value = MakeValue(hkmod, k);
    }

    public HotKey(HotKeyModifiers fsModifiers, Keys vk)
    {
        Modifiers = fsModifiers;
        Key = vk;
        m_value = MakeValue(fsModifiers, vk);
    }

    private readonly ushort MakeValue(HotKeyModifiers fsModifiers, Keys vk)
    {
        return ushort.MakeWord((byte)fsModifiers, (byte)vk);
    }

    public bool Equals(HotKey other)
    {
        return m_value == other.m_value;
    }

    public override bool Equals(object obj)
    {
        return Equals((HotKey)obj);
    }

    public override int GetHashCode()
    {
        return m_value;
    }
}