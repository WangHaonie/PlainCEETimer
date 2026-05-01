using System.Drawing;
using PlainCEETimer.Interop.Extensions;

namespace PlainCEETimer.Modules.JsonConverters;

public sealed class SizeFormatConverter : SimpleJsonConverter<Size, int>
{
    protected override Size Deserialize(int value)
    {
        return new(value.HiWord, value.LoWord);
    }

    protected override int Serialize(Size obj)
    {
        return int.MakeLong(obj.Height, obj.Width);
    }
}