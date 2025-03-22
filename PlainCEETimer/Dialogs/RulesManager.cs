using PlainCEETimer.Controls;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Configuration;
using System;
using System.Windows.Forms;

namespace PlainCEETimer.Dialogs
{
    public sealed class RulesManager : ListViewDialog<CustomRuleObject>
    {
        private Button ButtonGlobal;
        public RulesManager()
        {
            InitializeComponent();
        }

        protected override void OnLoad()
        {
            base.OnLoad();
            AlignExtraButtonL(ButtonGlobal, 6);
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

        protected override void ContextAdd_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        protected override void ContextEdit_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        protected override void OpenRetryDialog(CustomRuleObject Data)
        {
            throw new NotImplementedException();
        }

        private void InitializeComponent()
        {
            DialogTitle = "管理自定义规则 - 高考倒计时";
            ContentDescription = "规则";
            ListViewHeaders = ["类别", "时刻", "效果预览"];
            ListViewWidth = 490;

            ButtonGlobal = new Button()
            {
                Size = new(90, 23),
                Text = "全局设置(G)",
                UseVisualStyleBackColor = true
            };

            Controls.Add(ButtonGlobal);
        }
    }
}
