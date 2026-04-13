using System;
using System.Buffers;

namespace PlainCEETimer.Modules;

public class ArrayCache<T> : IDisposable
{
    public T[] Value => m_array;

    private readonly T[] m_array;
    private readonly ArrayPool<T> m_pool;

    public ArrayCache(int minimumLength)
    {
        m_pool = ArrayPool<T>.Shared;
        m_array = m_pool.Rent(minimumLength);
    }

    public void Dispose()
    {
        m_pool.Return(m_array);
        GC.SuppressFinalize(this);
    }
}