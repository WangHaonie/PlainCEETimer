using Newtonsoft.Json;
using PlainCEETimer.Forms;
using PlainCEETimer.Modules.JsonConverters;
using System;
using System.Drawing;

namespace PlainCEETimer.Modules.Configuration
{
    public sealed class AppearanceObject
    {
        [JsonConverter(typeof(FontFormatConverter))]
        public Font Font { get; set; } = DefaultValues.CountdownDefaultFont;

        public ColorSetObject[] Colors
        {
            get;
            set
            {
                if (MainForm.ValidateNeeded)
                {
                    if (value.Length > 4)
                    {
                        throw new Exception();
                    }

                    for (int i = 0; i < 4; i++)
                    {
                        if (!ColorHelper.IsNiceContrast(value[i].Fore, value[i].Back))
                        {
                            throw new Exception();
                        }
                    }
                }

                field = value;
            }
        } = DefaultValues.IsDarkModeSupported
          ? DefaultValues.CountdownDefaultColorsDark
          : DefaultValues.CountdownDefaultColorsLight;
    }
}
