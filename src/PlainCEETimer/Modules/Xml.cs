using System;
using System.Xml.Linq;

namespace PlainCEETimer.Modules;

public static class Xml
{
    public static XElement FormString(string raw)
    {
        return XDocument.Parse(raw).Root;
    }

    public static bool Check(this XElement top, string expectation, bool defaultValue, params string[] nodes)
    {
        var ns = top.GetDefaultNamespace();
        var current = top;

        foreach (var node in nodes)
        {
            current = current.Element(ns.GetName(node));

            if (current == null)
            {
                return defaultValue || false;
            }
        }

        return current.Value.Equals(expectation, StringComparison.OrdinalIgnoreCase);
    }
}
