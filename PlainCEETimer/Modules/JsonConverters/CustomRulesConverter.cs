using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PlainCEETimer.Modules.Configuration;
using PlainCEETimer.Modules.Extensions;
using System;

namespace PlainCEETimer.Modules.JsonConverters
{
    public class CustomRulesConverter : JsonConverter<CustomRuleObject>
    {
        public override CustomRuleObject ReadJson(JsonReader reader, Type objectType, CustomRuleObject existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var Json = serializer.Deserialize<JObject>(reader);
            var PhaseInt = int.Parse(Json[nameof(existingValue.Phase)].ToString());

            if (PhaseInt is < 0 or > 2)
            {
                throw new Exception();
            }

            var Tick = TimeSpan.FromSeconds(double.Parse(Json[nameof(existingValue.Tick)].ToString()));
            var Fore = Validator.GetColor(Json, 0);
            var Back = Validator.GetColor(Json, 1);

            if (!Validator.IsNiceContrast(Fore, Back))
            {
                throw new Exception();
            }

            var Text = Json[nameof(existingValue.Text)].ToString().RemoveIllegalChars();
            Validator.EnsureCustomTextLength(Text);

            return new()
            {
                Phase = (CountdownPhase)PhaseInt,
                Tick = Tick,
                Text = Text,
                Fore = Fore,
                Back = Back
            };
        }

        public override void WriteJson(JsonWriter writer, CustomRuleObject value, JsonSerializer serializer)
        {
            new JObject()
            {
                { nameof(value.Phase), (int)value.Phase },
                { nameof(value.Tick), (long)value.Tick.TotalSeconds },
                { nameof(value.Fore), value.Fore.ToArgbInt() },
                { nameof(value.Back), value.Back.ToArgbInt() },
                { nameof(value.Text), value.Text }
            }.WriteTo(writer);
        }
    }
}
