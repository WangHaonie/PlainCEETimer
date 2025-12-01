using System;
using Newtonsoft.Json;
using PlainCEETimer.Countdown;
using PlainCEETimer.Modules.Configuration;
using PlainCEETimer.Modules.Extensions;

namespace PlainCEETimer.Modules.JsonConverters;

public sealed class ColorSetConverter : JsonConverter<ColorPair>
{
    public override ColorPair ReadJson(JsonReader reader, Type objectType, ColorPair existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        var colors = serializer.Deserialize<int[]>(reader);

        if (colors == null || colors.Length < 2)
        {
            throw Validator.InvalidTampering(ConfigField.ColorSetPartsLength);
        }

        return Validator.ParseColorPairFromConfig(colors[0], colors[1]);
    }

    public override void WriteJson(JsonWriter writer, ColorPair value, JsonSerializer serializer)
    {
        serializer.Serialize(writer, new int[] { value.Fore.ToInt32(), value.Back.ToInt32() });
    }
}