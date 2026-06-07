using System.Runtime.CompilerServices;

namespace PlainCEETimer.Modules.Extensions;

public static class NumericExtensions
{
    public static int Clamp(this int value, int min, int max)
    {
        SwapIf(min > max, ref min, ref max);

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
        SwapIf(min > max, ref min, ref max);

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

    extension<T>(T)
        where T : struct
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Swap(ref T a, ref T b)
        {
            (a, b) = (b, a);
        }

        public static void SwapIf(bool condition, ref T a, ref T b)
        {
            if (condition)
            {
                Swap(ref a, ref b);
            }
        }
    }
}