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

    private readonly HashSet<Element> ItemsSet = [];

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

        if (ItemsSet.TryGetValue(Element.FromData(newData), out var actual) && existing.Equals(actual.Item))
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
