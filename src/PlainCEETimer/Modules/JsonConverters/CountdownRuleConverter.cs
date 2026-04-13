using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PlainCEETimer.Countdown;
using PlainCEETimer.Modules.Configuration;
using PlainCEETimer.Modules.Extensions;
using PlainCEETimer.UI;

namespace PlainCEETimer.Modules.JsonConverters;

public sealed class CountdownRuleConverter : JsonConverter<CountdownRule>
{
    public override CountdownRule ReadJson(JsonReader reader, Type objectType, CountdownRule existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        var json = reader.Load(serializer);
        var phaseInt = json.GetValue(nameof(existingValue.Phase), 0);

        if (phaseInt is < 0 or > 2)
        {
            throw ConfigValidator.InvalidTampering(ConfigField.CustomRulePhase);
        }

        var tick = default(TimeSpan);
        var is0tickAllowed = serializer.Context.CheckContext(ConfigValidator.DefaultCountdownRuleFlag);

        if (!is0tickAllowed)
        {
            tick = TimeSpan.FromSeconds(json.GetValue(nameof(existingValue.Tick), 0D));

            if (tick.Ticks is < ConfigValidator.MinTick or > ConfigValidator.MaxTick)
            {
                throw ConfigValidator.InvalidTampering(ConfigField.CustomRuleTick);
            }
        }

        var colors = ConfigValidator.ParseColorPairFromConfig(json.GetValue(nameof(ColorPair.Fore), 0), json.GetValue(nameof(ColorPair.Back), 0));

        var text = json.GetValue(nameof(existingValue.Text), string.Empty).Clean();
        ConfigValidator.EnsureCustomText(text);

        return new()
        {
            Phase = (CountdownPhase)phaseInt,
            Tick = tick,
            Text = text,
            Colors = colors,
            Default = is0tickAllowed
        };
    }

    public override void WriteJson(JsonWriter writer, CountdownRule value, JsonSerializer serializer)
    {
        var jo = new JObject
        {
            { nameof(value.Phase), (int)value.Phase }
        };

        if (!value.Default)
        {
            jo.Add(nameof(value.Tick), (long)value.Tick.TotalSeconds);
        }

        jo.Add(nameof(value.Colors.Fore), value.Colors.Fore.ToInt32());
        jo.Add(nameof(value.Colors.Back), value.Colors.Back.ToInt32());
        jo.Add(nameof(value.Text), value.Text);
        jo.WriteTo(writer);
    }
}
