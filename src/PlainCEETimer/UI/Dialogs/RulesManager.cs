using System;
using System.Windows.Forms;
using PlainCEETimer.Countdown;
using PlainCEETimer.Modules.Extensions;
using PlainCEETimer.UI.Controls;

namespace PlainCEETimer.UI.Dialogs;

public sealed class RulesManager : ListViewDialog<CustomRule, RuleDialog>
{
    public string[] CustomTextPreset { get; set; }
    public ColorPair[] ColorPresets { private get; set; }

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
        var item = new ListViewItem(data.Tick.Format()) { UseItemStyleForSubItems = false };
        item.SubItems.Add(data.Text, tmp.Fore, tmp.Back, null);
        return item;
    }

    protected override IListViewSubDialog<CustomRule> GetSubDialog(CustomRule data = null)
    {
        return new RuleDialog()
        {
            Data = data,
            GlobalColors = ColorPresets,
            GlobalTexts = CustomTextPreset
        };
    }

    protected override PlainButton AddButton(ControlBuilder b)
    {
        var button = b.Button("全局设置(&G)", ButtonGlobal_Click);
        button.Width = 90;
        return button;
    }

    private void ButtonGlobal_Click(object sender, EventArgs e)
    {
        var dialog = new CustomTextDialog()
        {
            CustomTexts = CustomTextPreset
        };

        if (dialog.ShowDialog() == DialogResult.OK)
        {
            CustomTextPreset = dialog.CustomTexts;
            UserChanged();
        }
    }
}
