using System;
using System.Runtime.CompilerServices;

namespace PlainCEETimer.Modules.Extensions;

public static class StringExtensions
{
    extension(string)
    {
        /*
        
        基于 ReadOnlySpan 的 string.Concat 参考：

        runtime/src/libraries/System.Private.CoreLib/src/System/String.Manipulation.cs at 62404420e54ff7257b87ca8cebb97673d4c42b84 · dotnet/runtime
        https://github.com/dotnet/runtime/blob/62404420e54ff7257b87ca8cebb97673d4c42b84/src/libraries/System.Private.CoreLib/src/System/String.Manipulation.cs#L287-L377

         */

        public unsafe static string Concat(ReadOnlySpan<char> str0, ReadOnlySpan<char> str1)
        {
            var len0 = str0.Length;
            int length = len0 + str1.Length;

            if (length == 0)
            {
                return string.Empty;
            }

            var result = StringInternals.FastAllocateString(length);

            fixed (char* ptr = result)
            {
                var span = new Span<char>(ptr, length);
                str0.CopyTo(span);
                str1.CopyTo(span.Slice(len0));
            }

            return result;
        }

        public unsafe static string Concat(ReadOnlySpan<char> str0, ReadOnlySpan<char> str1, ReadOnlySpan<char> str2)
        {
            var len0 = str0.Length;
            var len1 = str1.Length;
            int length = len0 + len1 + str2.Length;

            if (length == 0)
            {
                return string.Empty;
            }

            var result = StringInternals.FastAllocateString(length);

            fixed (char* ptr = result)
            {
                var span = new Span<char>(ptr, length);
                str0.CopyTo(span);
                span = span.Slice(len0);
                str1.CopyTo(span);
                span = span.Slice(len1);
                str2.CopyTo(span);
            }

            return result;
        }
    }

    public static string Clean(this string s)
    {
        if (string.IsNullOrEmpty(s))
        {
            return s;
        }

        return CleanCore(s);
    }

    public static string Compact(this string s)
    {
        if (string.IsNullOrEmpty(s))
        {
            return s;
        }

        return CompactCore(s);
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

        var result = str.AsSpan(0, maxLength);

        if (appendEllipses)
        {
            return string.Concat(result, "...");
        }

        return result.ToString();
    }

    public static string Truncate(this string str, int maxLength, int rearLength)
    {
        var length = str.Length;

        if (str == null || length <= maxLength || rearLength > length)
        {
            return str;
        }

        return string.Concat(str.AsSpan(0, maxLength), "...", str.AsSpan(length - rearLength, rearLength));
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

        using var cache = new ArrayCache<char>(max, out var buffer);
        var count = ws - start;

        for (int i = 0; i < count; i++)
        {
            buffer[i] = s[start + i];
        }

        for (int i = ws; i <= end; i++)
        {
            var c = s[i];

            if (!char.IsWhiteSpace(c))
            {
                buffer[count++] = c;
            }
        }

        return new string(buffer, 0, count);
    }

    private static string CompactCore(string s)
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
        var flws = false;

        for (int i = start; i <= end; i++)
        {
            var fws = char.IsWhiteSpace(s[i]);

            if (fws && flws)
            {
                ws = i;
                break;
            }

            flws = fws;
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

        using var cache = new ArrayCache<char>(max, out var buffer);
        var count = ws - start;

        for (int i = 0; i < count; i++)
        {
            buffer[i] = s[start + i];
        }

        flws = true;

        for (int i = ws; i <= end; i++)
        {
            var c = s[i];
            var fws = char.IsWhiteSpace(c);

            if (fws)
            {
                if (flws)
                {
                    continue;
                }

                flws = true;
            }
            else
            {
                flws = false;
            }

            buffer[count++] = c;
        }

        return new string(buffer, 0, count);
    }
}