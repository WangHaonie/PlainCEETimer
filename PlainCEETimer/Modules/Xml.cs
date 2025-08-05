using System;
using System.Xml.Linq;

namespace PlainCEETimer.Modules
{
    public static class Xml
    {
        public static XElement FormString(string raw)
        {
            return XDocument.Parse(raw).Root;
        }

        public static bool Check(this XElement top, string expectation, bool returnValueIfNull, params string[] nodes)
        {
            var ns = top.GetDefaultNamespace();
            var current = top;

            for (int i = 0; i < nodes.Length; i++)
            {
                current = current.Element(ns.GetName(nodes[i]));

                if (current == null)
                {
                    return returnValueIfNull || false;
                }
            }

            return current.Value.Equals(expectation, StringComparison.OrdinalIgnoreCase);
        }
    }
}
