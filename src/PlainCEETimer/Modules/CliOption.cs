using System.Collections.Generic;
using PlainCEETimer.Modules.Extensions;
using PlainCEETimer.Modules.Linq;

namespace PlainCEETimer.Modules;

public class CliOption
{
    public string FirstOption => m_first;

    private bool init;
    private string m_first;
    private Dictionary<string, string> m_argsdic;
    private readonly char[] m_idchars = ['/', '-'];

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

    public static string Quote(string str)
    {
        if (str != null)
        {
            return "\"^" + str + "\"";
        }

        return str;
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

                if (arg.StartsWith(m_idchars))
                {
                    var op = arg.Substring(1).ToLower();
                    string value = null;

                    if (!string.IsNullOrEmpty(op))
                    {
                        if (i + 1 < length)
                        {
                            var next = args[i + 1];

                            if (!next.StartsWith(m_idchars))
                            {
                                value = next.StartsWith('^') ? next.Substring(1) : next;
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
}