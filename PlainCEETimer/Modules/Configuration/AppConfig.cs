using System;
using System.Drawing;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using PlainCEETimer.Modules.JsonConverters;

namespace PlainCEETimer.Modules.Configuration
{
    public class AppConfig
    {
        public GeneralObject General { get; set; } = new();

        public DisplayObject Display { get; set; } = new();

        public ExamInfoObject[] Exams
        {
            get;
            set => Validator.SetValue(ref field, value);
        } = [];

        public int ExamIndex { get; set; }

        public string[] GlobalCustomTexts
        {
            get;
            set
            {
                if (Validator.ValidateNeeded)
                {
                    foreach (var text in value)
                    {
                        Validator.EnsureCustomText(text);
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
                if (Validator.ValidateNeeded && value.Length < 4)
                {
                    throw new Exception();
                }

                field = value;
            }
        } = DefaultValues.CountdownDefaultColors;

        public CustomRuleObject[] CustomRules
        {
            get;
            set => Validator.SetValue(ref field, value);
        } = [];

        [JsonConverter(typeof(CustomColorsConverter))]
        public int[] CustomColors { get; set; } = DefaultValues.ColorDialogColors;

        [JsonConverter(typeof(FontFormatConverter))]
        public Font Font { get; set; } = DefaultValues.CountdownDefaultFont;

        public int NtpServer
        {
            get;
            set => Validator.SetValue(ref field, value, 3, 0);
        }

        public int Dark
        {
            get;
            set => Validator.SetValue(ref field, value, 2, 0);
        }

        [JsonConverter(typeof(PointFormatConverter))]
        public Point Location { get; set; }

        public static readonly AppConfig Empty = new();

        [OnDeserialized]
        internal void OnDeserializedMethod(StreamingContext context)
        {
            var value = ExamIndex;
            Validator.SetValue(ref value, value, Exams.Length, 0);
            ExamIndex = value;
            Display.SeewoPptsvc = Validator.ValidateBoolean(Display.SeewoPptsvc, (General.TopMost && Display.X == 0) || Display.Draggable);
        }
    }
}
