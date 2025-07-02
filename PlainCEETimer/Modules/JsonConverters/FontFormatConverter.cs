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
            var parts = reader.Value.ToString().Split(Validator.ValueSeparator);
            var part1 = (Font)new FontConverter().ConvertFromString(string.Join(Validator.ValueSeparatorString, parts.Take(2)));
            var part2 = (FontStyle)Enum.Parse(typeof(FontStyle), string.Join(Validator.ValueSeparatorString, parts.Skip(2)));

            if (part1.Size is >= Validator.MinFontSize and <= Validator.MaxFontSize)
            {
                return new Font(part1, part2);
            }

            throw new Exception();
        }

        public override void WriteJson(JsonWriter writer, Font value, JsonSerializer serializer)
        {
            writer.WriteValue($"{value.Name}, {value.Size}pt, {value.Style}");
        }
    }
}
