using System.Windows.Forms;
using PlainCEETimer.Countdown;
using PlainCEETimer.Modules.Extensions;
using PlainCEETimer.UI.Controls;

namespace PlainCEETimer.UI.Dialogs;

public sealed class RulesManager : ListViewDialog<CountdownRule, RuleDialog>
{
    private readonly bool UseDark = ThemeManager.ShouldUseDarkMode;

    public RulesManager()
        : base(460, ["时刻", "效果预览"], Ph.RuleTypes, "规则")
    {
        Text = "管理自定义规则 - 高考倒计时";
    }

    protected override int GetGroupIndex(CountdownRule data)
    {
        return (int)data.Phase;
    }

    protected override ListViewItem GetListViewItem(CountdownRule data)
    {
        var tmp = data.Colors;
        var item = new ListViewItem(data.Tick.Format()) { UseItemStyleForSubItems = false };
        item.SubItems.Add(data.Text, tmp.Fore, tmp.Back, null);
        return item;
    }

    protected override IListViewChildDialog<CountdownRule> GetChildDialog(CountdownRule data = null)
    {
        CountdownRule[] p = null;

        if (data == null || !data.Default)
        {
            p = FixedData;
        }

        return new RuleDialog(data, p);
    }
}
