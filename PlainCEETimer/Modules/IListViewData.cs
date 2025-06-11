using System;

namespace PlainCEETimer.Modules
{
    public interface IListViewData<T> : IComparable<T>, IEquatable<T>
    {
        bool InternalEquals(T other);
    }
}
