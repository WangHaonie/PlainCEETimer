using System.Collections.Generic;

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
        CombineCore(value?.GetHashCode() ?? 0);
        return this;
    }

    public HashCode Add<T>(T[] value, IEqualityComparer<T> comparer)
    {
        CombineCore(new ArrayEqualityComparer<T>(comparer).GetHashCode(value));
        return this;
    }

    public int Combine()
    {
        return hash;
    }

    private void CombineCore(int hashCode)
    {
        hash = unchecked((hash * 397) ^ hashCode);
    }
}