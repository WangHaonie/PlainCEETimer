using System;
using Newtonsoft.Json;
using PlainCEETimer.Modules.Configuration;
using PlainCEETimer.Modules.Extensions;

namespace PlainCEETimer.Modules.JsonConverters
{
    public sealed class ColorSetConverter : JsonConverter<ColorSetObject>
    {
        public override ColorSetObject ReadJson(JsonReader reader, Type objectType, ColorSetObject existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var colors = serializer.Deserialize<int[]>(reader);

            if (colors.Length == 2)
            {
                var fore = Validator.GetColor(colors[0]);
                var back = Validator.GetColor(colors[1]);

                if (Validator.IsNiceContrast(fore, back))
                {
                    return new(fore, back);
                }
            }

            throw new Exception();
        }

        public override void WriteJson(JsonWriter writer, ColorSetObject value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, new int[] { value.Fore.ToInt32(), value.Back.ToInt32() });
        }
    }
}