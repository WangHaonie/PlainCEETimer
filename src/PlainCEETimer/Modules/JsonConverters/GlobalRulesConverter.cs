using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PlainCEETimer.Countdown;
using PlainCEETimer.Modules.Configuration;
using PlainCEETimer.Modules.Extensions;
using PlainCEETimer.Modules.Linq;

namespace PlainCEETimer.Modules.JsonConverters;

public sealed class GlobalRulesConverter : JsonConverter<CountdownRule[]>
{
    public override CountdownRule[] ReadJson(JsonReader reader, Type objectType, CountdownRule[] existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null)
        {
            return null;
        }

        serializer.Context = serializer.Context.SetContext(ConfigValidator.DefaultCountdownRuleFlag, out var context);
        var jarr = JArray.Load(reader);
        var length = jarr.Count;
        var list = new List<CountdownRule>(length);

        for (int i = 0; i < length; i++)
        {
            list.Add(jarr[i].ToObject<CountdownRule>(serializer));
        }

        serializer.Context = context;
        return [.. list];
    }

    public override void WriteJson(JsonWriter writer, CountdownRule[] value, JsonSerializer serializer)
    {
        if (value == null
            || (writer.Path != nameof(AppConfig.GlobalRules) && value.ArrayEquals(App.AppConfig.GlobalRules, new CountdownRuleComparer())))
        {
            writer.WriteNull();
        }
        else
        {
            writer.WriteStartArray();

            foreach (var r in value)
            {
                serializer.Serialize(writer, r);
            }

            writer.WriteEndArray();
        }
    }
}