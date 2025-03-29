using System.Collections;
using System.Windows.Forms;

namespace PlainCEETimer.Modules
{
    public sealed class PlainListViewComparer<TObject> : IComparer
        where TObject : IListViewObject<TObject>
    {
        public int Compare(object x, object y)
            => ((TObject)((ListViewItem)x).Tag).CompareTo((TObject)((ListViewItem)y).Tag);
    }
}
