using System;
using System.Collections;
using System.Collections.Generic;

namespace PlainCEETimer.Modules;

public sealed class EfficientInvocationList<T> : IEnumerable<EventHandler<T>>
{
    public int Count => _list.Count;

    private readonly object SyncRoot = new();
    private readonly List<EventHandler<T>> _list;

    public EfficientInvocationList()
    {
        _list = [];
    }

    public EfficientInvocationList(int capacity)
    {
        _list = new(capacity);
    }

    public void Add(EventHandler<T> item)
    {
        if (item != null)
        {
            lock (SyncRoot)
            {
                _list.Add(item);
            }
        }
    }

    public void Remove(EventHandler<T> item)
    {
        if (item != null)
        {
            lock (SyncRoot)
            {
                _list.Remove(item);
            }
        }
    }

    public void Invoke(object sender, T args)
    {
        var length = _list.Count;

        for (int i = 0; i < length; i++)
        {
            _list[i](sender, args);
        }
    }

    public IEnumerator<EventHandler<T>> GetEnumerator()
    {
        return _list.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
