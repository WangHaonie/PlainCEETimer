using System.Collections.Generic;
using System.Text;

namespace PlainCEETimer.Modules.Extensions;

public static class StringExtensions
{
    private static readonly HashSet<char> _illegalChars = [' ', '\n', '\r', '\t', '\v', '\f', '\b'];

    public static string RemoveIllegalChars(this string s)
    {
        if (string.IsNullOrEmpty(s))
        {
            return s;
        }

        var sb = new StringBuilder(s.Length);

        for (int i = 0; i < s.Length; i++)
        {
            var c = s[i];

            if (!_illegalChars.Contains(c))
            {
                sb.Append(c);
            }
        }

        return sb.ToString();
    }

    /*

    截断字符串 参考:

    c# - How do I truncate a .NET string? - Stack Overflow
    https://stackoverflow.com/a/2776689

    */
    public static string Truncate(this string str, int maxLength)
    {
        if (str == null || str.Length <= maxLength)
        {
            return str;
        }

        return str.Substring(0, maxLength) + "...";
    }
}