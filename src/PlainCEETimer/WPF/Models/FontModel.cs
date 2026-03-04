using System;
using System.Windows;
using System.Windows.Media;
using PlainCEETimer.Modules;
using Font = System.Drawing.Font;

namespace PlainCEETimer.WPF.Models;

public class FontModel : IEquatable<FontModel>
{
    public FontFamily FontFamily { get; set; }

    public double Size { get; set; }

    public FontWeight Weight { get; set; }

    public FontStyle Style { get; set; }

    public static FontModel FromGdiFont(Font font)
    {
        return new FontModel()
        {
            FontFamily = new(font.FontFamily.Name),
            Size = font.SizeInPoints * 96D / 72,
            Weight = font.Bold ? FontWeights.Bold : FontWeights.Normal,
            Style = font.Italic ? FontStyles.Italic : FontStyles.Normal
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
            && Weight == other.Weight
            && Style == other.Style;
    }

    public override bool Equals(object obj)
    {
        return Equals(obj as FontModel);
    }

    public override int GetHashCode()
    {
        return new HashCode()
            .Add(FontFamily?.Source)
            .Add(Size)
            .Add(Weight)
            .Add(Style)
            .Combine();
    }
}