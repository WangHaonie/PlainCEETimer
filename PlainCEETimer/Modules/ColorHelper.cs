using Newtonsoft.Json.Linq;
using PlainCEETimer.Modules.Configuration;
using System;
using System.Drawing;

namespace PlainCEETimer.Modules
{
    public static class ColorHelper
    {
        public static Color GetColor(object Argb)
            => GetColorCore(Argb);

        public static Color GetColor(JObject Json, int Index) => Index switch
        {
            0 => GetColorCore(Json[nameof(ColorSetObject.Fore)]),
            _ => GetColorCore(Json[nameof(ColorSetObject.Back)])
        };

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
