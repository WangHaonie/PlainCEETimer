using System;
using System.Windows;
using Newtonsoft.Json;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.JsonConverters;
using PlainCEETimer.WPF.Extensions;
using Font = System.Drawing.Font;

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
        return FontFamily.Value.Source;
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