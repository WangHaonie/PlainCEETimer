using System;

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

    private static void TryGetString(char* ptr, int len, out ReadOnlySpan<char> value)
    {
        value = default;

        if (ptr == null || (nuint)ptr < 0x10000)
        {
            return;
        }

        try
        {
            if (len < 0)
            {
                len = 0;

                while (ptr[len] != char.MinValue)
                {
                    len++;
                }
            }

            value = new ReadOnlySpan<char>(ptr, len);
            return;
        }
        catch
        {
            return;
        }
    }
}
