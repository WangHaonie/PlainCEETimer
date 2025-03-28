﻿using PlainCEETimer.Controls;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Configuration;
using System;
using System.Windows.Forms;

namespace PlainCEETimer.Dialogs
{
    public sealed class RulesManager : ListViewDialogBase<CustomRuleObject, RuleDialog>
    {
        public ColorSetObject[] ColorPresets { private get; set; }
        public string[] CustomTextPreset { get; set; }

        protected override string DialogTitle => "管理自定义规则 - 高考倒计时";
        protected override string ContentDescription => "规则";
        protected override string[] ListViewHeaders => ["类别", "时刻", "效果预览"];
        protected override int ListViewWidth => 490;

        private readonly Button ButtonGlobal;

        public RulesManager()
        {
            ButtonGlobal = new Button()
            {
                Size = new SmartSize(90, 23),
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
            var Item = new ListViewItem([Validator.GetPhaseText(Info.Phase), Validator.GetTickText(Info.Tick)])
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
