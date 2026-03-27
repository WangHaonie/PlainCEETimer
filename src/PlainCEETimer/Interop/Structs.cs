using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Newtonsoft.Json;
using PlainCEETimer.Modules.Extensions;
using PlainCEETimer.Modules.Fody;
using PlainCEETimer.Modules.JsonConverters;
using PlainCEETimer.UI;

namespace PlainCEETimer.Interop;

[NoConstants]
[DebuggerDisplay("{Left}, {Top}, {Right}, {Bottom}")]
[StructLayout(LayoutKind.Sequential)]
public struct RECT
{
    public int Left;
    public int Top;
    public int Right;
    public int Bottom;

    /// <summary>
    /// <code>LONG <see cref="left"/></code>
    /// </summary>
    public const int left = 0;

    /// <summary>
    /// <code>LONG <see cref="top"/></code>
    /// </summary>
    public const int top = 4;

    /// <summary>
    /// <code>LONG <see cref="right"/></code>
    /// </summary>
    public const int right = 8;

    /// <summary>
    /// <code>LONG <see cref="bottom"/></code>
    /// </summary>
    public const int bottom = 16;

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

    private static ushort MakeValue(HotkeyF fKeys, Keys key)
    {
        return ushort.MakeWord((byte)key, (byte)fKeys);
    }
}

[NoConstants]
[DebuggerDisplay("{DebuggerDisplay}")]
[JsonConverter(typeof(Win32ColorFormatConverter))]
public readonly struct COLORREF : IEquatable<COLORREF>
{
    public const int EmptyValue = 0x00FFFFFF;
    public const int LPCUSTCOLORS_Length = 16;

    private readonly int value;

    private COLORREF(int i)
    {
        value = i & EmptyValue;
    }

    private COLORREF(Color color) : this(color.ToWin32())
    {
        return;
    }

    public static implicit operator COLORREF(Color c)
    {
        return new(c);
    }

    public static implicit operator int(COLORREF cr)
    {
        return cr.value;
    }

    public static explicit operator COLORREF(int i)
    {
        return new(i);
    }

    public static explicit operator Color(COLORREF cr)
    {
        return cr.value.ToColor();
    }

    private string DebuggerDisplay
    {
        get
        {
            var color = value.ToColor();
            return $"RGB({color.R}, {color.G}, {color.B})";
        }
    }

    public bool Equals(COLORREF other)
    {
        return value == other.value;
    }

    public override bool Equals(object obj)
    {
        if (obj is COLORREF cr)
        {
            return Equals(cr);
        }

        return false;
    }

    public override int GetHashCode()
    {
        return value;
    }

    public static bool operator ==(COLORREF left, COLORREF right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(COLORREF left, COLORREF right)
    {
        return !(left == right);
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
        var sb = new StringBuilder(64)
            .Append(Index + 1)
            .Append(". ");

        var appendComma = false;

        if (!string.IsNullOrWhiteSpace(Name))
        {
            sb.Append(Name);
            appendComma = true;
        }

        if (!string.IsNullOrWhiteSpace(Id))
        {
            var id = GetId(Id);

            if (!string.IsNullOrWhiteSpace(id))
            {
                var appendSpace = false;

                if (appendComma)
                {
                    appendSpace = true;
                }

                appendComma = true;

                if (appendSpace)
                {
                    sb.Append(' ');
                }

                sb.Append(id);
            }
        }

        if (appendComma)
        {
            sb.Append(", ");
        }

        if (!string.IsNullOrWhiteSpace(Path))
        {
            sb.Append(Path)
              .Append(", ");
        }

        sb.Append(Bounds.Width)
          .Append('x')
          .Append(Bounds.Height)
          .Append(", ")
          .Append(RefreshRate.Format())
          .Append(" Hz");

        return sb.ToString();
    }

    private static string GetId(string did)
    {
        if (did != null)
        {
            var dids = did.Split('\\');
            var iname = did;

            if (dids.Length > 2)
            {
                iname = dids[1];
            }

            return iname;
        }

        return string.Empty;
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
    public int ShowCmd;
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
        font?.ToLogFont(lfobj);
        return (LOGFONT)lfobj;
    }
}