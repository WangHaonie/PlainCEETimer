using System;
using System.Collections.Generic;

namespace PlainCEETimer.WPF.Models;

public class FontSizeModel(double value)
{
    public string Display => value.ToString();

    public double Size => value;

    public static IEnumerable<FontSizeModel> Yield(double minSize, double maxSize, double step)
    {
        if (minSize <= 0 || maxSize <= 0)
        {
            throw new InvalidOperationException();
        }

        if (minSize == maxSize)
        {
            yield return new(minSize);
        }

        if (minSize > maxSize)
        {
            (minSize, maxSize) = (maxSize, minSize);
        }

        double current = minSize;

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