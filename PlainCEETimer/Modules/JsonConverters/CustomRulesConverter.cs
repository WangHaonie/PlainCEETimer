using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PlainCEETimer.Modules.Configuration;
using System;

namespace PlainCEETimer.Modules.JsonConverters
{
    public class CustomRulesConverter : JsonConverter<CustomRuleObject>
    {
        public override CustomRuleObject ReadJson(JsonReader reader, Type objectType, CustomRuleObject existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var Json = serializer.Deserialize<JObject>(reader);

            var Phase = CustomRuleHelper.GetPhase(int.Parse(Json[nameof(existingValue.Phase)].ToString()));
            var Tick = TimeSpan.FromSeconds(double.Parse(Json[nameof(existingValue.Tick)].ToString()));
            var Fore = ColorHelper.GetColor(Json, 0);
            var Back = ColorHelper.GetColor(Json, 1);

            if (!ColorHelper.IsNiceContrast(Fore, Back))
            {
                throw new Exception();
            }

            var Text = Json[nameof(existingValue.Text)].ToString().RemoveIllegalChars();
            CustomRuleHelper.VerifyText(Text);

            return new()
            {
                Phase = Phase,
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
                { nameof(value.Tick), value.Tick.TotalSeconds },
                { nameof(value.Fore), value.Fore.ToArgbInt() },
                { nameof(value.Back), value.Back.ToArgbInt() },
                { nameof(value.Text), value.Text }
            }.WriteTo(writer);
        }
    }
}
