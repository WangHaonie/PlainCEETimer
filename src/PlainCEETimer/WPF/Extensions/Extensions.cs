using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using WFColor = System.Drawing.Color;
using WFPoint = System.Drawing.Point;
using WFSize = System.Drawing.Size;

namespace PlainCEETimer.WPF.Extensions;

public static class Extensions
{
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

    public static Collection<ResourceDictionary> AddResource(this Collection<ResourceDictionary> dict, string uri, bool condition = true)
    {
        if (condition)
        {
            dict.Add(new() { Source = new(uri, UriKind.Relative) });
        }

        return dict;
    }
}