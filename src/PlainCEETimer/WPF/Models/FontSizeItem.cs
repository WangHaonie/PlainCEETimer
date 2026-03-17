using System;
using System.Collections.Generic;
using PlainCEETimer.Modules.Extensions;

namespace PlainCEETimer.WPF.Models;

public class FontSizeItem(double value)
{
    public string Display => value.Format();

    public double SizePt => value;

    public static IEnumerable<FontSizeItem> Yield(double minSize, double maxSize, double step)
    {
        if (minSize <= 0 || maxSize <= 0)
        {
            throw new InvalidOperationException();
        }

        if (minSize > maxSize)
        {
            (minSize, maxSize) = (maxSize, minSize);
        }

        var current = minSize;

        yield return new(current);

        while (current + step < maxSize)
        {
            current += step;
            yield return new(current);
        }

        if (Math.Abs(current - maxSize) > 1e-9)
        {
            yield return new(maxSize);
        }
    }
}