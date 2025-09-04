using System;
using System.Drawing;
using Newtonsoft.Json;
using PlainCEETimer.Interop.Extensions;
using PlainCEETimer.Modules.Configuration;

namespace PlainCEETimer.Modules.JsonConverters;

public sealed class PointFormatConverter : JsonConverter<Point>
{
    public override Point ReadJson(JsonReader reader, Type objectType, Point existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        try
        {
            if (reader.TokenType == JsonToken.StartArray)
            {
                var parts = serializer.Deserialize<int[]>(reader);

                if (parts == null || parts.Length < 2)
                {
                    throw new Exception();
                }

                return new(parts[0], parts[1]);
            }

            return new(Convert.ToInt32(reader.Value));
        }
        catch
        {
            throw new InvalidTamperingException(ConfigField.PointFormat);
        }
    }

    public override void WriteJson(JsonWriter writer, Point value, JsonSerializer serializer)
    {
        writer.WriteValue(int.MakeLong(value.X, value.Y));
    }
}
