using System;
using System.Drawing;
using Newtonsoft.Json;
using PlainCEETimer.Modules.JsonConverters;

namespace PlainCEETimer.Modules.Configuration;

[JsonConverter(typeof(ColorSetConverter))]
public readonly struct ColorSetObject(Color fore, Color back) : IEquatable<ColorSetObject>
{
    public Color Fore => fore;

    public Color Back => back;

    public bool Equals(ColorSetObject other)
    {
        return fore == other.Fore && back == other.Back;
    }

    public override bool Equals(object obj)
    {
        return Equals((ColorSetObject)obj);
    }

    public override int GetHashCode()
    {
        return fore.GetHashCode() + back.GetHashCode();
    }
}
