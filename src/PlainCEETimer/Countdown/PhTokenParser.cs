using System;
using System.Collections.Generic;

namespace PlainCEETimer.Countdown;

public static class PhTokenParser
{
    private static readonly Dictionary<string, PhParsedTokenCollection> _cache = new(StringComparer.Ordinal);

    public static PhParsedTokenCollection Parse(string format)
    {
        if (string.IsNullOrEmpty(format))
        {
            return [];
        }

        if (_cache.TryGetValue(format, out var parsed))
        {
            return parsed;
        }

        var tokens = new PhParsedTokenCollection();
        var length = format.Length;
        var i = 0;
        var j = 0;

        while (i < length)
        {
            var phstart = format.IndexOf('{', i);

            if (phstart < 0)
            {
                break;
            }

            var phend = format.IndexOf('}', phstart + 1);

            if (phend < 0)
            {
                break;
            }

            var lphstart = format.LastIndexOf('{', phend - 1, phend - phstart);
            var key = format.Substring(lphstart + 1, phend - lphstart - 1);
            var type = GetToken(key);

            if (type != PhToken.None)
            {
                if (lphstart > j)
                {
                    tokens.Add(new() { Value = format.Substring(j, lphstart - j) });
                }

                tokens.Add(new() { Token = type });
                i = phend + 1;
                j = i;
            }
            else
            {
                i = lphstart + 1;
            }
        }

        if (j < length)
        {
            tokens.Add(new() { Value = format.Substring(j) });
        }

        _cache[format] = tokens;
        return tokens;
    }

    private static PhToken GetToken(string key) => key switch
    {
        "x" => PhToken.ExamName,
        "d" => PhToken.Days,
        "dd" => PhToken.DecimalDays,
        "cd" => PhToken.CeilingDays,
        "h" => PhToken.Hours,
        "th" => PhToken.TotalHours,
        "dh" => PhToken.DecimalHours,
        "m" => PhToken.Minutes,
        "tm" => PhToken.TotalMinutes,
        "s" => PhToken.Seconds,
        "ts" => PhToken.TotalSeconds,
        "ht" => PhToken.Hint,
        _ => PhToken.None
    };
}
