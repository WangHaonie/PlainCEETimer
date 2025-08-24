using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PlainCEETimer.Modules.Configuration;
using PlainCEETimer.Modules.Extensions;
using PlainCEETimer.UI;

namespace PlainCEETimer.Modules.JsonConverters;

public sealed class CustomRuleConverter : JsonConverter<CustomRuleObject>
{
    public override CustomRuleObject ReadJson(JsonReader reader, Type objectType, CustomRuleObject existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        var json = serializer.Deserialize<JObject>(reader);
        var phaseInt = int.Parse(json[nameof(existingValue.Phase)].ToString());

        if (phaseInt is < 0 or > 2)
        {
            throw new Exception();
        }

        var tick = TimeSpan.FromSeconds(double.Parse(json[nameof(existingValue.Tick)].ToString()));

        if (tick.Ticks is < Validator.MinTick or > Validator.MaxTick)
        {
            throw new Exception();
        }

        var fore = Validator.GetColor(json[nameof(ColorSetObject.Fore)]);
        var back = Validator.GetColor(json[nameof(ColorSetObject.Back)]);

        if (!Validator.IsNiceContrast(fore, back))
        {
            throw new Exception();
        }

        var text = json[nameof(existingValue.Text)].ToString().RemoveIllegalChars();
        Validator.EnsureCustomText(text);

        return new()
        {
            Phase = (CountdownPhase)phaseInt,
            Tick = tick,
            Text = text,
            Colors = new(fore, back)
        };
    }

    public override void WriteJson(JsonWriter writer, CustomRuleObject value, JsonSerializer serializer)
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
