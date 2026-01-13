using System;
using System.Globalization;
using Newtonsoft.Json;
using PlainCEETimer.Modules.Configuration;
using PlainCEETimer.Modules.Extensions;

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

            return Convert.ToInt64(reader.Value).ToDateTime();
        }
        catch
        {
            throw ConfigValidator.InvalidTampering(ConfigField.DateTimeFormat);
        }
    }

    public override void WriteJson(JsonWriter writer, DateTime value, JsonSerializer serializer)
    {
        writer.WriteValue(value.ToTimestamp());
    }
}
