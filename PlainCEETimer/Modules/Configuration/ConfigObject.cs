using System;
using System.Drawing;
using Newtonsoft.Json;
using PlainCEETimer.Forms;
using PlainCEETimer.Modules.JsonConverters;

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
                    for (int i = 0; i < value.Length; i++)
                    {
                        Validator.EnsureCustomTextLength(value[i]);
                    }
                }

                field = value;
            }
        } = [Constants.PH_P1, Constants.PH_P2, Constants.PH_P3];

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
        } = DefaultValues.AutoDarkCountdown
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
