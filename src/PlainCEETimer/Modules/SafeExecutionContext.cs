using System;
using System.Threading;

namespace PlainCEETimer.Modules;

public static class SafeExecutionContext
{
    private static int m_tid;
    private static SynchronizationContext m_context;

    public static void Send(IActionInvoker state)
    {
        Send(state.Invoke);
    }

    public static void Send(SendOrPostCallback callback, object state = null)
    {
        Execute(callback, state, false);
    }

    public static void Post(IActionInvoker state)
    {
        Post(state.Invoke);
    }

    public static void Post(SendOrPostCallback callback, object state = null)
    {
        Execute(callback, state);
    }

    internal static void SetContext(SynchronizationContext context)
    {
        if (m_tid == default)
        {
            var tid = Environment.CurrentManagedThreadId;

            if (tid != m_tid)
            {
                m_tid = tid;
                m_context = context;
            }
        }
    }

    private static void Execute(SendOrPostCallback callback, object state = null, bool post = true)
    {
        if (m_context == null)
        {
            callback(state);
        }
        else
        {
            if (post)
            {
                m_context.Post(callback, state);
            }
            else
            {
                m_context.Send(callback, state);
            }
        }
    }
}