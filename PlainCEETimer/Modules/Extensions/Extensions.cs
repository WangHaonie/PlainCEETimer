using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PlainCEETimer.Modules.Extensions
{
    public static class Extensions
    {
        private static readonly char[] IllegalChars = [' ', '\n', '\r', '\t', '\v', '\f', '\b'];

        public static int ToInt32(this Color color)
            => -color.ToArgb();

        public static string RemoveIllegalChars(this string s)
            => new([.. s.Trim().Where(x => !IllegalChars.Contains(x))]);

        public static string Format(this DateTime dateTime)
            => dateTime.ToString("yyyy'-'MM'-'dd dddd HH':'mm':'ss");

        public static Task Start(this Action start, Action<Task> jobAfterStart = null)
            => jobAfterStart == null ? Task.Run(start) : Task.Run(start).ContinueWith(jobAfterStart);

        public static Task AsDelay(this int ms, Action jobAfterDelay, Control ui)
            => Task.Delay(ms).ContinueWith(_ => ui.BeginInvoke(jobAfterDelay));

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
        public static DateTime TruncateToSecond(this DateTime dt)
            => new(dt.Ticks / Validator.MinTick * Validator.MinTick);
    }
}
