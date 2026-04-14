using System;
using System.Windows;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PlainCEETimer.Modules.Configuration;
using PlainCEETimer.Modules.Extensions;
using PlainCEETimer.WPF.Models;

namespace PlainCEETimer.Modules.JsonConverters;

public sealed class FontModelConverter : JsonConverter<FontModel>
{
    public override FontModel ReadJson(JsonReader reader, Type objectType, FontModel existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        var json = reader.Load(serializer);
        var ffvalue = json.GetValue(nameof(FontModel.FontFamily), "#GLOBAL USER INTERFACE").Truncate(ConfigValidator.MaxFontFamilyLength, false);
        var fsvalue = json.GetValue(nameof(FontModel.Size), default(double)).Clamp(ConfigValidator.MinFontSize, ConfigValidator.MaxFontSize);
        var fwvalue = json.GetValue(nameof(FontModel.Weight), default(int)).Clamp(1, 999);

        return new()
        {
            FontFamily = new(ffvalue),
            Size = fsvalue,
            Weight = FontWeight.FromOpenTypeWeight(fwvalue)
        };
    }

    public override void WriteJson(JsonWriter writer, FontModel value, JsonSerializer serializer)
    {
        if (value != null && value.FontFamily != null)
        {
            var src = value.FontFamily?.Name;

            if (src != null)
            {
                new JObject()
                {
                    [nameof(FontModel.FontFamily)] = src.Trim(),
                    [nameof(FontModel.Size)] = Math.Round(value.Size, 2),
                    [nameof(FontModel.Weight)] = value.Weight.ToOpenTypeWeight()
                }.WriteTo(writer);
            }
        }
    }
}