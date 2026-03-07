namespace PlainCEETimer.Modules.Extensions;

public static class NumericExtensions
{
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

    public static double Clamp(this double value, double min, double max)
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

    public static string Format(this double value)
    {
        return value.ToString("0.#");
    }
}