using System;

namespace PlainCEETimer.UI;

public interface IListViewData<T> : IComparable<T>, IEquatable<T>
{
    bool Excluded { get; set; }

    bool InternalEquals(T other);
}
