using PlainCEETimer.Controls;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Configuration;
using System.Windows.Forms;

namespace PlainCEETimer.Dialogs
{
    public sealed class ExamInfoManager : ListViewDialogBase<ExamInfoObject, ExamInfoDialog>
    {
        protected override string DialogTitle => "管理考试信息 - 高考倒计时";
        protected override string ContentDescription => "考试信息";
        protected override string[] ListViewHeaders => ["考试名称", "开始日期和时间", "结束日期和时间"];
        protected override int ListViewWidth => 440;

        protected override void AddItem(ExamInfoObject Data, bool IsSelected = false)
        {
            AddItem(new ListViewItem([Data.Name, Data.Start.ToString(App.DateTimeFormat), Data.End.ToString(App.DateTimeFormat)])
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
