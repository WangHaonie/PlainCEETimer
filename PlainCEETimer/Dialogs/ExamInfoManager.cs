using PlainCEETimer.Controls;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Configuration;
using System;
using System.Windows.Forms;

namespace PlainCEETimer.Dialogs
{
    public sealed class ExamInfoManager : ListViewDialog<ExamInfoObject>
    {
        public ExamInfoManager()
        {
            InitializeComponent();
        }

        protected override void AddItem(ExamInfoObject Info, bool IsSelected = false)
        {
            AddItem(new ListViewItem([Info.Name, Info.Start.ToString(App.DateTimeFormat), Info.End.ToString(App.DateTimeFormat)])
            {
                Tag = Info,
                Selected = IsSelected,
                Focused = IsSelected
            }, Info);
        }

        protected override void ContextAdd_Click(object sender, EventArgs e)
        {
            ExamInfoDialog Dialog = new();

            if (Dialog.ShowDialog() == DialogResult.OK)
            {
                AddItemSafe(Dialog.ExamInfo);
            }
        }

        protected override void ContextEdit_Click(object sender, EventArgs e)
        {
            var TargetItem = SelectedItem;
            ExamInfoDialog Dialog = new((ExamInfoObject)TargetItem.Tag);

            if (Dialog.ShowDialog() == DialogResult.OK)
            {
                AddItemSafe(Dialog.ExamInfo, TargetItem);
            }
        }

        protected override void OpenRetryDialog(ExamInfoObject Data)
        {
            ExamInfoDialog Dialog = new(Data);

            if (Dialog.ShowDialog() == DialogResult.OK)
            {
                AddItemSafe(Dialog.ExamInfo);
            }
        }

        private void InitializeComponent()
        {
            DialogTitle = "管理考试信息 - 高考倒计时";
            ContentDescription = "考试信息";
            ListViewHeaders = ["考试名称", "开始日期和时间", "结束日期和时间"];
            ListViewWidth = 440;
        }
    }
}
