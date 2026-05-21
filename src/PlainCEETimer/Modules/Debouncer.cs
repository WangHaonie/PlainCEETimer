using System;
using System.Threading;
using PlainCEETimer.Modules.Extensions;

namespace PlainCEETimer.Modules;

public class Debouncer<T> : IDisposable
{
    private T m_value;
    private readonly int m_delay;
    private readonly Action<T> m_action;
    private readonly Timer m_timer;
    private readonly SynchronizationContext m_context;
    private readonly object syncLock;

    public Debouncer(Action<T> action, int delay = 500)
    {
        m_action = action ?? throw new ArgumentNullException(nameof(action));
        m_delay = delay;
        m_context = SynchronizationContext.Current;
        m_timer = new(DoAction, null, Timeout.Infinite, Timeout.Infinite);
        syncLock = new();
    }

    public void Debounce(T value)
    {
        lock (syncLock)
        {
            m_value = value;
            m_timer.Change(m_delay, Timeout.Infinite);
        }
    }

    public void Dispose()
    {
        m_timer.Destroy();
        GC.SuppressFinalize(this);
    }

    private void DoAction(object state)
    {
        T value;

        lock (syncLock)
        {
            value = m_value;
        }

        if (m_context == null)
        {
            m_action(value);
        }
        else
        {
            m_context.Post(_ => m_action(value), null);
        }
    }

    ~Debouncer()
    {
        Dispose();
    }
}
