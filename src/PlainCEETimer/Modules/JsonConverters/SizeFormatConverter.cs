using System;
using System.Drawing;
using Newtonsoft.Json;
using PlainCEETimer.Interop;

namespace PlainCEETimer.Modules.JsonConverters;

public sealed class SizeFormatConverter : JsonConverter<Size>
{
    public override Size ReadJson(JsonReader reader, Type objectType, Size existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        var raw = serializer.Deserialize<int>(reader);
        return new(raw.HiWord, raw.LoWord);
    }

    public override void WriteJson(JsonWriter writer, Size value, JsonSerializer serializer)
    {
        serializer.Serialize(writer, int.MakeLong(value.Height, value.Width));
    }
}