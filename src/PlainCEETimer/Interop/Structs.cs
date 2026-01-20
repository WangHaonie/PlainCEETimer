using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using PlainCEETimer.Modules.Extensions;
using PlainCEETimer.UI;

namespace PlainCEETimer.Interop;

[DebuggerDisplay("{DebuggerDisplay}")]
public readonly struct COLORREF
{
    public const int EmptyValue = 0xFFFFFF;

    private readonly int value;

    private COLORREF(Color color)
    {
        value = color.ToWin32();
    }

    public Color ToColor()
    {
        return value.ToColor();
    }

    public static implicit operator COLORREF(Color c)
    {
        return new(c);
    }

    private string DebuggerDisplay
    {
        get
        {
            var color = value.ToColor();
            return $"RGB({color.R}, {color.G}, {color.B})";
        }
    }
}

public readonly struct LPCUSTCOLORS : IDisposable
{
    private readonly IntPtr value;

    private LPCUSTCOLORS(int[] colors)
    {
        value = Marshal.AllocHGlobal(16 * sizeof(int));
        Marshal.Copy(colors, 0, value, 16);
    }

    public readonly void Populate(int[] colors)
    {
        Marshal.Copy(value, colors, 0, 16);
    }

    public void Dispose()
    {
        Marshal.FreeHGlobal(value);
    }

    public static explicit operator LPCUSTCOLORS(int[] colors)
    {
        return new(colors);
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
    private readonly IntPtr value;

    public Icon ToIcon()
    {
        var result = Icon.FromHandle(value).Copy();
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