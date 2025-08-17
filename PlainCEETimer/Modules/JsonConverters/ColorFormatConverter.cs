using System;
using System.Drawing;
using Newtonsoft.Json;

namespace PlainCEETimer.Modules.JsonConverters
{
    public sealed class ColorFormatConverter : JsonConverter<Color>
    {
        public override Color ReadJson(JsonReader reader, Type objectType, Color existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            return ColorTranslator.FromOle(serializer.Deserialize<int>(reader));
        }

        public override void WriteJson(JsonWriter writer, Color value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, ColorTranslator.ToWin32(value));
        }
    }
}
