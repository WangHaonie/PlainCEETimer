using System;
using System.Buffers;

namespace PlainCEETimer.Modules;

public class ArrayCache<T> : IDisposable
{
    private readonly T[] m_array;
    private readonly ArrayPool<T> m_pool;

    public ArrayCache(int minimumLength, out T[] buffer)
    {
        m_pool = ArrayPool<T>.Shared;
        m_array = m_pool.Rent(minimumLength);
        buffer = m_array;
    }

    public void Dispose()
    {
        m_pool.Return(m_array);
        GC.SuppressFinalize(this);
    }
}