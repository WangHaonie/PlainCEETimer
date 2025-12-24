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
        hash = unchecked((hash * 397) ^ (value?.GetHashCode() ?? 0));
        return this;
    }

    public int Combine()
    {
        return hash;
    }
}