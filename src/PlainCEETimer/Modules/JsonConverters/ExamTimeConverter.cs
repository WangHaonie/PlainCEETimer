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
            if (reader.ValueType == typeof(string) &&
            DateTime.TryParseExact(reader.Value.ToString(), App.DateTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime value))
            {
                return value;
            }

            return new((Convert.ToInt64(reader.Value) + Validator.MinDateSeconds) * Validator.MinTick);
        }
        catch
        {
            throw Validator.InvalidTampering(ConfigField.DateTimeFormat);
        }
    }

    public override void WriteJson(JsonWriter writer, DateTime value, JsonSerializer serializer)
    {
        writer.WriteValue((value.Ticks / Validator.MinTick) - Validator.MinDateSeconds);
    }
}
