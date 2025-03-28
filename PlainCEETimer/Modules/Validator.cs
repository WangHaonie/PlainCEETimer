﻿using Newtonsoft.Json.Linq;
using PlainCEETimer.Modules.Configuration;
using System;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;

namespace PlainCEETimer.Modules
{
    public static class Validator
    {
        private static readonly string[] AllPHs = [Placeholders.PH_EXAMNAME, Placeholders.PH_DAYS, Placeholders.PH_HOURS, Placeholders.PH_MINUTES, Placeholders.PH_SECONDS, Placeholders.PH_CEILINGDAYS, Placeholders.PH_TOTALHOURS, Placeholders.PH_TOTALMINUTES, Placeholders.PH_TOTALSECONDS];

        public static bool VerifyCustomText(string CustomText, out string Warning, int Index = 0)
        {
            var IndexHint = Index != 0 ? $"第{Index}个自定义文本" : "自定义文本";

            if (string.IsNullOrWhiteSpace(CustomText))
            {
                Warning = $"{IndexHint}不能为空白！";
                return false;
            }

            var Matches = Regex.Matches(CustomText, @"\{.*?\}");

            foreach (Match m in Matches)
            {
                var mv = m.Value;

                if (!AllPHs.Contains(mv))
                {
                    Warning = $"在{IndexHint}中检测到了无效的占位符 {mv}，请重新设置！";
                    return false;
                }
            }

            if (Matches.Count == 0)
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

        public static string GetTickText(TimeSpan timeSpan)
            => $"{timeSpan.Days}天{timeSpan.Hours}时{timeSpan.Minutes}分{timeSpan.Seconds}秒";

        public static string GetPhaseText(CountdownPhase i) => i switch
        {
            CountdownPhase.P2 => Placeholders.PH_LEFT,
            CountdownPhase.P3 => Placeholders.PH_PAST,
            _ => Placeholders.PH_START
        };

        public static void EnsureCustomTextLength(string Text)
        {
            if (Text.Length > ConfigPolicy.MaxCustomTextLength)
            {
                throw new Exception();
            }
        }

        public static Color GetColor(object Argb)
            => GetColorCore(Argb);

        public static Color GetColor(JObject Json, int Index)
            => Index == 0
            ? GetColorCore(Json[nameof(ColorSetObject.Fore)])
            : GetColorCore(Json[nameof(ColorSetObject.Back)]);

        private static Color GetColorCore(object obj)
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
    }
}
