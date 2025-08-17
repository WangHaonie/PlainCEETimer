using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using PlainCEETimer.Modules;

namespace PlainCEETimer.Interop
{
    public static class Natives
    {
        [DllImport(App.User32Dll)]
        public static extern IntPtr SendMessage(HWND hWnd, int msg, IntPtr wParam, IntPtr lParam);

        [DllImport(App.Gdi32Dll)]
        public static extern COLORREF SetTextColor(HDC hdc, COLORREF color);
    }

    [Flags]
    public enum HotkeyModifiers : byte
    {
        None = 0x00,
        Shit = 0x01, // (●'◡'●)
        Control = 0x02,
        Alt = 0x04,
        Ext = 0x08
    }

    public enum ShowWindowCommand
    {
        Normal = 1,
        Maximize = 3,
        Minimize = 7
    }

    public delegate IntPtr WNDPROC(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

    [DebuggerDisplay("{(Value == 0 ? false : true)}")]
    public readonly struct BOOL
    {
        private readonly int Value;

        public static readonly BOOL TRUE = new(true);
        public static readonly BOOL FALSE = new(false);

        private BOOL(bool value)
        {
            Value = value ? 1 : 0;
        }

        public static implicit operator bool(BOOL b)
        {
            return b.Value != 0;
        }

        public static explicit operator BOOL(bool b)
        {
            return new(b);
        }

        public static implicit operator IntPtr(BOOL b)
        {
            return new(b.Value);
        }
    }

    [DebuggerDisplay("{Value}")]
    public readonly struct HWND
    {
        public static readonly HWND TOPMOST = new(new(-1));

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
        private readonly int Value;

        private COLORREF(Color color)
        {
            Value = ColorTranslator.ToWin32(color);
        }

        public static implicit operator COLORREF(Color c)
        {
            return new(c);
        }

        private string DebuggerDisplay
        {
            get
            {
                var color = ColorTranslator.FromOle(Value);
                return $"RGB({color.R}, {color.G}, {color.B})";
            }
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

    [DebuggerDisplay("{code}")]
    [StructLayout(LayoutKind.Sequential)]
    public struct NMHDR
    {
        public HWND hWndFrom;
        public IntPtr idFrom;
        public int code;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct NMCUSTOMDRAW
    {
        public NMHDR hdr;
        public int dwDrawStage;
        public HDC hdc;
        public RECT rc;
        public IntPtr dwItemSpec;
        public uint uItemState;
        public IntPtr lItemlParam;
    }

    [DebuggerDisplay("{DebuggerDisplay}")]
    public readonly struct LnkHotkey(HotkeyModifiers fKeys, Keys keys)
    {
        private readonly ushort Value = (ushort)(((byte)fKeys << 8) | ((byte)((int)keys & 0xFF)));

        public static readonly LnkHotkey None = new();

        private string DebuggerDisplay => $"{(HotkeyModifiers)((Value >> 8) & 0xFF)}, {(Keys)(Value & 0xFF)}";
    }

    [DebuggerDisplay("{ToString(),nq}")]
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public readonly struct SystemDisplay
    {
        private readonly int index;
        private readonly string deviceName;
        private readonly string deviceId;
        private readonly string dosPath;
        private readonly RECT bounds;
        private readonly double refreshRate;

        public readonly override string ToString()
        {
            return string.Format("{0}. {1} {2}, {3}, {4}x{5}, {6:0.0} Hz", index + 1, deviceName, GetId(deviceId), dosPath, bounds.Right - bounds.Left, bounds.Bottom - bounds.Top, refreshRate);
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
    public struct Win32Lnk(string lnkPath)
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
}
