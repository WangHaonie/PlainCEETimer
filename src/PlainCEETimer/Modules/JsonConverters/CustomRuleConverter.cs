using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PlainCEETimer.Countdown;
using PlainCEETimer.Modules.Configuration;
using PlainCEETimer.Modules.Extensions;
using PlainCEETimer.UI;

namespace PlainCEETimer.Modules.JsonConverters;

public sealed class CustomRuleConverter : JsonConverter<CountdownRule>
{
    public override CountdownRule ReadJson(JsonReader reader, Type objectType, CountdownRule existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        var json = JObject.Load(reader);
        var phaseInt = json[nameof(existingValue.Phase)].ToObject<int>(serializer);

        if (phaseInt is < 0 or > 2)
        {
            throw Validator.InvalidTampering(ConfigField.CustomRulePhase);
        }

        var tick = default(TimeSpan);
        var is0tickAllowed = serializer.Context.CheckContext(Validator.DefaultCountdownRuleFlag);

        if (!is0tickAllowed)
        {
            tick = TimeSpan.FromSeconds(json[nameof(existingValue.Tick)].ToObject<double>(serializer));

            if (tick.Ticks is < Validator.MinTick or > Validator.MaxTick)
            {
                throw Validator.InvalidTampering(ConfigField.CustomRuleTick);
            }
        }

        var colors = Validator.ParseColorPairFromConfig(json[nameof(ColorPair.Fore)].ToObject<int>(serializer), json[nameof(ColorPair.Back)].ToObject<int>(serializer));
        var text = json[nameof(existingValue.Text)].ToObject<string>(serializer).RemoveIllegalChars();
        Validator.EnsureCustomText(text);

        return new()
        {
            Phase = (CountdownPhase)phaseInt,
            Tick = tick,
            Text = text,
            Colors = colors,
            IsDefault = is0tickAllowed
        };
    }

    public override void WriteJson(JsonWriter writer, CountdownRule value, JsonSerializer serializer)
    {
        var jo = new JObject
        {
            { nameof(value.Phase), (int)value.Phase }
        };

        if (!value.IsDefault)
        {
            jo.Add(nameof(value.Tick), (long)value.Tick.TotalSeconds);
        }

        jo.Add(nameof(value.Colors.Fore), value.Colors.Fore.ToInt32());
        jo.Add(nameof(value.Colors.Back), value.Colors.Back.ToInt32());
        jo.Add(nameof(value.Text), value.Text);
        jo.WriteTo(writer);
    }
}
