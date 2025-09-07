using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PlainCEETimer.Modules.Configuration;
using PlainCEETimer.Modules.Countdown;
using PlainCEETimer.Modules.Extensions;
using PlainCEETimer.UI;

namespace PlainCEETimer.Modules.JsonConverters;

public sealed class CustomRuleConverter : JsonConverter<CustomRule>
{
    public override CustomRule ReadJson(JsonReader reader, Type objectType, CustomRule existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        var json = JObject.Load(reader);
        var phaseInt = json[nameof(existingValue.Phase)].ToObject<int>(serializer);

        if (phaseInt is < 0 or > 2)
        {
            throw Validator.InvalidTampering(ConfigField.CustomRulePhase);
        }

        var tick = TimeSpan.FromSeconds(json[nameof(existingValue.Tick)].ToObject<double>(serializer));

        if (tick.Ticks is < Validator.MinTick or > Validator.MaxTick)
        {
            throw Validator.InvalidTampering(ConfigField.CustomRuleTick);
        }

        var fore = Validator.GetColorFromInt32(json[nameof(ColorPair.Fore)].ToObject<int>(serializer));
        var back = Validator.GetColorFromInt32(json[nameof(ColorPair.Back)].ToObject<int>(serializer));

        if (!Validator.IsNiceContrast(fore, back))
        {
            throw Validator.InvalidTampering(ConfigField.ColorSetContrast);
        }

        var text = json[nameof(existingValue.Text)].ToObject<string>(serializer).RemoveIllegalChars();
        Validator.EnsureCustomText(text);

        return new()
        {
            Phase = (CountdownPhase)phaseInt,
            Tick = tick,
            Text = text,
            Colors = new(fore, back)
        };
    }

    public override void WriteJson(JsonWriter writer, CustomRule value, JsonSerializer serializer)
    {
        new JObject()
        {
            { nameof(value.Phase), (int)value.Phase },
            { nameof(value.Tick), (long)value.Tick.TotalSeconds },
            { nameof(value.Colors.Fore), value.Colors.Fore.ToInt32() },
            { nameof(value.Colors.Back), value.Colors.Back.ToInt32() },
            { nameof(value.Text), value.Text }
        }.WriteTo(writer);
    }
}
