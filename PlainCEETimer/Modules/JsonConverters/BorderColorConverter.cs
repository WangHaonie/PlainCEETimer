using System;
using Newtonsoft.Json;
using PlainCEETimer.Modules.Configuration;

namespace PlainCEETimer.Modules.JsonConverters
{
    public sealed class BorderColorConverter : JsonConverter<BorderColorObject>
    {
        public override BorderColorObject ReadJson(JsonReader reader, Type objectType, BorderColorObject existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            return new(serializer.Deserialize<int>(reader));
        }

        public override void WriteJson(JsonWriter writer, BorderColorObject value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value.MakeLong());
        }
    }
}
