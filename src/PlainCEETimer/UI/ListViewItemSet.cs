using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;

namespace PlainCEETimer.UI;

public class ListViewItemSet<TData>()
    where TData : IListViewData<TData>
{
    [DebuggerDisplay("{Data}")]
    private struct Element(TData data, ListViewItem item)
    {
        public TData Data = data;
        public ListViewItem Item = item;
    }

    private class ItemSetComparer : IEqualityComparer<Element>
    {
        private readonly IEqualityComparer<TData> Comparer = EqualityComparer<TData>.Default;

        bool IEqualityComparer<Element>.Equals(Element x, Element y)
        {
            return Comparer.Equals(x.Data, y.Data);
        }

        int IEqualityComparer<Element>.GetHashCode(Element obj)
        {
            return Comparer.GetHashCode(obj.Data);
        }
    }

    private readonly HashSet<Element> ItemsSet = new(new ItemSetComparer());

    public bool CanAdd(TData data)
    {
        return !ItemsSet.Contains(new(data, null));
    }

    public bool? CanEdit(TData newData, ListViewItem existing)
    {
        if (CanAdd(newData))
        {
            return true;
        }

        if (ItemsSet.TryGetValue(new(newData, null), out var actual) && existing.Equals(actual.Item))
        {
            return newData.InternalEquals(actual.Data) ? null : true;
        }

        return false;
    }

    public void Add(TData data, ListViewItem item)
    {
        ItemsSet.Add(new(data, item));
    }

    public void Remove(TData data)
    {
        ItemsSet.Remove(new(data, null));
    }

    public void Clear()
    {
        ItemsSet.Clear();
    }
}
