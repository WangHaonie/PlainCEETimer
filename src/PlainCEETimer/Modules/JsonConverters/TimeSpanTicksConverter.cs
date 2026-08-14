using System;

namespace PlainCEETimer.Modules.JsonConverters;

public sealed class TimeSpanTicksConverter : SimpleJsonConverter<TimeSpan, long>
{
    protected override TimeSpan Deserialize(long value)
    {
        return new(value);
    }

    protected override long Serialize(TimeSpan obj)
    {
        return obj.Ticks;
    }
}
