using System;
using System.Threading;

namespace PlainCEETimer.Modules;

public static class SafeExecutionContext
{
    private static int s_tid;
    private static SynchronizationContext s_context;

    public static void Send(IActionInvoker invoker)
    {
        if (invoker != null)
        {
            Send(invoker.Invoke);
        }
    }

    public static void Send(SendOrPostCallback callback, object state = null)
    {
        Execute(callback, state, false);
    }

    public static void Post(IActionInvoker invoker)
    {
        if (invoker != null)
        {
            Post(invoker.Invoke);
        }
    }

    public static void Post(SendOrPostCallback callback, object state = null)
    {
        Execute(callback, state);
    }

    internal static void SetContext(SynchronizationContext context)
    {
        if (s_tid == default)
        {
            var tid = Environment.CurrentManagedThreadId;

            if (tid != s_tid)
            {
                s_tid = tid;
                s_context = context;
            }
        }
    }

    private static void Execute(SendOrPostCallback callback, object state = null, bool post = true)
    {
        if (callback == null)
        {
            return;
        }

        if (s_context == null)
        {
            callback(state);
        }
        else
        {
            if (post)
            {
                s_context.Post(callback, state);
            }
            else
            {
                s_context.Send(callback, state);
            }
        }
    }
}