using System;
using System.Drawing;
using System.Windows;
using Newtonsoft.Json;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Extensions;
using PlainCEETimer.Modules.JsonConverters;
using PlainCEETimer.WPF.Extensions;
using Font = System.Drawing.Font;
using WFFontStyle = System.Drawing.FontStyle;

namespace PlainCEETimer.WPF.Models;

[JsonConverter(typeof(FontModelConverter))]
public class FontModel : IEquatable<FontModel>
{
    public FontFamilyWrapper FontFamily { get; init; }

    public double Size { get; init; }

    [JsonIgnore]
    public double SizePt
    {
        get
        {
            if (field == default)
            {
                field = Size.Dip2Pt();
            }

            return field;
        }

        init;
    }

    public FontWeight Weight { get; init; }

    public Font ToGdiFont()
    {
        var ff = FontFamily;

        if (ff != null)
        {
            var name = ff.FirstFontFamilyName;

            if (!string.IsNullOrEmpty(name))
            {
                try
                {
                    var result = new Font(name, (float)SizePt,
                        Weight == FontWeights.Bold ? WFFontStyle.Bold : WFFontStyle.Regular,
                        GraphicsUnit.Point);

                    if (result.Name != name)
                    {
                        result.Destroy();
                        return null;
                    }

                    return result;
                }
                catch
                {
                    return null;
                }
            }
        }

        return null;
    }

    public static FontModel FromGdiFont(Font font)
    {
        return new FontModel()
        {
            FontFamily = new(font.FontFamily.Name),
            SizePt = font.SizeInPoints,
            Size = ((double)font.SizeInPoints).Pt2Dip(),
            Weight = font.Bold ? FontWeights.Bold : FontWeights.Normal
        };
    }

    public bool Equals(FontModel other)
    {
        if (other == null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return FontFamily?.Value.Source == other.FontFamily?.Value.Source
            && Size == other.Size
            && Weight == other.Weight;
    }

    public override bool Equals(object obj)
    {
        return Equals(obj as FontModel);
    }

    public override string ToString()
    {
        return FontFamily?.Value.Source;
    }

    public override int GetHashCode()
    {
        return new HashCode()
            .Add(FontFamily?.Value.Source)
            .Add(Size)
            .Add(Weight)
            .Combine();
    }
}