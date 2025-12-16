using System;
using Newtonsoft.Json;
using PlainCEETimer.Interop;

namespace PlainCEETimer.Modules.JsonConverters;

public class HotKeyConverter : JsonConverter<HotKey>
{
    public override HotKey ReadJson(JsonReader reader, Type objectType, HotKey existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        return new(serializer.Deserialize<ushort>(reader));
    }

    public override void WriteJson(JsonWriter writer, HotKey value, JsonSerializer serializer)
    {
        writer.WriteValue(value.GetHashCode());
    }
}