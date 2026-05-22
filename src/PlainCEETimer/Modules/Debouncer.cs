using System;
using System.Threading;
using PlainCEETimer.Modules.Extensions;

namespace PlainCEETimer.Modules;

public class Debouncer : IDisposable
{
    private object[] m_args;
    private readonly int m_delay;
    private readonly Delegate m_method;
    private readonly Timer m_timer;
    private readonly SynchronizationContext m_context;
    private readonly object syncLock;

    public Debouncer(Delegate method, int delay = 500)
    {
        m_method = method ?? throw new ArgumentNullException(nameof(method));
        m_delay = delay;
        m_context = SynchronizationContext.Current;
        m_timer = new(DoAction, null, Timeout.Infinite, Timeout.Infinite);
        syncLock = new();
    }

    public void Debounce(params object[] args)
    {
        lock (syncLock)
        {
            m_args = args;
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
        object[] args;

        lock (syncLock)
        {
            args = m_args;
        }

        if (m_context == null)
        {
            m_method.DynamicInvoke(args);
        }
        else
        {
            m_context.Post(_ => m_method.DynamicInvoke(args), null);
        }
    }

    ~Debouncer()
    {
        Dispose();
    }
}
