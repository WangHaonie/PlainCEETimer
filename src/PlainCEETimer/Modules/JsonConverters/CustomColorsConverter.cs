using System;
using System.Linq;
using Newtonsoft.Json;
using PlainCEETimer.Modules.Configuration;

namespace PlainCEETimer.Modules.JsonConverters;

public sealed class CustomColorsConverter : JsonConverter<int[]>
{
    public override int[] ReadJson(JsonReader reader, Type objectType, int[] existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        return reader.TokenType == JsonToken.Null ? DefaultValues.ColorDialogColors : serializer.Deserialize<int[]>(reader);
    }

    public override void WriteJson(JsonWriter writer, int[] value, JsonSerializer serializer)
    {
        serializer.Serialize(writer, value.SequenceEqual(DefaultValues.ColorDialogColors) ? null : value);
    }
}
