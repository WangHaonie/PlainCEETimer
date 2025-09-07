using System;
using System.Drawing;
using Newtonsoft.Json;
using PlainCEETimer.Modules.JsonConverters;

namespace PlainCEETimer.Modules.Countdown;

[JsonConverter(typeof(ColorSetConverter))]
public readonly struct ColorPair(Color fore, Color back) : IEquatable<ColorPair>
{
    public Color Fore => fore;

    public Color Back => back;

    public bool Equals(ColorPair other)
    {
        return fore == other.Fore && back == other.Back;
    }

    public override bool Equals(object obj)
    {
        return Equals((ColorPair)obj);
    }

    public override int GetHashCode()
    {
        return fore.GetHashCode() + back.GetHashCode();
    }
}
