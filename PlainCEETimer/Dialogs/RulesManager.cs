using PlainCEETimer.Controls;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Configuration;
using PlainCEETimer.Modules.Extensions;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace PlainCEETimer.Dialogs
{
    public sealed class RulesManager : ListViewDialogBase<CustomRuleObject, RuleDialog>
    {
        public ColorSetObject[] ColorPresets { private get; set; }
        public string[] CustomTextPreset { get; set; }

        private Button ButtonGlobal;

        protected override void InitializeDialog()
        {
            DialogTitle = "管理自定义规则 - 高考倒计时";
            ContentDescription = "规则";
            ListViewHeaders = ["类别", "时刻", "效果预览"];
            ListViewWidth = 490;

            ButtonGlobal = new Button()
            {
                Size = new Size(90, 23).ScaleToDpi(),
                Text = "全局设置(G)",
                UseVisualStyleBackColor = true
            };
            ButtonGlobal.Click += ButtonGlobal_Click;

            Controls.Add(ButtonGlobal);
        }

        protected override void OnLoad()
        {
            base.OnLoad();
            AddNewButton(ButtonGlobal, 6);
        }

        protected override void AddItem(CustomRuleObject Info, bool IsSelected = false)
        {
            var Item = new ListViewItem([CustomRuleHelper.GetRuleTypeText(Info.Phase), CustomRuleHelper.GetExamTickText(Info.Tick)])
            {
                Tag = Info,
                Selected = IsSelected,
                Focused = IsSelected,
                UseItemStyleForSubItems = false
            };

            Item.SubItems.Add(new ListViewItem.ListViewSubItem(Item, Info.Text, Info.Fore, Info.Back, null));

            AddItem(Item, Info);
        }

        protected override ISubDialog<CustomRuleObject, RuleDialog> GetSubDialogInstance(CustomRuleObject Existing = null) => new RuleDialog()
        {
            Data = Existing,
            GlobalColors = ColorPresets,
            GlobalTexts = CustomTextPreset
        };

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
    }
}
