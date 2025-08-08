using System;
using System.Windows.Forms;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Configuration;
using PlainCEETimer.Modules.Extensions;
using PlainCEETimer.UI.Controls;
using PlainCEETimer.UI.Extensions;

namespace PlainCEETimer.UI.Dialogs
{
    public sealed class RulesManager : ListViewDialog<CustomRuleObject, RuleDialog>
    {
        public string[] CustomTextPreset { get; set; }
        public ColorSetObject[] ColorPresets { private get; set; }

        public RulesManager()
            : base(460, ["时刻", "效果预览"], Constants.PhRuleTypes)
        {
            Text = "管理自定义规则 - 高考倒计时";
            ItemDescription = "规则";
        }

        protected override int GetGroupIndex(CustomRuleObject data)
        {
            return (int)data.Phase;
        }

        protected override ListViewItem GetListViewItem(CustomRuleObject data)
        {
            var tmp = data.Colors;
            var item = new ListViewItem(data.Tick.Format()) { UseItemStyleForSubItems = false };
            item.SubItems.Add(data.Text, tmp.Fore, tmp.Back, null);
            return item;
        }

        protected override IListViewSubDialog<CustomRuleObject> GetSubDialog(CustomRuleObject data = null)
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
            return b.Button("全局设置(&G)", ButtonGlobal_Click).With(x => x.SetBounds(0, 0, 90, 23, BoundsSpecified.Size));
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
}
