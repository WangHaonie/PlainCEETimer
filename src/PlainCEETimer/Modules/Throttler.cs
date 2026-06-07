using System;
using System.Diagnostics;

namespace PlainCEETimer.Modules;

public class Throttler(long interval = 500L)
{
    private readonly Stopwatch sw = new();
    private readonly object syncLock = new();

    public void Throttle(Action action)
    {
        if (CanExecute())
        {
            action();
        }
    }

    public void Throttle<T>(Action<T> action, T obj)
    {
        if (CanExecute())
        {
            action(obj);
        }
    }

    private bool CanExecute()
    {
        lock (syncLock)
        {
            if (!sw.IsRunning)
            {
                sw.Start();
                return true;
            }

            if (sw.ElapsedMilliseconds >= interval)
            {
                sw.Restart();
                return true;
            }

            return false;
        }
    }
}