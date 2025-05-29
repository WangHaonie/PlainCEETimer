using System;
using System.Drawing;
using System.Linq;
using Newtonsoft.Json;

namespace PlainCEETimer.Modules.JsonConverters
{
    public sealed class FontFormatConverter : JsonConverter<Font>
    {
        public override Font ReadJson(JsonReader reader, Type objectType, Font existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var FontParts = reader.Value.ToString().Split(Validator.ValueSeparator);
            var FontPart1 = (Font)new FontConverter().ConvertFromString(string.Join(Validator.ValueSeparatorString, FontParts.Take(2)));
            var FontPart2 = (FontStyle)Enum.Parse(typeof(FontStyle), string.Join(Validator.ValueSeparatorString, FontParts.Skip(2)));

            if (FontPart1.Size is >= Validator.MinFontSize and <= Validator.MaxFontSize)
            {
                return new Font(FontPart1, FontPart2);
            }

            throw new Exception();
        }

        public override void WriteJson(JsonWriter writer, Font value, JsonSerializer serializer)
        {
            writer.WriteValue($"{value.Name}, {value.Size}pt, {value.Style}");
        }
    }
}
