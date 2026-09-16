#pragma warning disable IDE0290

using System;
using System.Drawing;
using Newtonsoft.Json;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Annotations.SourceGenerators;
using PlainCEETimer.Modules.Extensions;
using PlainCEETimer.Modules.JsonConverters;

namespace PlainCEETimer.Countdown;

[JsonConverter(typeof(ColorPairConverter))]
public partial struct ColorPair : IEquatable<ColorPair>
{
    [BackingField(MemberNames.fore)]
    public readonly partial Color Fore { get; }

    [BackingField(MemberNames.back)]
    public readonly partial Color Back { get; }

    public bool? Readable
    {
        get
        {
            if (field == null)
            {
                //
                // 对比度判断 参考:
                //
                // Guidance on Applying WCAG 2 to Non-Web Information and ...
                // https://www.w3.org/TR/wcag2ict/#dfn-contrast-ratio
                //

                var L1 = GetRelativeLum(fore);
                var L2 = GetRelativeLum(back);
                double.SwapIf(L1 < L2, ref L1, ref L2);
                field = (L1 + 0.05) / (L2 + 0.05) >= 3;
            }

            return field;
        }
    }

    public ColorPair(Color foreColor, Color backColor)
    {
        fore = foreColor;
        back = backColor;
    }

    public readonly bool Equals(ColorPair other)
    {
        return fore == other.fore && back == other.back;
    }

    public readonly override bool Equals(object obj)
    {
        if (obj is ColorPair cp)
        {
            return Equals(cp);
        }

        return false;
    }

    public readonly override int GetHashCode()
    {
        return new HashCode()
            .Add(fore)
            .Add(back)
            .Combine();
    }

    public static bool operator ==(ColorPair left, ColorPair right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(ColorPair left, ColorPair right)
    {
        return !(left == right);
    }

    private static double GetRelativeLum(Color color)
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

#pragma warning restore IDE0290
