using System;
using System.Runtime.CompilerServices;

namespace PlainCEETimer.Modules.Extensions;

public static class NumericExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Clamp(this int value, int min, int max)
    {
        if (min > max) ThrowMinMaxException(min, max);
        if (value < min) return min;
        if (value > max) return max;
        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ClampSafe(this int value, int min, int max)
    {
        SwapIf(min > max, ref min, ref max);
        if (value < min) return min;
        if (value > max) return max;
        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Clamp(this double value, double min, double max)
    {
        if (min > max) ThrowMinMaxException(min, max);
        if (value < min) return min;
        if (value > max) return max;
        return value;
    }

    public static string Format(this double value)
    {
        return value.ToString("0.#");
    }

    private static void ThrowMinMaxException<T>(T min, T max)
        where T : struct
    {
        throw new ArgumentException($"min ({min}) > max ({max}) !");
    }

    extension<T>(T)
        where T : struct
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Swap(ref T a, ref T b)
        {
            (a, b) = (b, a);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SwapIf(bool condition, ref T a, ref T b)
        {
            if (condition)
            {
                Swap(ref a, ref b);
            }
        }
    }
}