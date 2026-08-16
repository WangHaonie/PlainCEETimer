using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace PlainCEETimer.Modules;

[DebuggerDisplay("{_value}")]
public readonly struct StringHashCodeProvider : IEquatable<StringHashCodeProvider>
{
    private class StringOrdinalComparer : IEqualityComparer<StringHashCodeProvider>
    {
        public bool Equals(StringHashCodeProvider x, StringHashCodeProvider y)
        {
            if (x._hashCode != y._hashCode)
            {
                return false;
            }

            return x._value == y._value;
        }

        public int GetHashCode(StringHashCodeProvider obj)
        {
            return obj._hashCode;
        }
    }

    public string Value => _value;

    public static IEqualityComparer<StringHashCodeProvider> OrdinalComparer => field ??= new StringOrdinalComparer();

    private readonly string _value;
    private readonly int _hashCode;

    public StringHashCodeProvider(string s)
    {
        _value = s ?? string.Empty;
        _hashCode = _value.GetHashCode();
    }

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
        return _value;
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