using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using PlainCEETimer.Modules.Extensions;

namespace PlainCEETimer.Interop;

[DebuggerDisplay("{(Value == 0 ? false : true)}")]
public readonly struct BOOL(int value)
{
    private readonly int Value = value;

    public static readonly BOOL TRUE = new(1);
    public static readonly BOOL FALSE = new(0);

    public static implicit operator int(BOOL b)
    {
        return b.Value;
    }

    public static implicit operator bool(BOOL b)
    {
        return b.Value != 0;
    }

    public static explicit operator BOOL(bool b)
    {
        return b ? TRUE : FALSE;
    }

    public static implicit operator IntPtr(BOOL b)
    {
        return new(b.Value);
    }
}

[DebuggerDisplay("{Value}")]
public readonly struct HWND
{
    private readonly IntPtr Value;

    private HWND(IntPtr value)
    {
        Value = value;
    }

    public static implicit operator bool(HWND hWnd)
    {
        return hWnd.Value != IntPtr.Zero;
    }

    public static implicit operator HWND(IntPtr ptr)
    {
        return new(ptr);
    }

    public static explicit operator IntPtr(HWND hWnd)
    {
        return hWnd.Value;
    }
}

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

public readonly struct CUSTCOLORS : IDisposable
{
    private readonly IntPtr Value;

    public CUSTCOLORS(int[] colors)
    {
        var ptr = Marshal.AllocHGlobal(16 * 4);
        Marshal.Copy(colors, 0, ptr, 16);
        Value = ptr;
    }

    public readonly void ToArray(int[] colors)
    {
        Marshal.Copy(Value, colors, 0, 16);
    }

    public void Dispose()
    {
        Marshal.FreeHGlobal(Value);
    }
}

[DebuggerDisplay("{Value}")]
public readonly struct HDC
{
    private readonly IntPtr Value;

    private HDC(IntPtr value)
    {
        Value = value;
    }

    public static implicit operator bool(HDC hDC)
    {
        return hDC.Value != IntPtr.Zero;
    }

    public static implicit operator HDC(IntPtr ptr)
    {
        return new(ptr);
    }

    public static explicit operator IntPtr(HDC hDC)
    {
        return hDC.Value;
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
        var result = (Icon)Icon.FromHandle(Value).Clone();
        Win32UI.DestroyIcon(Value);
        return result;
    }

    public static Icon ExtractIcon(string file, int index = 0)
    {
        Win32UI.ExtractIconEx(file, index, out var hIcon, default, 1);
        return hIcon.ToIcon();
    }
}

[DebuggerDisplay("{DebuggerDisplay}")]
public readonly struct LnkHotkey(HotkeyModifier fKeys, Keys keys)
{
    private readonly ushort Value = ushort.MakeWord((byte)keys, (byte)fKeys);

    public static readonly LnkHotkey None;

    private string DebuggerDisplay => $"{(HotkeyModifier)Value.HiByte}, {(Keys)Value.LoByte}";
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
    public LnkHotkey Hotkey;
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
