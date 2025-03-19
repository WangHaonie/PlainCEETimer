using System;
using System.Collections;
using System.Windows.Forms;

namespace PlainCEETimer.Modules
{
    public sealed class PlainComparer<TObject> : IComparer where TObject : IComparable<TObject>
    {
        public int Compare(object x, object y)
            => ((TObject)((ListViewItem)x).Tag).CompareTo((TObject)((ListViewItem)y).Tag);
    }
}
