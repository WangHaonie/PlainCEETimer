using PlainCEETimer.Modules.Configuration;
using System.Collections;
using System.Windows.Forms;

namespace PlainCEETimer.Modules
{
    public class CustomRulesComparer : IComparer
    {
        public int Compare(object x, object y)
            => ((CustomRuleObject)((ListViewItem)x).Tag).CompareTo((CustomRuleObject)((ListViewItem)y).Tag);
    }
}
