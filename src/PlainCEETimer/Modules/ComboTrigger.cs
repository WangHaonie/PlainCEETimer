using System;
using PlainCEETimer.Modules.Annotations.SourceGenerators;
using PlainCEETimer.Modules.Extensions;

namespace PlainCEETimer.Modules;

public partial class ComboTrigger
{
    public int RemainCount
    {
        get
        {
            lock (m_lock)
            {
                return m_total - m_count;
            }
        }
    }

    [BackingField("m_triggered")]
    public partial bool Triggered { get; set; }

    private int m_count;
    private ulong m_lastTick;
    private readonly int m_total;
    private readonly ulong m_delay;
    private readonly object m_lock = new();

    public ComboTrigger(int requiredCount = 5, ulong msDelay = 100)
    {
        if (requiredCount < 2 || msDelay < 100)
        {
            throw new InvalidOperationException();
        }

        m_total = requiredCount;
        m_delay = msDelay;
    }

    public bool Trigger()
    {
        lock (m_lock)
        {
            if (m_triggered)
            {
                return false;
            }

            var tick = DateTime.TickCount;

            if (tick - m_lastTick > m_delay)
            {
                m_count = 1;
            }
            else
            {
                m_count++;
            }

            m_lastTick = tick;

            if (m_count >= m_total)
            {
                m_count = 0;
                m_lastTick = 0;
                return true;
            }

            return false;
        }
    }
}
