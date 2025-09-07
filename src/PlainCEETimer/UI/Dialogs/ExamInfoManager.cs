using System;
using System.Windows.Forms;
using PlainCEETimer.Modules.Countdown;
using PlainCEETimer.Modules.Extensions;
using PlainCEETimer.UI.Controls;

namespace PlainCEETimer.UI.Dialogs;

public sealed class ExamInfoManager : ListViewDialog<Exam, ExamInfoDialog>
{
    private DateTime Now;

    public ExamInfoManager()
        : base(450, ["考试名称", "开始日期和时间", "结束日期和时间"], ["已过去的", "正在进行", "未开始的"])
    {
        Text = "管理考试信息 - 高考倒计时";
        ItemDescription = "考试信息";
    }

    protected override int GetGroupIndex(Exam data)
    {
        Now = DateTime.Now;

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

    protected override ListViewItem GetListViewItem(Exam data)
    {
        return new([data.Name, data.Start.Format(), data.End.Format()]);
    }

    protected override IListViewSubDialog<Exam> GetSubDialog(Exam data = null)
    {
        return new ExamInfoDialog(data);
    }
}
