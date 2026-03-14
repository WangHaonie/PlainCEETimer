using PlainCEETimer.UI;

namespace PlainCEETimer.Modules.JsonConverters;

public sealed class HotKeyConverter : SimpleJsonConverter<HotKey, ushort>
{
    protected override HotKey Deserialize(ushort value)
    {
        return new(value);
    }

    protected override ushort Serialize(HotKey obj)
    {
        return (ushort)obj.GetHashCode();
    }
}