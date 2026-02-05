using System;
using System.Drawing;
using System.Runtime.Serialization;
using System.Windows.Forms;
using PlainCEETimer.Countdown;

namespace PlainCEETimer.Modules.Extensions;

public static class Extensions
{
    public static bool AsBool(this DialogResult dr)
        => dr is DialogResult.OK or DialogResult.Yes;

    public static int ToInt32(this Color color)
        => -color.ToArgb();

    public static Color ToColor(this int value)
        => ColorTranslator.FromOle(value);

    public static string Format(this Font font)
        => $"{font.Name}, {font.Size}pt, {font.Style}";

    public static int Clamp(this int value, int min, int max)
    {
        if (min > max)
        {
            (min, max) = (max, min);
        }

        if (value < min)
        {
            return min;
        }

        if (value > max)
        {
            return max;
        }

        return value;
    }

    public static void Destory(this IDisposable obj)
        => obj?.Dispose();

    public static T Copy<T>(this T obj) where T : ICloneable
        => (T)obj.Clone();

    public static bool IsEnabled(this ExamSettings settings)
        => settings != null && settings.Enabled;

    public static StreamingContext SetContext<T>(this StreamingContext sc, T value, out StreamingContext original)
    {
        original = sc;
        return new(sc.State, value);
    }

    public static bool CheckContext<T>(this StreamingContext sc, T expectation)
    {
        var context = sc.Context;

        if (context != null)
        {
            return context.Equals(expectation);
        }

        return false;
    }
}