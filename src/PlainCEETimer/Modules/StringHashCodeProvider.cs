using System;

namespace PlainCEETimer.Modules;

public readonly struct StringHashCodeProvider(string s) : IEquatable<StringHashCodeProvider>
{
    public string Value => s;

    private readonly int _hashCode = (s ?? string.Empty).GetHashCode();

    public bool Equals(StringHashCodeProvider other)
    {
        return _hashCode == other._hashCode;
    }

    public override bool Equals(object obj)
    {
        return obj is StringHashCodeProvider s && Equals(s);
    }

    public override int GetHashCode()
    {
        return _hashCode;
    }

    public override string ToString()
    {
        return s;
    }

    public static bool operator ==(StringHashCodeProvider left, StringHashCodeProvider right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(StringHashCodeProvider left, StringHashCodeProvider right)
    {
        return !(left == right);
    }

    public static implicit operator StringHashCodeProvider(string value)
    {
        return new(value);
    }
}