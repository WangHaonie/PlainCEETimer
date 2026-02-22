using System.Collections.Generic;
using System.Runtime.CompilerServices;
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

    public static string Truncate(this string str, int maxLength, int rearLength)
    {
        var length = str.Length;

        if (str == null || length <= maxLength || rearLength > length)
        {
            return str;
        }

        return str.Substring(0, maxLength) + "..." + str.Substring(length - rearLength, rearLength);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool StartsWith(this string s, char value)
    {
        return s[0] == value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool EndsWith(this string s, char value)
    {
        return s[s.Length - 1] == value;
    }
}