using System;

namespace PlainCEETimer.Interop;

public unsafe readonly ref struct NativeStringUni
{
    private readonly bool m_isValid;
    private readonly ReadOnlySpan<char> m_value;

    public NativeStringUni(char* ptr)
    {
        m_isValid = TryGetString(ptr, out m_value);
    }

    public bool Equals(string str)
    {
        if (str == null || !m_isValid)
        {
            return false;
        }

        return m_value.SequenceEqual(str.AsSpan());
    }

    public bool Equals(NativeStringUni str)
    {
        if (!m_isValid || !str.m_isValid)
        {
            return false;
        }

        if (m_value == str.m_value)
        {
            return true;
        }

        return m_value.SequenceEqual(str.m_value);
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

    private static bool TryGetString(char* ptr, out ReadOnlySpan<char> value)
    {
        value = default;

        if (ptr == null || (nuint)ptr < 0x10000)
        {
            return false;
        }

        try
        {
            var len = 0;

            while (ptr[len] != '\0')
            {
                len++;
            }

            value = new ReadOnlySpan<char>(ptr, len);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public override bool Equals(object obj)
    {
        return Equals(obj as string);
    }

    public override int GetHashCode()
    {
        return m_value.GetHashCode();
    }
}
