using System;
using System.Drawing;
using Newtonsoft.Json;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.JsonConverters;

namespace PlainCEETimer.Countdown;

[JsonConverter(typeof(ColorPairConverter))]
public struct ColorPair(Color fore, Color back) : IEquatable<ColorPair>
{
    public readonly Color Fore => fore;

    public readonly Color Back => back;

    public bool Readable
    {
        get
        {
            if (!init)
            {
                //
                // 对比度判断 参考:
                //
                // Guidance on Applying WCAG 2 to Non-Web Information and ...
                // https://www.w3.org/TR/wcag2ict/#dfn-contrast-ratio
                //

                var L1 = GetRelativeLum(fore);
                var L2 = GetRelativeLum(back);

                if (L1 < L2)
                {
                    (L1, L2) = (L2, L1);
                }

                field = (L1 + 0.05) / (L2 + 0.05) >= 3;
                init = true;
            }

            return field;
        }
    }

    private bool init;

    public readonly bool Equals(ColorPair other)
    {
        return Fore == other.Fore && Back == other.Back;
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
            .Add(Fore)
            .Add(Back)
            .Combine();
    }

    private readonly double GetRelativeLum(Color color)
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
