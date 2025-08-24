using System;
using System.Drawing;
using Newtonsoft.Json;

namespace PlainCEETimer.Modules.JsonConverters;

public sealed class PointFormatConverter : JsonConverter<Point>
{
    public override Point ReadJson(JsonReader reader, Type objectType, Point existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        var parts = serializer.Deserialize<int[]>(reader);

        if (parts.Length == 2)
        {
            return new(parts[0], parts[1]);
        }

        throw new Exception();
    }

    public override void WriteJson(JsonWriter writer, Point value, JsonSerializer serializer)
    {
        serializer.Serialize(writer, new int[] { value.X, value.Y });
    }
}
