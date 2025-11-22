using System.Collections.Generic;
using System.Windows.Forms;
using PlainCEETimer.Countdown;
using PlainCEETimer.Modules.Extensions;
using PlainCEETimer.UI.Controls;

namespace PlainCEETimer.UI.Dialogs;

public sealed class RulesManager : ListViewDialog<Rule, RuleDialog>
{
    public Rule[] GlobalRules { get; set; }

    protected override Rule[] DefaultData => GlobalRules;

    private readonly List<Rule> gRules = new(3);

    public RulesManager()
        : base(460, ["时刻", "效果预览"], Ph.RuleTypes, "规则")
    {
        Text = "管理自定义规则 - 高考倒计时";
    }

    protected override int GetGroupIndex(Rule data)
    {
        return (int)data.Phase;
    }

    protected override ListViewItem GetListViewItem(Rule data)
    {
        var tmp = data.Colors;
        var flag = data.IsDefault;
        var item = new ListViewItem(flag ? "默认规则" : data.Tick.Format()) { UseItemStyleForSubItems = false };

        if (flag)
        {
            item.ForeColor = ThemeManager.ShouldUseDarkMode ? Colors.DarkForeLinkDisabled : Colors.LightForeLinkDisabled;
        }

        item.SubItems.Add(data.Text, tmp.Fore, tmp.Back, null);
        return item;
    }

    protected override IListViewSubDialog<Rule> GetSubDialog(Rule data = null)
    {
        return new RuleDialog(data);
    }

    protected override bool OnCollectingData(Rule data)
    {
        if (data == null)
        {
            GlobalRules = [.. gRules];
            return false;
        }

        if (data.IsDefault)
        {
            gRules.Add(data);
            return true;
        }

        return false;
    }

    protected override bool OnRemovingData(Rule data)
    {
        return !data.IsDefault;
    }
}
