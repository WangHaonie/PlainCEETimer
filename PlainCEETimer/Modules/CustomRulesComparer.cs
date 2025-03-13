using PlainCEETimer.Modules.Configuration;
using System.Collections;
using System.Windows.Forms;

namespace PlainCEETimer.Modules
{
    public class CustomRulesComparer : IComparer
    {
        public int Compare(object x, object y)
        {
            var Item1Data = (CustomRuleObject)((ListViewItem)x).Tag;
            var Item2Data = (CustomRuleObject)((ListViewItem)y).Tag;
            var Item1Phase = Item1Data.Phase;
            int RuleTypeOrder = Item1Phase.CompareTo(Item2Data.Phase);

            if (RuleTypeOrder != 0)
            {
                return RuleTypeOrder;
            }

            int ExamTickOrder = Item2Data.Tick.CompareTo(Item1Data.Tick);
            return Item1Phase == CountdownPhase.P3 ? -ExamTickOrder : ExamTickOrder;
        }
    }
}
