using PlainCEETimer.Modules.Configuration;

namespace PlainCEETimer.Modules.JsonConverters;

public sealed class BorderColorConverter : SimpleJsonConverter<BorderColorObject, int>
{
    protected override BorderColorObject Deserialize(int value)
    {
        return new(value);
    }

    protected override int Serialize(BorderColorObject obj)
    {
        return obj.Value;
    }
}
