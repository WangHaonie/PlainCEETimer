using System.Collections.Generic;
using System.Windows.Forms;

namespace PlainCEETimer.UI
{
    public class ListViewItemSet<TData>
        where TData : IListViewData<TData>
    {
        private struct Entry
        {
            public TData Data;
            public ListViewItem Item;

            public Entry(TData data)
            {
                Data = data;
            }

            public Entry(TData data, ListViewItem item)
            {
                Data = data;
                Item = item;
            }
        }

        private class ItemSetComparer : IEqualityComparer<Entry>
        {
            private readonly IEqualityComparer<TData> Comparer = EqualityComparer<TData>.Default;

            bool IEqualityComparer<Entry>.Equals(Entry x, Entry y)
            {
                return Comparer.Equals(x.Data, y.Data);
            }

            int IEqualityComparer<Entry>.GetHashCode(Entry obj)
            {
                return Comparer.GetHashCode(obj.Data);
            }
        }

        private readonly HashSet<Entry> ItemsSet = new(new ItemSetComparer());

        public bool CanAdd(TData data)
        {
            return !ItemsSet.Contains(new(data));
        }

        public bool? CanEdit(TData newData, ListViewItem existing)
        {
            if (CanAdd(newData))
            {
                return true;
            }

            ItemsSet.TryGetValue(new(newData), out Entry actual);

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
            ItemsSet.Remove(new(data));
        }
    }
}
