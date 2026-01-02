using System;
using System.Drawing;
using Newtonsoft.Json;
using PlainCEETimer.Modules.Configuration;
using PlainCEETimer.Modules.Extensions;
using PlainCEETimer.Modules.Linq;

namespace PlainCEETimer.Modules.JsonConverters;

public sealed class FontFormatConverter : JsonConverter<Font>
{
    public override Font ReadJson(JsonReader reader, Type objectType, Font existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        var parts = reader.Value.ToString().Split(ConfigValidator.ValueSeparator);
        using var part1 = (Font)new FontConverter().ConvertFromString(string.Join(ConfigValidator.ValueSeparatorString, parts.ArrayTake(2)));
        var part2 = (FontStyle)Enum.Parse(typeof(FontStyle), string.Join(ConfigValidator.ValueSeparatorString, parts.ArraySkip(2)));
        var size = part1.Size;

        if ((size >= ConfigValidator.MinFontSize - ConfigValidator.MinFontSizeError) && size <= ConfigValidator.MaxFontSize)
        {
            return new Font(part1, part2);
        }

        throw ConfigValidator.InvalidTampering(ConfigField.CountdownFont);
    }

    public override void WriteJson(JsonWriter writer, Font value, JsonSerializer serializer)
    {
        writer.WriteValue(value.Format());
    }
}
