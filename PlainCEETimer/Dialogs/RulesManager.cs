using System;
using System.Windows.Forms;
using PlainCEETimer.Controls;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Configuration;

namespace PlainCEETimer.Dialogs
{
    public sealed class RulesManager : ListViewDialog<CustomRuleObject, RuleDialog>
    {
        public string[] CustomTextPreset { get; set; }
        public ColorSetObject[] ColorPresets { private get; set; }

        private readonly PlainButton ButtonGlobal;

        public RulesManager() : base(500, "管理自定义规则 - 高考倒计时", "规则", ["类别", "时刻", "效果预览"])
        {
            ButtonGlobal = new PlainButton()
            {
                Size = new(ScaleToDpi(90), ScaleToDpi(23)),
                Text = "全局设置(&G)",
                UseVisualStyleBackColor = true
            };

            ButtonGlobal.Click += ButtonGlobal_Click;
            Controls.Add(ButtonGlobal);
        }

        protected override void OnShown()
        {
            AddNewButton(ButtonGlobal, 6);
        }

        protected override ListViewItem GetListViewItem(CustomRuleObject data)
        {
            var tmp = data.Colors;
            var item = new ListViewItem([GetPhaseText(data.Phase), GetTickText(data.Tick)])
            {
                UseItemStyleForSubItems = false
            };

            item.SubItems.Add(data.Text, tmp.Fore, tmp.Back, null);
            return item;
        }

        protected override ISubDialog<CustomRuleObject> GetSubDialog(CustomRuleObject data = null)
        {
            return new RuleDialog()
            {
                Data = data,
                GlobalColors = ColorPresets,
                GlobalTexts = CustomTextPreset
            };
        }

        private void ButtonGlobal_Click(object sender, EventArgs e)
        {
            CustomTextDialog Dialog = new()
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
            => $"{timeSpan.Days}天{timeSpan.Hours}时{timeSpan.Minutes}分{timeSpan.Seconds}秒";

        private string GetPhaseText(CountdownPhase i) => i switch
        {
            CountdownPhase.P2 => Constants.PH_RTP2,
            CountdownPhase.P3 => Constants.PH_RTP3,
            _ => Constants.PH_RTP1
        };
    }
}
