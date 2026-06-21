using System;
using PlainCEETimer.Modules.Extensions;

namespace PlainCEETimer.Interop;

public unsafe readonly ref struct NativeStringUni
{
    private readonly ReadOnlySpan<char> m_value;

    public NativeStringUni(char* ptr, int length = -1)
    {
        TryGetString(ptr, length, out m_value);
    }

    public bool Equals(string str)
    {
        if (str == null || m_value.IsEmpty)
        {
            return false;
        }

        return m_value.SequenceEqual(str.AsSpan());
    }

    public bool Equals(NativeStringUni str)
    {
        if (m_value.IsEmpty || str.m_value.IsEmpty)
        {
            return false;
        }

        if (m_value == str.m_value)
        {
            return true;
        }

        return m_value.SequenceEqual(str.m_value);
    }

    public override bool Equals(object obj)
    {
        return Equals(obj as string);
    }

    public override int GetHashCode()
    {
        return m_value.GetHashCode();
    }

    public override string ToString()
    {
        return m_value.ToString();
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

    private static void TryGetString(char* ptr, int length, out ReadOnlySpan<char> value)
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
