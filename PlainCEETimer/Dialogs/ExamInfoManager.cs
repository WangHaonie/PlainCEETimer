﻿using PlainCEETimer.Controls;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Configuration;
using System.Windows.Forms;

namespace PlainCEETimer.Dialogs
{
    public sealed class ExamInfoManager : ListViewDialog<ExamInfoObject, ExamInfoDialog>
    {
        public ExamInfoManager() : base(440, "管理考试信息 - 高考倒计时", "考试信息", ["考试名称", "开始日期和时间", "结束日期和时间"]) { }

        protected override void AddItem(ExamInfoObject Data, bool IsSelected = false)
        {
            AddItem(new ListViewItem([Data.Name, Data.Start.ToFormatted(), Data.End.ToFormatted()])
            {
                Tag = Data,
                Selected = IsSelected,
                Focused = IsSelected
            }, Data);
        }

        protected override ISubDialog<ExamInfoObject> GetSubDialogInstance(ExamInfoObject Existing = null)
        {
            return new ExamInfoDialog(Existing);
        }
    }
}
