using System;

namespace PlainCEETimer.UI
{
    public interface IListViewData<T> : IComparable<T>, IEquatable<T>
    {
        bool InternalEquals(T other);
    }
}
