using System.Collections.Generic;

namespace PlainCEETimer.Modules;

internal static class ObjectComparer<T>
{
    internal static readonly ObjectAscendingComparer AscendingComparer = new();
    internal static readonly ObjectDescendingComparer DescendingComparer = new();

    private static readonly Comparer<T> comparer = Comparer<T>.Default;

    internal class ObjectAscendingComparer : IComparer<T>
    {
        int IComparer<T>.Compare(T x, T y)
        {
            return comparer.Compare(x, y);
        }
    }

    internal class ObjectDescendingComparer : IComparer<T>
    {
        int IComparer<T>.Compare(T x, T y)
        {
            return comparer.Compare(y, x);
        }
    }
}