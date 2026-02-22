using System.Collections.Generic;
using System.Text;
using PlainCEETimer.Modules.Extensions;
using PlainCEETimer.Modules.Linq;

namespace PlainCEETimer.Modules;

public class CliOption
{
    public string FirstOption => m_first;

    private bool init;
    private string m_first;
    private Dictionary<string, string> m_argsdic;
    private static readonly char[] m_idchars = ['/', '-'];

    public static CliOption Parse(string[] args)
    {
        var cp = new CliOption();
        cp.ParseInternal(args);
        return cp;
    }

    public string GetFirst()
    {
        return Get(m_first);
    }

    public string Get(string option)
    {
        if (init && m_argsdic.TryGetValue(option, out var v))
        {
            return v;
        }

        return null;
    }

    public bool Defined(string option)
    {
        return init && m_argsdic.ContainsKey(option);
    }

    public static string Quote(string arg)
    {
        /*
        
        转义命令行参数中 \ 或 " 参考：

        Environment.GetCommandLineArgs Method (System) | Microsoft Learn
        https://learn.microsoft.com/en-us/dotnet/api/system.environment.getcommandlineargs

        runtime/src/libraries/System.Private.CoreLib/src/System/PasteArguments.cs at main · dotnet/runtime
        https://github.com/dotnet/runtime/blob/e2a94a1b7a621ea9339ad4c4919d732b9f4aadf7/src/libraries/System.Private.CoreLib/src/System/PasteArguments.cs#L10-L79

         */

        if (arg == null)
        {
            return arg;
        }

        var length = arg.Length;

        if (length != 0 && ContainsNoWhitespaceOrQuotes(arg))
        {
            return arg;
        }

        var outer = IsOption(arg) ? @"""\""" : "\"";
        var sb = new StringBuilder().Append(outer);

        int i = 0;
        const char Quote = '"';
        const char Backslash = '\\';

        while (i < length)
        {
            var c = arg[i++];

            if (c == Backslash)
            {
                var numBackSlash = 1;

                while (i < length && arg[i] == Backslash)
                {
                    i++;
                    numBackSlash++;
                }

                if (i == length)
                {
                    sb.Append(Backslash, numBackSlash * 2);
                }
                else if (arg[i] == Quote)
                {
                    sb.Append(Backslash, numBackSlash * 2 + 1);
                    sb.Append(Quote);
                    i++;
                }
                else
                {
                    sb.Append(Backslash, numBackSlash);
                }
            }
            else if (c == Quote)
            {
                sb.Append(Backslash);
                sb.Append(Quote);
            }
            else
            {
                sb.Append(c);
            }
        }

        return sb.Append(outer).ToString();
    }

    private void ParseInternal(string[] args)
    {
        if (!args.IsNullOrEmpty())
        {
            m_argsdic = [];
            var length = args.Length;

            for (int i = 0; i < length; i++)
            {
                var arg = args[i];

                if (IsOption(arg))
                {
                    var op = arg.Substring(1).ToLower();
                    string value = null;

                    if (!string.IsNullOrEmpty(op))
                    {
                        if (i + 1 < length)
                        {
                            var next = args[i + 1];

                            if (!IsOption(next))
                            {
                                value = Unquote(next);
                                i++;
                            }
                        }

                        m_argsdic[op] = value;
                        m_first ??= op;
                    }
                }
            }

            init = true;
        }
    }

    private static bool IsOption(string arg)
    {
        if (!string.IsNullOrEmpty(arg))
        {
            foreach (var c in m_idchars)
            {
                if (arg.StartsWith(c))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private string Unquote(string arg)
    {
        if (!string.IsNullOrEmpty(arg)
            && arg.StartsWith('"') && arg.EndsWith('"'))
        {
            var length = arg.Length;

            if (length == 2)
            {
                return string.Empty;
            }

            return arg.Substring(1, length - 2);
        }

        return arg;
    }

    private static bool ContainsNoWhitespaceOrQuotes(string s)
    {
        var length = s.Length;

        for (int i = 0; i < length; i++)
        {
            var c = s[i];

            if (char.IsWhiteSpace(c) || c == '"')
            {
                return false;
            }
        }

        return true;
    }
}