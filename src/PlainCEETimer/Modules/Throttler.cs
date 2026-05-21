using System;
using System.Diagnostics;

namespace PlainCEETimer.Modules;

public class Throttler(Action action, int interval = 500)
{
    private readonly Action m_action = action ?? throw new ArgumentNullException(nameof(action));
    private readonly Stopwatch sw = new();
    private readonly object syncLock = new();

    public void Throttle()
    {
        lock (syncLock)
        {
            if (!sw.IsRunning)
            {
                sw.Start();
                m_action();
                return;
            }

            if (sw.ElapsedMilliseconds >= interval)
            {
                sw.Restart();
                m_action();
            }
        }
    }
}