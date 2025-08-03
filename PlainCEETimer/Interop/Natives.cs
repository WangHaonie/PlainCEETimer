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
    public enum HOTKEYF : byte
    {
        NONE = 0x00,
        SHIFT = 0x01,
        CONTROL = 0x02,
        ALT = 0x04,
        EXT = 0x08
    }

    public enum SWCMD
    {
        NORMAL = 1,
        MAXIMIZE = 3,
        MINIMIZE = 7
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

    [DebuggerDisplay("{Value}")]
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
    public readonly struct SHKEY(HOTKEYF fKeys, Keys keys)
    {
        private readonly ushort Value = (ushort)(((byte)fKeys << 8) | ((byte)((int)keys & 0xFF)));

        public static readonly SHKEY NONE = new();

        private string DebuggerDisplay => $"{(HOTKEYF)((Value >> 8) & 0xFF)} | {(Keys)(Value & 0xFF)}";
    }

    [DebuggerDisplay("{pszFile,nq} {pszArgs,nq}")]
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct SHLNKINFO(string lnkPath)
    {
        public string pszLnkPath = lnkPath;
        public string pszFile;
        public string pszArgs;
        public string pszWorkDir;
        public SHKEY wHotkey;
        public SWCMD iShowCmd;
        public string pszDescr;
        public string pszIconPath;
        public int iIcon;
    }
}
