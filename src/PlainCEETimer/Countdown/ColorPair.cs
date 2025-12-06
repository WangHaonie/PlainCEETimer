using System;
using System.Drawing;
using Newtonsoft.Json;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.JsonConverters;

namespace PlainCEETimer.Countdown;

[JsonConverter(typeof(ColorSetConverter))]
public readonly struct ColorPair(Color fore, Color back) : IEquatable<ColorPair>
{
    public Color Fore => foreColor;

    public Color Back => backColor;

    public bool Readable
    {
        get
        {
            //
            // 对比度判断 参考:
            //
            // Guidance on Applying WCAG 2 to Non-Web Information and ...
            // https://www.w3.org/TR/wcag2ict/#dfn-contrast-ratio
            //

            var L1 = GetRelativeLum(foreColor);
            var L2 = GetRelativeLum(backColor);

            if (L1 < L2)
            {
                (L1, L2) = (L2, L1);
            }

            return (L1 + 0.05) / (L2 + 0.05) >= 3;
        }
    }

    private readonly Color foreColor = fore;
    private readonly Color backColor = back;

    public bool Equals(ColorPair other)
    {
        return foreColor == other.foreColor && backColor == other.backColor;
    }

    public override bool Equals(object obj)
    {
        return Equals((ColorPair)obj);
    }

    public override int GetHashCode()
    {
        return new HashCode()
            .Add(foreColor)
            .Add(backColor)
            .Combine();
    }

    private double GetRelativeLum(Color color)
    {
        //
        // 亮度计算 参考:
        //
        // Guidance on Applying WCAG 2 to Non-Web Information and ...
        // https://www.w3.org/TR/wcag2ict/#dfn-relative-luminance
        //

        var RsRGB = color.R / 255.0;
        var GsRGB = color.G / 255.0;
        var BsRGB = color.B / 255.0;

        var R = RsRGB <= 0.03928 ? RsRGB / 12.92 : Math.Pow((RsRGB + 0.055) / 1.055, 2.4);
        var G = GsRGB <= 0.03928 ? GsRGB / 12.92 : Math.Pow((GsRGB + 0.055) / 1.055, 2.4);
        var B = BsRGB <= 0.03928 ? BsRGB / 12.92 : Math.Pow((BsRGB + 0.055) / 1.055, 2.4);

        return 0.2126 * R + 0.7152 * G + 0.0722 * B;
    }
}
