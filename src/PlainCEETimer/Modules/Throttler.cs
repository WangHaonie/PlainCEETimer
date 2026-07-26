using System;
using PlainCEETimer.Modules.Extensions;

namespace PlainCEETimer.Modules;

public class Throttler(ulong interval = 500L)
{
    private ulong lastTick;
    private bool initialized;
    private readonly object syncLock = new();

    public bool Throttle()
    {
        lock (syncLock)
        {
            var now = DateTime.TickCount;

            if (!initialized || now - lastTick >= interval)
            {
                lastTick = now;
                initialized = true;
                return true;
            }

            return false;
        }
    }
}