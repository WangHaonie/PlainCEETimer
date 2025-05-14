using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;

namespace PlainCEETimer.Modules
{
    public static class Validator
    {
        public const int MaxExamNameLength = 15;
        public const int MinExamNameLength = 2;
        public const int MaxFontSize = 36;
        public const int MinFontSize = 10;
        public const double MaxTick = 5662310399D; // 65535d 23h 59m 59s
        public const double MinTick = 1D; // 1s
        public const long MaxDate = 3155063615990000000L; // 9998-12-31 23:59:59
        public const long MinDate = 552877920000000000L; // 1753-01-01 00:00:00
        public const int MaxCustomTextLength = 800;
        public const char ValueSeparator = ',';
        public const string ValueSeparatorString = ", ";

        public static bool VerifyCustomText(string CustomText, out string Warning, int Index = 0)
        {
            var IndexHint = Index != 0 ? $"第{Index}个自定义文本" : "自定义文本";

            if (string.IsNullOrWhiteSpace(CustomText))
            {
                Warning = $"{IndexHint}不能为空白！";
                return false;
            }

            if (Regex.Matches(CustomText, @"\{.*?\}").Count == 0)
            {
                Warning = $"请在{IndexHint}中至少使用一个占位符！";
                return false;
            }

            Warning = null;
            return true;
        }

        public static bool IsNiceContrast(Color Fore, Color Back)
        {
            //
            // 对比度判断 参考:
            //
            // Guidance on Applying WCAG 2 to Non-Web Information and ...
            // https://www.w3.org/TR/wcag2ict/#dfn-contrast-ratio
            //

            double L1 = GetRelativeLuminance(Fore);
            double L2 = GetRelativeLuminance(Back);

            if (L1 < L2)
            {
                (L1, L2) = (L2, L1);
            }

            return (L1 + 0.05) / (L2 + 0.05) >= 3;
        }

        public static bool IsValidExamLength(int ExamLength)
            => ExamLength is >= MinExamNameLength and <= MaxExamNameLength;

        public static void EnsureExamDate(DateTime ExamTime)
        {
            if (ExamTime.Ticks is < MinDate or > MaxDate)
            {
                throw new Exception();
            }
        }

        public static void EnsureCustomTextLength(string Text)
        {
            if (Text.Length > MaxCustomTextLength)
            {
                throw new Exception();
            }
        }

        public static void Validate<T>(T[] value)
            where T : IListViewObject<T>
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

        public static Color GetColor(object obj)
        {
            int Argb;

            if (obj is int tmp)
            {
                Argb = tmp;
            }
            else
            {
                Argb = int.Parse(obj.ToString());
            }

            if (Argb > 0)
            {
                Argb = -Argb;
            }

            return Color.FromArgb(Argb);
        }

        private static double GetRelativeLuminance(Color color)
        {
            //
            // 亮度计算 参考:
            //
            // Guidance on Applying WCAG 2 to Non-Web Information and ...
            // https://www.w3.org/TR/wcag2ict/#dfn-relative-luminance
            //

            double RsRGB = color.R / 255.0;
            double GsRGB = color.G / 255.0;
            double BsRGB = color.B / 255.0;

            double R = RsRGB <= 0.03928 ? RsRGB / 12.92 : Math.Pow((RsRGB + 0.055) / 1.055, 2.4);
            double G = GsRGB <= 0.03928 ? GsRGB / 12.92 : Math.Pow((GsRGB + 0.055) / 1.055, 2.4);
            double B = BsRGB <= 0.03928 ? BsRGB / 12.92 : Math.Pow((BsRGB + 0.055) / 1.055, 2.4);

            return 0.2126 * R + 0.7152 * G + 0.0722 * B;
        }
    }
}
