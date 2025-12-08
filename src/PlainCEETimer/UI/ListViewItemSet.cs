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
    }

    private readonly HashSet<Element> ItemsSet = [];

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
