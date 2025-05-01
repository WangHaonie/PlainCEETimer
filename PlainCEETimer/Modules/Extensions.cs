using System;
using System.Drawing;
using System.Linq;

namespace PlainCEETimer.Modules
{
    public static class Extensions
    {
        private static readonly char[] IllegalChars = [' ', '\n', '\r', '\t', '\v', '\f', '\b'];

        public static string RemoveIllegalChars(this string s)
            => new([.. s.Trim().Where(x => !IllegalChars.Contains(x))]);

        #region 来自网络
        /*

        截断字符串 参考:

        c# - How do I truncate a .NET string? - Stack Overflow
        https://stackoverflow.com/a/2776689

        */
        public static string Truncate(this string s, int MaxLength)
            => s?.Length > MaxLength ? s.Substring(0, MaxLength) + "..." : s;
        #endregion

        public static int ToInt32(this Color color)
            => -color.ToArgb();

        public static string Format(this DateTime dateTime)
            => dateTime.ToString("yyyy'-'MM'-'dd dddd HH':'mm':'ss");
    }
}
