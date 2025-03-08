using Newtonsoft.Json;
using PlainCEETimer.Modules.Configuration;
using PlainCEETimer.Modules.Extensions;
using System;

namespace PlainCEETimer.Modules.JsonConverters
{
    public class ColorSetConverter : JsonConverter<ColorSetObject>
    {
        public override ColorSetObject ReadJson(JsonReader reader, Type objectType, ColorSetObject existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            int[] Colors = serializer.Deserialize<int[]>(reader);

            if (Colors.Length == 2)
            {
                var Fore = ColorHelper.GetColor(Colors[0]);
                var Back = ColorHelper.GetColor(Colors[1]);

                if (ColorHelper.IsNiceContrast(Fore, Back))
                {
                    return new ColorSetObject(Fore, Back);
                }
            }

            throw new Exception();
        }

        public override void WriteJson(JsonWriter writer, ColorSetObject value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, new int[] { value.Fore.ToArgbInt(), value.Back.ToArgbInt() });
        }
    }
}