using Newtonsoft.Json;
using PlainCEETimer.Forms;
using PlainCEETimer.Modules.JsonConverters;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace PlainCEETimer.Modules.Configuration
{
    public sealed class ConfigObject
    {
        public GeneralObject General { get; set; } = new();

        public DisplayObject Display { get; set; } = new();

        public ExamInfoObject[] Exams
        {
            get;
            set
            {
                if (value != null)
                {
                    if (MainForm.ValidateNeeded)
                    {
                        Array.Sort(value);
                    }

                    field = value;
                }
            }
        } = [];

        public int ExamIndex
        {
            get;
            set
            {
                if (MainForm.ValidateNeeded)
                {
                    if (value < 0 || value > Exams.Length)
                    {
                        throw new Exception();
                    }
                }

                field = value;
            }
        }

        public string[] GlobalCustomTexts
        {
            get;
            set
            {
                if (MainForm.ValidateNeeded)
                {
                    foreach (var Text in value)
                    {
                        Validator.EnsureCustomTextLength(Text);
                    }
                }

                field = value;
            }
        } = [Placeholders.PH_P1, Placeholders.PH_P2, Placeholders.PH_P3];

        public ColorSetObject[] GlobalColors
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
                        if (!Validator.IsNiceContrast(value[i].Fore, value[i].Back))
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

        public CustomRuleObject[] CustomRules
        {
            get;
            set
            {
                if (value != null)
                {
                    if (MainForm.ValidateNeeded)
                    {
                        var HashSet = new HashSet<CustomRuleObject>();

                        foreach (var Item in value)
                        {
                            if (!HashSet.Add(Item))
                            {
                                throw new Exception();
                            }
                        }

                        Array.Sort(value);
                    }

                    field = value;
                }
            }
        } = [];

        [JsonConverter(typeof(CustomColorsConverter))]
        public int[] CustomColors { get; set; } = DefaultValues.ColorDialogColors;

        [JsonConverter(typeof(FontFormatConverter))]
        public Font Font { get; set; } = DefaultValues.CountdownDefaultFont;

        public int NtpServer
        {
            get;
            set
            {
                if (MainForm.ValidateNeeded)
                {
                    if (value is < 0 or > 3)
                    {
                        throw new Exception();
                    }
                }

                field = value;
            }
        }

        [JsonConverter(typeof(PointFormatConverter))]
        public Point Location { get; set; }
    }
}
