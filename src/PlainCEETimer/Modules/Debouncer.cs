using System;
using System.Threading;
using PlainCEETimer.Modules.Extensions;

namespace PlainCEETimer.Modules;

public class Debouncer : IDisposable
{
    private IDebounceState m_state;
    private readonly long m_delay;
    private readonly Timer m_timer;
    private readonly object syncLock;

    public Debouncer(long delay = 500L)
    {
        m_delay = delay;
        m_timer = new(TimerCallback, null, Timeout.Infinite, Timeout.Infinite);
        syncLock = new();
    }

    public void Debounce(IDebounceState state)
    {
        lock (syncLock)
        {
            m_state = state;
            m_timer.Change(m_delay, Timeout.Infinite);
        }
    }

    public void Debounce<T>(IDebounceState<T> state, T arg)
    {
        state.Update(arg);
        Debounce(state);
    }

    public void Dispose()
    {
        m_timer.Destroy();
        GC.SuppressFinalize(this);
    }

    private void TimerCallback(object state)
    {
        IDebounceState s;

        lock (syncLock)
        {
            s = m_state;
            m_state = null;
        }

        SafeExecutionContext.Execute(_ => s.Invoke());
    }

    ~Debouncer()
    {
        Dispose();
    }
}
