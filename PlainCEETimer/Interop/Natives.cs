using System;
using System.Drawing;
using System.Runtime.InteropServices;
using PlainCEETimer.Modules;

namespace PlainCEETimer.Interop
{
    public static class Natives
    {
        [DllImport(App.User32Dll)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);
    }

    public delegate IntPtr HOOKPROC(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

    public struct BOOL(bool value)
    {
        public int Value = value ? 1 : 0;

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

    public struct COLORREF(Color color)
    {
        public int Value = ColorTranslator.ToWin32(color);

        public static implicit operator COLORREF(Color c)
        {
            return new(c);
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
}
