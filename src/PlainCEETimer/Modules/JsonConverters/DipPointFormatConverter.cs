using System;
using System.Windows;
using Newtonsoft.Json;
using PlainCEETimer.Modules.Configuration;

namespace PlainCEETimer.Modules.JsonConverters;

public sealed class DipPointFormatConverter : JsonConverter<Point>
{
    public override Point ReadJson(JsonReader reader, Type objectType, Point existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        var arr = serializer.Deserialize<double[]>(reader);

        if (arr == null)
        {
            return default;
        }

        if (arr.Length < 2)
        {
            throw ConfigValidator.InvalidTampering(ConfigField.DipPointFormat);
        }

        return new(arr[0], arr[1]);
    }

    public override void WriteJson(JsonWriter writer, Point value, JsonSerializer serializer)
    {
        serializer.Serialize(writer, new double[] { value.X, value.Y });
    }
}
