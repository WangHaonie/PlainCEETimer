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
    private readonly object syncLock;

    public Debouncer(Delegate method, int delay = 500)
    {
        m_method = method ?? throw new ArgumentNullException(nameof(method));
        m_delay = delay;
        m_timer = new(TimerCallback, null, Timeout.Infinite, Timeout.Infinite);
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

    private void TimerCallback(object state)
    {
        object[] args;

        lock (syncLock)
        {
            args = m_args;
        }

        SafeExecutionContext.Execute(_ => m_method.DynamicInvoke(args));
    }

    ~Debouncer()
    {
        Dispose();
    }
}
