using System;
using System.Threading;
using PlainCEETimer.Modules.Annotations.Fody;
using PlainCEETimer.Modules.Extensions;
using UITimer = System.Windows.Forms.Timer;

namespace PlainCEETimer.Modules;

[NoConstants]
public class Debouncer : IDisposable
{
    private IActionInvoker m_invoker;
    private readonly bool m_uiCritical;
    private readonly long m_delay;
    private readonly object syncLock;
    private readonly IDebounceState m_state;
    private readonly Timer m_thTimer;
    private readonly UITimer m_uiTimer;

    private const long DefaultDelay = 500L;

    public Debouncer(long delay = DefaultDelay, bool uiCritical = false)
    {
        if (uiCritical)
        {
            m_uiTimer = new() { Interval = (int)delay };
            m_uiTimer.Tick += Timer_Tick;
        }
        else
        {
            m_thTimer = new(TimerCallback, null, Timeout.Infinite, Timeout.Infinite);
        }

        syncLock = new();
        m_delay = delay;
        m_uiCritical = uiCritical;
    }

    public Debouncer(IDebounceState state, long delay = DefaultDelay, bool uiCritical = false) : this(delay, uiCritical)
    {
        m_state = state;
    }

    public void Debounce(IActionInvoker invoker)
    {
        lock (syncLock)
        {
            if (m_state?.ShouldDebounce == false)
            {
                SafeExecutionContext.Send(invoker);
            }
            else
            {
                m_invoker = invoker;

                if (m_uiCritical)
                {
                    m_uiTimer.Stop();
                    m_uiTimer.Start();
                }
                else
                {
                    m_thTimer.Change(m_delay, Timeout.Infinite);
                }
            }
        }
    }

    public void Dispose()
    {
        m_uiTimer.Destroy();
        m_thTimer.Destroy();
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

    private void Timer_Tick(object sender, EventArgs e)
    {
        IActionInvoker invoker = null;

        lock (syncLock)
        {
            m_uiTimer.Stop();
            invoker = m_invoker;
        }

        invoker.Invoke(null);
    }

    ~Debouncer()
    {
        Dispose();
    }
}
