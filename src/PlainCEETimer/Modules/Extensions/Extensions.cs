using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Windows.Forms;
using PlainCEETimer.Countdown;
using PlainCEETimer.Modules.Configuration;

namespace PlainCEETimer.Modules.Extensions;

public static class Extensions
{
    private static readonly char[] IllegalChars = [' ', '\n', '\r', '\t', '\v', '\f', '\b'];

    public static int ToInt32(this Color color)
        => -color.ToArgb();

    public static string RemoveIllegalChars(this string s)
        => new([.. s.Trim().Where(x => !IllegalChars.Contains(x))]);

    public static string Format(this DateTime dateTime)
        => dateTime.ToString(Validator.DateTimeFormat);

    public static string Format(this TimeSpan timeSpan)
        => timeSpan.ToString("d'天'h'时'm'分's'秒'");

    public static string Format(this Font font)
        => $"{font.Name}, {font.Size}pt, {font.Style}";

    public static Task Start(this Action start, Action<Task> after = null)
        => after == null ? Task.Run(start) : Task.Run(start).ContinueWith(after);

    public static Task AsDelay(this int ms, Action after, Control ui)
        => Task.Delay(ms).ContinueWith(_ => ui.BeginInvoke(after));

    /*

    截断字符串 参考:

    c# - How do I truncate a .NET string? - Stack Overflow
    https://stackoverflow.com/a/2776689

    */
    public static string Truncate(this string s, int maxLength)
        => s?.Length > maxLength ? s.Substring(0, maxLength) + "..." : s;

    /*

    将 DateTime 精确至秒而不是 Tick 参考：

    c# - How do I truncate milliseconds off "Ticks" without converting to datetime? - Stack Overflow
    https://stackoverflow.com/a/35018359

    C# DateTime 精确到秒/截断毫秒部分 - eshizhan - 博客园
    https://www.cnblogs.com/eshizhan/archive/2011/11/15/2250007.html

    */
    public static DateTime TruncateToSeconds(this DateTime dt)
        => new(dt.Ticks / Validator.MinTick * Validator.MinTick);

    public static int ToWin32(this Color color)
        => ColorTranslator.ToWin32(color);

    public static int ToWin32(this bool b)
        => b ? 1 : 0;

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
        else if (value > max)
        {
            return max;
        }

        return value;
    }

    public static bool ToBool(this int i)
        => i != 0;

    public static Color ToColor(this int value)
        => ColorTranslator.FromOle(value);

    public static void Destory(this IDisposable obj)
        => obj?.Dispose();

    public static T Copy<T>(this T obj) where T : ICloneable
        => (T)obj.Clone();

    public static void PopulateWith<T>(this T[] destination, T[] source)
    {
        if (source != null)
        {
            Array.Copy(source, destination, Math.Min(source.Length, destination.Length));
        }
    }

    public static bool IsNullOrEmpty<T>(this T[] arr)
        => arr == null || arr.Length == 0;

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

    public static bool ArrayEquals<T>(this T[] arr1, T[] arr2, IEqualityComparer<T> comparer = null)
    {
        // Enumerable.SequenceEqual 基于 IEnumerator，在比较数组类型时，速度可能略慢，故编写此方法专用于数组序列的比较。

        if (arr1 == null || arr2 == null || arr1.Length != arr2.Length)
        {
            return false;
        }

        if (ReferenceEquals(arr1, arr2))
        {
            return true;
        }

        comparer ??= EqualityComparer<T>.Default;

        for (int i = 0; i < arr1.Length; i++)
        {
            if (!comparer.Equals(arr1[i], arr2[i]))
            {
                return false;
            }
        }

        return true;
    }
}