using PlainCEETimer.Interop;

namespace PlainCEETimer.Modules.JsonConverters;

public sealed class Win32ColorFormatConverter : SimpleJsonConverter<COLORREF, int>
{
    protected override COLORREF Deserialize(int value)
    {
        return (COLORREF)value;
    }

    protected override int Serialize(COLORREF obj)
    {
        return obj;
    }
}