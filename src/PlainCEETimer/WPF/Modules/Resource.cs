using System;
using System.Windows;

namespace PlainCEETimer.WPF.Modules;

public static class Resource
{
    public static ResourceDictionary Create(string uri)
    {
        return new() { Source = new(uri, UriKind.Relative) };
    }
}