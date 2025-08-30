using System;
using System.Drawing;
using Newtonsoft.Json;
using PlainCEETimer.Modules.Configuration;

namespace PlainCEETimer.Modules.JsonConverters;

public sealed class PointFormatConverter : JsonConverter<Point>
{
    public override Point ReadJson(JsonReader reader, Type objectType, Point existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        var parts = serializer.Deserialize<int[]>(reader);

        if (parts == null || parts.Length < 2)
        {
            throw new InvalidTamperingException(ConfigField.PointPartsLength);
        }

        return new(parts[0], parts[1]);
    }

    public override void WriteJson(JsonWriter writer, Point value, JsonSerializer serializer)
    {
        serializer.Serialize(writer, new int[] { value.X, value.Y });
    }
}
