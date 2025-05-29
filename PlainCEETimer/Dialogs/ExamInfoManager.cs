using System;
using System.Windows.Forms;
using PlainCEETimer.Controls;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Configuration;

namespace PlainCEETimer.Dialogs
{
    public sealed class ExamInfoManager : ListViewDialog<ExamInfoObject, ExamInfoDialog>
    {
        public ExamInfoManager()
            : base(420, ["考试名称", "开始日期和时间", "结束日期和时间"], ["已过去的", "正在进行", "未开始的"])
        {
            Text = "管理考试信息 - 高考倒计时";
            ItemDescription = "考试信息";
        }

        protected override ListViewItem GetListViewItem(ExamInfoObject data)
        {
            return new([data.Name, data.Start.Format(), data.End.Format()]);
        }

        protected override int GetGroupIndex(ExamInfoObject data)
        {
            var Now = DateTime.Now;

            if (Now > data.End)
            {
                return 0;
            }

            if (Now < data.Start)
            {
                return 2;
            }

            return 1;
        }

        protected override IListViewSubDialog<ExamInfoObject> GetSubDialog(ExamInfoObject data = null)
        {
            return new ExamInfoDialog(data);
        }
    }
}
