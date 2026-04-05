using System.Collections.Generic;

namespace PlainCEETimer.Modules;

public class ObjectComparer<T>
{
    public static readonly ObjectAscendingComparer AscendingComparer = new();
    public static readonly ObjectDescendingComparer DescendingComparer = new();

    private static readonly Comparer<T> comparer = Comparer<T>.Default;

    public class ObjectAscendingComparer : IComparer<T>
    {
        public int Compare(T x, T y)
        {
            return comparer.Compare(x, y);
        }
    }

    public class ObjectDescendingComparer : IComparer<T>
    {
        public int Compare(T x, T y)
        {
            return comparer.Compare(y, x);
        }
    }
}