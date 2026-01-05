using System;

namespace PlainCEETimer.UI;

public interface IListViewData<T> : IComparable<T>, IEquatable<T>, ICloneable
{
    bool Excluded { get; set; }

    bool InternalEquals(T other);
}
