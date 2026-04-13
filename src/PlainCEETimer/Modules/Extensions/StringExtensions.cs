using System.Runtime.CompilerServices;

namespace PlainCEETimer.Modules.Extensions;

public static class StringExtensions
{
    public static string Clean(this string s)
    {
        if (string.IsNullOrEmpty(s))
        {
            return s;
        }

        return CleanCore(s);
    }

    /*

    截断字符串 参考:

    c# - How do I truncate a .NET string? - Stack Overflow
    https://stackoverflow.com/a/2776689

    */

    public static string Truncate(this string str, int maxLength, bool appendEllipses = true)
    {
        if (str == null || str.Length <= maxLength)
        {
            return str;
        }

        var result = str.Substring(0, maxLength);

        if (appendEllipses)
        {
            return result + "...";
        }

        return result;
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

    private static string CleanCore(string s)
    {
        var length = s.Length;
        var start = 0;

        while (start < length && char.IsWhiteSpace(s[start]))
        {
            start++;
        }

        if (start == length)
        {
            return string.Empty;
        }

        var end = length - 1;

        while (start <= end && char.IsWhiteSpace(s[end]))
        {
            end--;
        }

        var ws = -1;

        for (int i = start; i <= end; i++)
        {
            if (char.IsWhiteSpace(s[i]))
            {
                ws = i;
                break;
            }
        }

        var max = end - start + 1;

        if (ws == -1)
        {
            if (start == 0 && max == length)
            {
                return s;
            }

            return s.Substring(start, max);
        }

        using var cache = new ArrayCache<char>(max);
        var buffer = cache.Value;
        int count = ws - start;

        for (int i = 0; i < count; i++)
        {
            buffer[i] = s[start + i];
        }

        for (int i = ws; i <= end; i++)
        {
            char c = s[i];

            if (!char.IsWhiteSpace(c))
            {
                buffer[count++] = c;
            }
        }

        return new string(buffer, 0, count);
    }
}