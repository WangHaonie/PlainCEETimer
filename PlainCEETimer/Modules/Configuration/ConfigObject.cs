using System;
using System.Drawing;
using Newtonsoft.Json;
using PlainCEETimer.Modules.JsonConverters;
using PlainCEETimer.UI.Forms;

namespace PlainCEETimer.Modules.Configuration
{
    public class ConfigObject
    {
        public GeneralObject General { get; set; } = new();

        public DisplayObject Display { get; set; } = new();

        public ExamInfoObject[] Exams
        {
            get;
            set
            {
                if (MainForm.ValidateNeeded)
                {
                    Validator.Validate(value);
                }

                field = value;
            }
        } = [];

        public int ExamIndex
        {
            get;
            set
            {
                if (MainForm.ValidateNeeded && (value < 0 || value > Exams.Length))
                {
                    throw new Exception();
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
                    foreach (var text in value)
                    {
                        Validator.EnsureCustomTextLength(text);
                    }
                }

                field = value;
            }
        } = DefaultValues.GlobalDefaultCustomTexts;

        public ColorSetObject[] GlobalColors
        {
            get;
            set
            {
                if (MainForm.ValidateNeeded && value.Length < 4)
                {
                    throw new Exception();
                }

                field = value;
            }
        } = DefaultValues.AutoDarkTheme
          ? DefaultValues.CountdownDefaultColorsDark
          : DefaultValues.CountdownDefaultColorsLight;

        public CustomRuleObject[] CustomRules
        {
            get;
            set
            {
                if (MainForm.ValidateNeeded)
                {
                    Validator.Validate(value);
                }

                field = value;
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
                if (MainForm.ValidateNeeded && (value is < 0 or > 3))
                {
                    throw new Exception();
                }

                field = value;
            }
        }

        public int Dark
        {
            get;
            set
            {
                if (MainForm.ValidateNeeded && (value is < 0 or > 2))
                {
                    throw new Exception();
                }

                field = value;
            }
        }

        [JsonConverter(typeof(PointFormatConverter))]
        public Point Location { get; set; }
    }
}
