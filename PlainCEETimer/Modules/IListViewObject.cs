using System;

namespace PlainCEETimer.Modules
{
    public interface IListViewObject<TData> : IComparable<TData>, IEquatable<TData>
    {
        bool CanExecute();
    }
}
