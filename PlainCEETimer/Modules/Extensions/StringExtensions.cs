using System;
using System.Linq;

namespace PlainCEETimer.Modules.Extensions
{
    public static class StringExtensions
    {
        private static readonly char[] IllegalChars = [' ', '\n', '\r', '\t', '\v', '\f', '\b'];

        public static string ToMessage(this Exception ex)
            => $"\n\n错误信息: \n{ex.Message}\n\n错误详情: \n{ex}";

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
    }
}
