﻿using Newtonsoft.Json;
using System;
using System.Drawing;

namespace PlainCEETimer.Modules.JsonConverters
{
    public class PointFormatConverter : JsonConverter<Point>
    {
        public override Point ReadJson(JsonReader reader, Type objectType, Point existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            int[] PointParts = serializer.Deserialize<int[]>(reader);

            if (PointParts.Length == 2)
            {
                return new(PointParts[0], PointParts[1]);
            }

            throw new Exception();
        }

        public override void WriteJson(JsonWriter writer, Point value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, new int[] { value.X, value.Y });
        }
    }
}
