using System;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using PlainCEETimer.Modules.Fody;
using WFColor = System.Drawing.Color;
using WFPoint = System.Drawing.Point;
using WFSize = System.Drawing.Size;

namespace PlainCEETimer.WPF.Extensions;

[NoConstants]
public static class Extensions
{
    private const double PtToDipRatio = 96.0 / 72.0;

    public static Color ToColor(this WFColor color)
    {
        return Color.FromArgb(color.A, color.R, color.G, color.B);
    }

    public static Point ToDouble(this WFPoint point)
    {
        return new(point.X, point.Y);
    }

    public static WFPoint Truncate(this Point point)
    {
        return new((int)point.X, (int)point.Y);
    }

    public static Size ToDouble(this WFSize size)
    {
        return new(size.Width, size.Height);
    }

    public static WFSize Truncate(this Size size)
    {
        return new((int)size.Width, (int)size.Height);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Pt2Dip(this double pt)
    {
        return pt * PtToDipRatio;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Dip2Pt(this double dip)
    {
        return dip / PtToDipRatio;
    }

    public static Collection<ResourceDictionary> Add(this Collection<ResourceDictionary> dict, string uri, bool condition = true)
    {
        if (condition)
        {
            dict.Add(new() { Source = new(uri, UriKind.Relative) });
        }

        return dict;
    }
}