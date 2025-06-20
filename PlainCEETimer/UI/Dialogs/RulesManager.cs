using System;
using System.Windows.Forms;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Configuration;
using PlainCEETimer.UI.Controls;

namespace PlainCEETimer.UI.Dialogs
{
    public sealed class RulesManager : ListViewDialog<CustomRuleObject, RuleDialog>
    {
        public string[] CustomTextPreset { get; set; }
        public ColorSetObject[] ColorPresets { private get; set; }

        private PlainButton ButtonGlobal;

        public RulesManager()
            : base(480, ["时刻", "效果预览"], [Constants.PH_RTP1, Constants.PH_RTP2, Constants.PH_RTP3])
        {
            Text = "管理自定义规则 - 高考倒计时";
            ItemDescription = "规则";
        }

        protected override void OnInitializing()
        {
            base.OnInitializing();

            this.AddControls(b =>
            [
                ButtonGlobal = b.Button("全局设置(&G)", ButtonGlobal_Click).With(x => x.SetBounds(0, 0, 90, 23, BoundsSpecified.Size))
            ]);
        }

        protected override void StartLayout(bool isHighDpi)
        {
            base.StartLayout(isHighDpi);
            ArrangeControlXT(ButtonGlobal, ButtonOperation, 3);
        }

        protected override ListViewItem GetListViewItem(CustomRuleObject data)
        {
            var tmp = data.Colors;
            var item = new ListViewItem(GetTickText(data.Tick)) { UseItemStyleForSubItems = false };
            item.SubItems.Add(data.Text, tmp.Fore, tmp.Back, null);
            return item;
        }

        protected override int GetGroupIndex(CustomRuleObject data)
        {
            return (int)data.Phase;
        }

        protected override IListViewSubDialog<CustomRuleObject> GetSubDialog(CustomRuleObject data = null) => new RuleDialog()
        {
            Data = data,
            GlobalColors = ColorPresets,
            GlobalTexts = CustomTextPreset
        };

        private void ButtonGlobal_Click(object sender, EventArgs e)
        {
            var Dialog = new CustomTextDialog()
            {
                CustomTexts = CustomTextPreset
            };

            if (Dialog.ShowDialog() == DialogResult.OK)
            {
                CustomTextPreset = Dialog.CustomTexts;
                UserChanged();
            }
        }

        private string GetTickText(TimeSpan timeSpan)
        {
            return $"{timeSpan.Days}天{timeSpan.Hours}时{timeSpan.Minutes}分{timeSpan.Seconds}秒";
        }
    }
}
