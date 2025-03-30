﻿using Newtonsoft.Json;
using PlainCEETimer.Modules.JsonConverters;
using System.Drawing;

namespace PlainCEETimer.Modules.Configuration
{
    [JsonConverter(typeof(ColorSetConverter))]
    public readonly struct ColorSetObject(Color fore, Color back)
    {
        public Color Fore => fore;

        public Color Back => back;
    }
}
