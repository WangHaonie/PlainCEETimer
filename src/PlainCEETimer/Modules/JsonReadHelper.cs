using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PlainCEETimer.Modules;

public class JsonReadHelper(JObject json, JsonSerializer serializer)
{
    public T GetValue<T>(string propertyName, T defaultValue)
    {
        if (json.TryGetValue(propertyName, StringComparison.OrdinalIgnoreCase, out var jt))
        {
            var tmp = jt.ToObject<T>(serializer);

            if (tmp != null)
            {
                return tmp;
            }
        }

        return defaultValue;
    }
}