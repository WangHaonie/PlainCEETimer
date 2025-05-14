using System.Windows.Forms;
using PlainCEETimer.Controls;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Configuration;

namespace PlainCEETimer.Dialogs
{
    public sealed class ExamInfoManager : ListViewDialog<ExamInfoObject, ExamInfoDialog>
    {
        public ExamInfoManager()
            : base(440, "管理考试信息 - 高考倒计时", "考试信息", ["考试名称", "开始日期和时间", "结束日期和时间"]) { }

        protected override ListViewItem GetListViewItem(ExamInfoObject data)
            => new([data.Name, data.Start.Format(), data.End.Format()]);

        protected override ISubDialog<ExamInfoObject> GetSubDialog(ExamInfoObject data = null)
            => new ExamInfoDialog(data);
    }
}
