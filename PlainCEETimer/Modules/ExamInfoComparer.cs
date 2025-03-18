using PlainCEETimer.Modules.Configuration;
using System.Collections;
using System.Windows.Forms;

namespace PlainCEETimer.Modules
{
    public sealed class ExamInfoComparer : IComparer
    {
        public int Compare(object x, object y)
            => ((ExamInfoObject)((ListViewItem)x).Tag).CompareTo((ExamInfoObject)((ListViewItem)y).Tag);
    }
}
