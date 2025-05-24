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
            : base(420, "管理考试信息 - 高考倒计时", "考试信息", ["考试名称", "开始日期和时间", "结束日期和时间"], ["已过去的", "正在进行", "未开始的"])
        { }

        protected override ListViewItem GetListViewItem(ExamInfoObject data)
        {
            return new([data.Name, data.Start.Format(), data.End.Format()]);
        }

        protected override int GetGroupIndex(ExamInfoObject data)
        {
            int i = 1;
            var Now = DateTime.Now;

            if (Now > data.End)
            {
                i = 0;
            }

            if (Now < data.Start)
            {
                i = 2;
            }

            return i;
        }

        protected override IListViewSubDialog<ExamInfoObject> GetSubDialog(ExamInfoObject data = null)
        {
            return new ExamInfoDialog(data);
        }
    }
}
