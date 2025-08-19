using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using PlainCEETimer.Modules.JsonConverters;
using PlainCEETimer.UI;

namespace PlainCEETimer.Modules.Configuration
{
    public class ConfigObject
    {
        public GeneralObject General { get; set; } = new();

        public DisplayObject Display { get; set; } = new();

        public ExamInfoObject[] Exams
        {
            get;
            set => SetValue(ref field, value);
        } = [];

        public int ExamIndex { get; set; }

        public string[] GlobalCustomTexts
        {
            get;
            set
            {
                if (ValidateNeeded)
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
                if (ValidateNeeded && value.Length < 4)
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
            set => SetValue(ref field, value);
        } = [];

        [JsonConverter(typeof(CustomColorsConverter))]
        public int[] CustomColors { get; set; } = DefaultValues.ColorDialogColors;

        [JsonConverter(typeof(FontFormatConverter))]
        public Font Font { get; set; } = DefaultValues.CountdownDefaultFont;

        public int NtpServer
        {
            get;
            set => SetValue(ref field, value, 3, 0);
        }

        public int Dark
        {
            get;
            set => SetValue(ref field, value, 2, 0);
        }

        [JsonConverter(typeof(PointFormatConverter))]
        public Point Location { get; set; }

        internal static bool ValidateNeeded { get; set; } = true;

        public static readonly ConfigObject Empty = new();

        public static void SetValue(ref int field, int value, int max, int min, int defvalue = 0)
        {
            field = (ValidateNeeded && (value > max || value < min)) ? defvalue : value;
        }

        public static bool InvalidateBoolean(bool value, bool condition)
        {
            return ValidateNeeded ? value && condition : value;
        }

        public static void SetValue<T>(ref T[] field, T[] value)
            where T : IListViewData<T>
        {
            if (ValidateNeeded)
            {
                HashSet<T> set = [];

                foreach (var item in value)
                {
                    if (!set.Add(item))
                    {
                        throw new Exception();
                    }
                }

                Array.Sort(value);
            }

            field = value;
        }

        [OnDeserialized]
        internal void OnDeserializedMethod(StreamingContext context)
        {
            var value = ExamIndex;
            SetValue(ref value, value, Exams.Length, 0);
            ExamIndex = value;
            Display.SeewoPptsvc = InvalidateBoolean(Display.SeewoPptsvc, (General.TopMost && Display.X == 0) || Display.Draggable);
        }
    }
}
