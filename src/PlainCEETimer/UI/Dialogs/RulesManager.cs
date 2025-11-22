using System.Collections.Generic;
using System.Windows.Forms;
using PlainCEETimer.Countdown;
using PlainCEETimer.Modules.Extensions;
using PlainCEETimer.UI.Controls;

namespace PlainCEETimer.UI.Dialogs;

public sealed class RulesManager : ListViewDialog<CustomRule, RuleDialog>
{
    public CustomRule[] GlobalRules { get; set; }

    protected override CustomRule[] DefaultData => GlobalRules;

    private readonly List<CustomRule> gRules = new(3);

    public RulesManager()
        : base(460, ["时刻", "效果预览"], Ph.RuleTypes, "规则")
    {
        Text = "管理自定义规则 - 高考倒计时";
    }

    protected override int GetGroupIndex(CustomRule data)
    {
        return (int)data.Phase;
    }

    protected override ListViewItem GetListViewItem(CustomRule data)
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

    protected override IListViewSubDialog<CustomRule> GetSubDialog(CustomRule data = null)
    {
        return new RuleDialog() { Data = data };
    }

    protected override bool OnCollectingData(CustomRule data)
    {
        if (data == null)
        {
            GlobalRules = [.. gRules];
        }
        else if (data.IsDefault)
        {
            gRules.Add(data);
            return true;
        }

        return false;
    }

    protected override bool OnRemovingData(CustomRule data)
    {
        return !data.IsDefault;
    }
}
