using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
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
        => dateTime.ToString("yyyy'-'MM'-'dd dddd HH':'mm':'ss");

    public static string Format(this TimeSpan timeSpan)
        => timeSpan.ToString("d'天'h'时'm'分's'秒'");

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

    */
    public static DateTime TruncateToSeconds(this DateTime dt)
        => new(dt.Ticks / Validator.MinTick * Validator.MinTick);

    public static int ToWin32(this Color color)
        => ColorTranslator.ToWin32(color);

    public static Color ToColor(this int value)
        => ColorTranslator.FromOle(value);

    public static void Destory(this IDisposable obj)
        => obj?.Dispose();

    public static T[] Copy<T>(this T[] array)
        => (T[])array.Clone();

    public static void PopulateWith<T>(this T[] destination, T[] source)
    {
        if (source != null)
        {
            Array.Copy(source, destination, Math.Min(source.Length, destination.Length));
        }
    }
}