using System;
using System.Drawing;
using Newtonsoft.Json;
using PlainCEETimer.Modules.JsonConverters;

namespace PlainCEETimer.Countdown;

[JsonConverter(typeof(ColorSetConverter))]
public readonly struct ColorPair(Color fore, Color back) : IEquatable<ColorPair>
{
    public Color Fore => foreColor;

    public Color Back => backColor;

    private readonly Color foreColor = fore;
    private readonly Color backColor = back;

    public bool Equals(ColorPair other)
    {
        return foreColor == other.foreColor && backColor == other.backColor;
    }

    public override bool Equals(object obj)
    {
        return Equals((ColorPair)obj);
    }

    public override int GetHashCode()
    {
        return foreColor.GetHashCode() + backColor.GetHashCode();
    }
}
