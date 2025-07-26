using System;
using System.Drawing;
using System.Runtime.InteropServices;
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

    public delegate IntPtr HOOKPROC(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

    public readonly struct BOOL(bool value)
    {
        private readonly int Value = value ? 1 : 0;

        public static readonly BOOL TRUE = new(true);
        public static readonly BOOL FALSE = new(false);

        public static implicit operator bool(BOOL b)
        {
            return b.Value != 0;
        }

        public static implicit operator IntPtr(BOOL b)
        {
            return new(b.Value);
        }

        public static implicit operator BOOL(bool b)
        {
            return new(b);
        }
    }

    public readonly struct HWND(IntPtr value)
    {
        private readonly IntPtr Value = value;

        public static implicit operator HWND(IntPtr ptr)
        {
            return new(ptr);
        }

        public static implicit operator bool(HWND hWnd)
        {
            return hWnd.Value != IntPtr.Zero;
        }

        public IntPtr ToIntPtr()
        {
            return Value;
        }
    }

    public readonly struct COLORREF(Color color)
    {
        private readonly int Value = ColorTranslator.ToWin32(color);

        public static implicit operator COLORREF(Color c)
        {
            return new(c);
        }
    }

    public readonly struct HDC(IntPtr value)
    {
        private readonly IntPtr Value = value;

        public static implicit operator bool(HDC HDC)
        {
            return HDC.Value != IntPtr.Zero;
        }

        public IntPtr ToIntPtr()
        {
            return Value;
        }
    }

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

    [StructLayout(LayoutKind.Sequential)]
    public struct NMHDR
    {
        public IntPtr hWndFrom;
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
}
