using Newtonsoft.Json;
using PlainCEETimer.Modules.Configuration;
using System;

namespace PlainCEETimer.Modules.JsonConverters
{
    public sealed class ColorSetConverter : JsonConverter<ColorSetObject>
    {
        public override ColorSetObject ReadJson(JsonReader reader, Type objectType, ColorSetObject existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            int[] Colors = serializer.Deserialize<int[]>(reader);

            if (Colors.Length == 2)
            {
                var Fore = Validator.GetColor(Colors[0]);
                var Back = Validator.GetColor(Colors[1]);

                if (Validator.IsNiceContrast(Fore, Back))
                {
                    return new ColorSetObject(Fore, Back);
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