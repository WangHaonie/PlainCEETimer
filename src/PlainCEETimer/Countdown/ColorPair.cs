using System;
using System.Drawing;
using Newtonsoft.Json;
using PlainCEETimer.Modules.JsonConverters;

namespace PlainCEETimer.Countdown;

[JsonConverter(typeof(ColorSetConverter))]
public readonly struct ColorPair(Color fore, Color back) : IEquatable<ColorPair>
{
    private readonly Color _fore = fore;
    private readonly Color _back = back;

    public Color Fore => _fore;
    public Color Back => _back;

    public bool Equals(ColorPair other)
    {
        return _fore == other._fore && _back == other._back;
    }

    public override bool Equals(object obj)
    {
        return Equals((ColorPair)obj);
    }

    public override int GetHashCode()
    {
        return _fore.GetHashCode() + _back.GetHashCode();
    }
}
