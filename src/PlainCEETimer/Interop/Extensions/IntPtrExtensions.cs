using System;
using System.Drawing;
using System.Windows.Forms;

namespace PlainCEETimer.Interop.Extensions;

public static class IntPtrExtensions
{
    extension(nint value)
    {
        public byte LoByte => unchecked((byte)value);

        public byte HiByte => unchecked((byte)(value >> 8));

        public int LoWord => unchecked((ushort)value);

        public int HiWord => unchecked((ushort)(value >> 16));

        public static nint MakeWord(byte low, byte high)
        {
            return (high << 8) | low;
        }

        public static nint MakeLong(int low, int high)
        {
            return (nint)(uint)(((ushort)high << 16) | (ushort)low);
        }
    }

    public static Point AsMenuLocation(this nint lParam)
    {
        if (lParam == -1)
        {
            return Cursor.Position;
        }

        return new((int)lParam);
    }

    public static DpiAwarenessContextHandle AsDpiAwarenessContextHandle(this IntPtr ptr)
    {
        return new(ptr);
    }

    public unsafe static NativeStringUni AsStringUni(this IntPtr ptr, int length = -1, bool free = true)
    {
        return new((char*)ptr, length, free);
    }
}