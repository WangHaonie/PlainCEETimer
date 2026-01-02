using System;
using System.Globalization;
using Newtonsoft.Json;
using PlainCEETimer.Modules.Configuration;

namespace PlainCEETimer.Modules.JsonConverters;

public sealed class ExamTimeConverter : JsonConverter<DateTime>
{
    public override DateTime ReadJson(JsonReader reader, Type objectType, DateTime existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        try
        {
            if (reader.TokenType == JsonToken.String &&
            DateTime.TryParseExact(reader.Value.ToString(), App.DateTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out var value))
            {
                return value;
            }

            return new((Convert.ToInt64(reader.Value) + ConfigValidator.MinDateSeconds) * ConfigValidator.MinTick);
        }
        catch
        {
            throw ConfigValidator.InvalidTampering(ConfigField.DateTimeFormat);
        }
    }

    public override void WriteJson(JsonWriter writer, DateTime value, JsonSerializer serializer)
    {
        writer.WriteValue((value.Ticks / ConfigValidator.MinTick) - ConfigValidator.MinDateSeconds);
    }
}
