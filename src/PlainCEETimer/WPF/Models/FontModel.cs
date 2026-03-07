using System;
using System.Windows;
using System.Windows.Media;
using Newtonsoft.Json;
using PlainCEETimer.Modules;
using Font = System.Drawing.Font;

namespace PlainCEETimer.WPF.Models;

public class FontModel : IEquatable<FontModel>
{
    public FontFamily FontFamily { get; set; }

    public double Size { get; set; }

    [JsonIgnore]
    public double SizePt
    {
        get
        {
            if (field == default)
            {
                field = Size / (96.0 / 72.0);
            }

            return field;
        }

        set;
    }

    public FontWeight Weight { get; set; }

    public static FontModel FromGdiFont(Font font)
    {
        return new FontModel()
        {
            FontFamily = new(font.FontFamily.Name),
            SizePt = font.SizeInPoints,
            Size = font.SizeInPoints * (96.0 / 72.0),
            Weight = font.Bold ? FontWeights.Bold : FontWeights.Normal
        };
    }

    public bool Equals(FontModel other)
    {
        if (other == null)
        {
            return false;
        }

        return FontFamily.Source == other.FontFamily.Source
            && Size == other.Size
            && Weight == other.Weight;
    }

    public override bool Equals(object obj)
    {
        return Equals(obj as FontModel);
    }

    public override string ToString()
    {
        return FontFamily.Source;
    }

    public override int GetHashCode()
    {
        return new HashCode()
            .Add(FontFamily?.Source)
            .Add(Size)
            .Add(Weight)
            .Combine();
    }
}