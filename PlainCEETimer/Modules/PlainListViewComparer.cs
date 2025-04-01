using System.Collections;
using System.Windows.Forms;

namespace PlainCEETimer.Modules
{
    public sealed class PlainListViewComparer<TData> : IComparer
        where TData : IListViewObject<TData>
    {
        public int Compare(object x, object y)
            => ((TData)((ListViewItem)x).Tag).CompareTo((TData)((ListViewItem)y).Tag);
    }
}
