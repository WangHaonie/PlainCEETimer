using System;
using System.Drawing;
using Newtonsoft.Json;
using PlainCEETimer.Modules.Extensions;

namespace PlainCEETimer.Modules.JsonConverters
{
    public sealed class ColorFormatConverter : JsonConverter<Color>
    {
        public override Color ReadJson(JsonReader reader, Type objectType, Color existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            return serializer.Deserialize<int>(reader).ToColor();
        }

        public override void WriteJson(JsonWriter writer, Color value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value.ToWin32());
        }
    }
}
