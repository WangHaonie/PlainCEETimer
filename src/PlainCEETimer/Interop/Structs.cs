using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Newtonsoft.Json;
using PlainCEETimer.Modules.Extensions;
using PlainCEETimer.Modules.JsonConverters;

namespace PlainCEETimer.Interop;

[DebuggerDisplay("{DebuggerDisplay}")]
public readonly struct COLORREF
{
    public const int EmptyValue = 0xFFFFFF;

    private readonly int Value;

    private COLORREF(Color color)
    {
        Value = color.ToWin32();
    }

    public Color ToColor()
    {
        return Value.ToColor();
    }

    public static implicit operator COLORREF(Color c)
    {
        return new(c);
    }

    private string DebuggerDisplay
    {
        get
        {
            var color = Value.ToColor();
            return $"RGB({color.R}, {color.G}, {color.B})";
        }
    }
}

public readonly struct LPCUSTCOLORS : IDisposable
{
    private readonly IntPtr Value;

    public LPCUSTCOLORS(int[] colors)
    {
        Value = Marshal.AllocHGlobal(16 * sizeof(int));
        Marshal.Copy(colors, 0, Value, 16);
    }

    public readonly void Populate(int[] colors)
    {
        Marshal.Copy(Value, colors, 0, 16);
    }

    public void Dispose()
    {
        Marshal.FreeHGlobal(Value);
    }
}

[DebuggerDisplay("{Left}, {Top}, {Right}, {Bottom}")]
[StructLayout(LayoutKind.Sequential)]
public struct RECT
{
    public int Left;
    public int Top;
    public int Right;
    public int Bottom;

    public static implicit operator Rectangle(RECT r)
    {
        return Rectangle.FromLTRB(r.Left, r.Top, r.Right, r.Bottom);
    }
}

public readonly struct HICON
{
    private readonly IntPtr Value;

    public Icon ToIcon()
    {
        var result = Icon.FromHandle(Value).Copy();
        Win32UI.DestroyIcon(this);
        return result;
    }

    public unsafe static HICON FromFile(string file, int index = 0)
    {
        Win32UI.ExtractIconEx(file, index, out var hi, null, 1);
        return hi;
    }
}

[DebuggerDisplay("{Modifiers} | {Key}")]
public readonly struct Hotkey
{
    public HotkeyF Modifiers => (HotkeyF)m_value.HiByte;

    public Keys Key => (Keys)m_value.LoByte;

    private readonly ushort m_value;

    public static readonly Hotkey None;

    public Hotkey(ushort value)
    {
        m_value = value;
    }

    public Hotkey(HotKey hk)
    {
        var hkmod = hk.Modifiers;
        var hmod = HotkeyF.None;

        if ((hkmod & HotKeyModifiers.Alt) == HotKeyModifiers.Alt)
        {
            hmod |= HotkeyF.Alt;
        }

        if ((hkmod & HotKeyModifiers.Ctrl) == HotKeyModifiers.Ctrl)
        {
            hmod |= HotkeyF.Ctrl;
        }

        if ((hkmod & HotKeyModifiers.Shit) == HotKeyModifiers.Shit)
        {
            hmod |= HotkeyF.Shit;
        }

        if ((hkmod & HotKeyModifiers.Win) == HotKeyModifiers.Win)
        {
            hmod |= HotkeyF.Ext;
        }

        m_value = MakeValue(hmod, hk.Key);
    }

    public Hotkey(HotkeyF fKeys, Keys key)
    {
        m_value = MakeValue(fKeys, key);
    }

    public static implicit operator ushort(Hotkey h)
    {
        return h.m_value;
    }

    private readonly ushort MakeValue(HotkeyF fKeys, Keys key)
    {
        return ushort.MakeWord((byte)key, (byte)fKeys);
    }
}

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

[DebuggerDisplay("{ToString(),nq}")]
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
public readonly struct SystemDisplay
{
    private readonly int Index;
    private readonly string Name;
    private readonly string Id;
    private readonly string Path;
    private readonly Rectangle Bounds;
    private readonly double RefreshRate;

    public readonly override string ToString()
    {
        return string.Format("{0}. {1} {2}, {3}, {4}x{5}, {6:0.0} Hz", Index + 1, Name, GetId(Id), Path, Bounds.Width, Bounds.Height, RefreshRate);
    }

    private readonly string GetId(string did)
    {
        var dids = did.Split('\\');
        var iname = did;

        if (dids.Length > 2)
        {
            iname = dids[1];
        }

        return iname;
    }
}

[DebuggerDisplay("{Target,nq} {Args,nq}")]
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
public struct LNKFILEINFO(string lnkPath)
{
    public string LnkPath = lnkPath;
    public string Target;
    public string Args;
    public string WorkingDir;
    public Hotkey Hotkey;
    public ShowWindowCommand ShowCmd;
    public string Description;
    public string IconPath;
    public int IconIndex;
}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Size = 92)]
public struct LOGFONT
{
    public readonly Font ToFont()
    {
        using var tmp = Font.FromLogFont(this);
        return new(tmp.FontFamily, tmp.SizeInPoints, tmp.Style, GraphicsUnit.Point, tmp.GdiCharSet, tmp.GdiVerticalFont);
    }

    public static LOGFONT FromFont(Font font)
    {
        object lfobj = default(LOGFONT);
        font.ToLogFont(lfobj);
        return (LOGFONT)lfobj;
    }
}