using System.Collections.Generic;
using System.Windows.Forms;

namespace PlainCEETimer.UI;

public class ListViewItemSet<TData>()
    where TData : IListViewData<TData>
{
    private struct Element(TData data, ListViewItem item)
    {
        public TData Data = data;
        public ListViewItem Item = item;

        public static Element FromData(TData data)
        {
            return new(data, null);
        }
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
        return !ItemsSet.Contains(Element.FromData(data));
    }

    public bool? CanEdit(TData newData, ListViewItem existing)
    {
        if (CanAdd(newData))
        {
            return true;
        }

        ItemsSet.TryGetValue(Element.FromData(newData), out Element actual);

        if (existing == actual.Item)
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
        ItemsSet.Remove(Element.FromData(data));
    }

    public void Clear()
    {
        ItemsSet.Clear();
    }
}
