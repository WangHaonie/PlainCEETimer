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
        bool IEqualityComparer<Element>.Equals(Element x, Element y)
        {
            return x.Data.Equals(y.Data);
        }

        int IEqualityComparer<Element>.GetHashCode(Element obj)
        {
            return obj.Data.GetHashCode();
        }
    }

    private readonly HashSet<Element> ItemsSet = new(new ItemSetComparer());

    public bool CanAdd(TData data)
    {
        return CanAddCore(data, out _);
    }

    public bool? CanEdit(TData newData, ListViewItem existing)
    {
        if (CanAddCore(newData, out var actual))
        {
            return true;
        }

        if (existing.Equals(actual.Item))
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

    private bool CanAddCore(TData data, out Element actualElement)
    {
        return !ItemsSet.TryGetValue(new(data, null), out actualElement);
    }
}
