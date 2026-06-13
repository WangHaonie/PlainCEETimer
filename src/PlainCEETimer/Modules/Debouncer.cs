using System;
using System.Threading;
using PlainCEETimer.Modules.Extensions;

namespace PlainCEETimer.Modules;

public class Debouncer : IDisposable
{
    private IActionInvoker m_invoker;
    private readonly long m_delay;
    private readonly Timer m_timer;
    private readonly object syncLock;

    public Debouncer(long delay = 500L)
    {
        m_delay = delay;
        m_timer = new(TimerCallback, null, Timeout.Infinite, Timeout.Infinite);
        syncLock = new();
    }

    public void Debounce(IActionInvoker invoker)
    {
        lock (syncLock)
        {
            m_invoker = invoker;
            m_timer.Change(m_delay, Timeout.Infinite);
        }
    }

    public void Dispose()
    {
        m_timer.Destroy();
        GC.SuppressFinalize(this);
    }

    private void TimerCallback(object state)
    {
        IActionInvoker invoker;

        lock (syncLock)
        {
            invoker = m_invoker;
        }

        SafeExecutionContext.Post(invoker.Invoke, state);
    }

    ~Debouncer()
    {
        Dispose();
    }
}
