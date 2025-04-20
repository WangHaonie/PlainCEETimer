using System.Collections;
using System.Windows.Forms;

namespace PlainCEETimer.Modules
{
    public sealed class ListViewItemComparer<T> : IComparer
        where T : IListViewObject<T>
    {
        public int Compare(object x, object y)
        {
            return ((T)((ListViewItem)x).Tag).CompareTo((T)((ListViewItem)y).Tag);
        }
    }
}
