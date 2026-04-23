using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace PlainCEETimer.Modules;

public class HashCode
{
    private int hash = 17;

    public HashCode()
    {
        Add(23);
    }

    public HashCode Add<T>(T value)
    {
        return CombineCore(value?.GetHashCode() ?? 0);
    }

    public HashCode Add<T>(T[] value, IEqualityComparer<T> comparer)
    {
        return CombineCore(new ArrayEqualityComparer<T>(comparer).GetHashCode(value));
    }

    public int Combine()
    {
        return hash;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private HashCode CombineCore(int hashCode)
    {
        hash = unchecked((hash * 397) ^ hashCode);
        return this;
    }
}