﻿using PlainCEETimer.Controls;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Configuration;
using System;
using System.Windows.Forms;

namespace PlainCEETimer.Dialogs
{
    public sealed class RulesManager : ListViewDialog<CustomRuleObject, RuleDialog>
    {
        public string[] CustomTextPreset { get; set; }
        public ColorSetObject[] ColorPresets { private get; set; }

        private readonly PlainButton ButtonGlobal;

        public RulesManager() : base(490, "管理自定义规则 - 高考倒计时", "规则", ["类别", "时刻", "效果预览"])
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

        protected override void AddItem(CustomRuleObject Data, bool IsSelected = false)
        {
            var tmp = Data.Colors;
            var Item = new ListViewItem([Validator.GetPhaseText(Data.Phase), Validator.GetTickText(Data.Tick)])
            {
                Tag = Data,
                Selected = IsSelected,
                Focused = IsSelected,
                UseItemStyleForSubItems = false
            };

            Item.SubItems.Add(new ListViewItem.ListViewSubItem(Item, Data.Text, tmp.Fore, tmp.Back, null));
            AddItem(Item, Data);
        }

        protected override ISubDialog<CustomRuleObject> GetSubDialogInstance(CustomRuleObject Existing = null)
        {
            return new RuleDialog()
            {
                Data = Existing,
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
    }
}
