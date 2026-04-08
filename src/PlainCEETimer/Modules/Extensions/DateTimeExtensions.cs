using System;
using PlainCEETimer.Modules.Configuration;

namespace PlainCEETimer.Modules.Extensions;

public static class DateTimeExtensions
{
    public static long ToTimestamp(this DateTime dt)
        => (dt.Ticks / ConfigValidator.MinTick) - ConfigValidator.MinDateSeconds;

    public static DateTime ToDateTime(this long timestamp)
        => new((timestamp + ConfigValidator.MinDateSeconds) * ConfigValidator.MinTick);

    /*

    将 DateTime 精确至秒而不是 Tick 参考：

    c# - How do I truncate milliseconds off "Ticks" without converting to datetime? - Stack Overflow
    https://stackoverflow.com/a/35018359

    C# DateTime 精确到秒/截断毫秒部分 - eshizhan - 博客园
    https://www.cnblogs.com/eshizhan/archive/2011/11/15/2250007.html

    */
    public static DateTime TruncateToSeconds(this DateTime dt)
        => new(dt.Ticks / ConfigValidator.MinTick * ConfigValidator.MinTick);

    public static string Format(this DateTime dateTime)
        => dateTime.ToString(ConfigValidator.DateTimeFormat);

    public static string Format(this TimeSpan timeSpan)
        => timeSpan.ToString("d'天'h'时'm'分's'秒'");

    public static string FormatToTimeAgo(this DateTime dt) => ProcessTimeSpan(DateTime.Now - dt, out var str) switch
    {
        DurationUnit.Seconds => "刚刚",
        DurationUnit.Minutes => str + " 分钟前",
        DurationUnit.Hours => str + " 小时前",
        _ => str + " 天前"
    };

    public static string FormatToDuration(this TimeSpan span) => ProcessTimeSpan(span, out var str) switch
    {
        DurationUnit.Seconds => str + " 秒",
        DurationUnit.Minutes => str + " 分钟",
        DurationUnit.Hours => str + " 小时",
        _ => str + " 天"
    };

    public static string FormatToDurationWhen(this TimeSpan span, bool condition, string defaultValue = null)
    {
        if (condition)
        {
            return FormatToDuration(span);
        }

        return defaultValue;
    }

    private static DurationUnit ProcessTimeSpan(TimeSpan span, out string str)
    {
        var ts = span.TotalSeconds;
        var tm = span.TotalMinutes;
        var th = span.TotalHours;
        var td = span.TotalDays;

        if (ts < 60.0)
        {
            str = ts.Format();
            return DurationUnit.Seconds;
        }

        if (tm < 60.0)
        {
            str = tm.Format();
            return DurationUnit.Minutes;
        }

        if (th < 24.0)
        {
            str = th.Format();
            return DurationUnit.Hours;
        }

        str = td.Format();
        return DurationUnit.Days;
    }
}