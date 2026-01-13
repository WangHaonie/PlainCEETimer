using System;
using PlainCEETimer.Modules.Configuration;

namespace PlainCEETimer.Modules.Extensions;

public static class DateTimeExtensions
{
    public static string Format(this DateTime dateTime)
        => dateTime.ToString(ConfigValidator.DateTimeFormat);

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

    public static string Format(this TimeSpan timeSpan)
        => timeSpan.ToString("d'天'h'时'm'分's'秒'");
}