using System;
using System.Collections.Generic;
using PlainCEETimer.Modules.Extensions;

namespace PlainCEETimer.WPF.Models;

public class FontSizeItem(double value)
{
    public string Display => value.Format();

    public double SizePt => value;

    public static IEnumerable<FontSizeItem> Yield(double min, double max, double step)
    {
        if (min <= 0 || max <= 0)
        {
            throw new InvalidOperationException();
        }

        if (min > max)
        {
            (min, max) = (max, min);
        }

        var current = min;

        yield return new(current);

        while (current + step < max)
        {
            current += step;
            yield return new(current);
        }

        if (Math.Abs(current - max) > 1e-9)
        {
            yield return new(max);
        }
    }
}