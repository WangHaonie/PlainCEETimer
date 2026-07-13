using System;
using System.Runtime.InteropServices;
using PlainCEETimer.Modules.Extensions;

namespace PlainCEETimer.Interop;

public ref struct NativeStringUni
{
    private bool disposed;
    private string m_str;
    private readonly IntPtr m_ptr;
    private readonly ReadOnlySpan<char> m_value;

    public unsafe NativeStringUni(char* ptr, int length = -1, bool free = true)
    {
        m_ptr = free ? new(ptr) : IntPtr.Zero;
        TryGetString(ptr, length, out m_value);
    }

    public readonly bool Equals(string str)
    {
        if (str == null || m_value.IsEmpty)
        {
            return false;
        }

        if (disposed)
        {
            return m_str == str;
        }

        return m_value.SequenceEqual(str.AsSpan());
    }

    public readonly bool Equals(NativeStringUni str)
    {
        if (m_value.IsEmpty || str.m_value.IsEmpty)
        {
            return false;
        }

        if (disposed)
        {
            if (str.disposed)
            {
                return m_str == str.m_str;
            }

            return str.Equals(m_str);
        }

        if (str.disposed)
        {
            return Equals(str.m_str);
        }

        return m_value.SequenceEqual(str.m_value);
    }

    public void Dispose()
    {
        Marshal.FreeCoTaskMem(m_ptr);
        disposed = true;
    }

    public override readonly bool Equals(object obj)
    {
        return Equals(obj as string);
    }

    public override readonly int GetHashCode()
    {
        return m_ptr.GetHashCode();
    }

    public override string ToString()
    {
        if (!disposed)
        {
            m_str = m_value.ToString();
            Dispose();
            disposed = true;
        }

        return m_str;
    }

    public static bool operator ==(NativeStringUni a, string b)
    {
        return a.Equals(b);
    }

    public static bool operator !=(NativeStringUni a, string b)
    {
        return !(a == b);
    }

    public static bool operator ==(NativeStringUni a, NativeStringUni b)
    {
        return a.Equals(b);
    }

    public static bool operator !=(NativeStringUni a, NativeStringUni b)
    {
        return !(a == b);
    }

    private unsafe static void TryGetString(char* ptr, int length, out ReadOnlySpan<char> value)
    {
        value = default;

        if (ptr == null)
        {
            return;
        }

        try
        {
            var n = Win32.lstrlenW(ptr);

            if (n > 0)
            {
                if (length <= 0)
                {
                    length = n;
                }
                else
                {
                    length = length.Clamp(0, n);
                }

                value = new ReadOnlySpan<char>(ptr, length);
            }
        }
        catch { }
    }
}
