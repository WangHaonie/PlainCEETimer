using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PlainCEETimer.Countdown;
using PlainCEETimer.Modules.Configuration;
using PlainCEETimer.Modules.Extensions;

namespace PlainCEETimer.Modules.JsonConverters;

public class GlobalRulesConverter : JsonConverter<CountdownRule[]>
{
    public override CountdownRule[] ReadJson(JsonReader reader, Type objectType, CountdownRule[] existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null)
        {
            return null;
        }

        serializer.Context = serializer.Context.SetContext(Validator.DefaultCountdownRuleFlag, out var context);
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
        if (value == null || (writer.Path != "GlobalRules" && value.ArrayEquals(App.AppConfig.GlobalRules)))
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