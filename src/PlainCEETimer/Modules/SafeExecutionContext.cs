using System;
using System.Threading;

namespace PlainCEETimer.Modules;

public static class SafeExecutionContext
{
    private static int m_tid;
    private static SynchronizationContext m_context;

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

    public static void Execute(SendOrPostCallback callback, object state = null)
    {
        if (m_context == null)
        {
            callback(state);
        }
        else
        {
            m_context.Post(callback, state);
        }
    }
}