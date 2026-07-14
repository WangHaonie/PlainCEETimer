using System;

namespace PlainCEETimer.Interop;

internal unsafe ref struct FastStringBuffer(char* buffer, int length)
{
    private int m_length;
    private readonly Span<char> m_buffer = new(buffer, length);

    public int Append(ReadOnlySpan<char> value)
    {
        if (value.IsEmpty)
        {
            return 0;
        }

        var remain = length - m_length;

        if (remain <= 0)
        {
            return 0;
        }

        var len = Math.Min(remain, value.Length);
        value.Slice(0, len).CopyTo(m_buffer.Slice(m_length));
        m_length += len;
        return len;
    }

    public bool Append(char value)
    {
        if (m_length < length)
        {
            buffer[m_length] = value;
            m_length++;
            return true;
        }

        return false;
    }
}
