using System;
using System.Windows.Forms;
using PlainCEETimer.Countdown;
using PlainCEETimer.Modules.Extensions;
using PlainCEETimer.UI.Controls;

namespace PlainCEETimer.UI.Dialogs;

public sealed class ExamManager : ListViewDialog<Exam, ExamDialog>
{
    public ExamManager()
        : base(450, ["考试名称", "开始日期和时间", "结束日期和时间"], ["已过去的", "正在进行", "未开始的"], "考试信息")
    {
        Text = "管理考试信息 - 高考倒计时";
    }

    protected override int GetGroupIndex(Exam data)
    {
        var now = DateTime.Now;

        if (now > data.End)
        {
            return 0;
        }

        if (now < data.Start)
        {
            return 2;
        }

        return 1;
    }

    protected override ListViewItem GetListViewItem(Exam data)
    {
        return new([data.Name, data.Start.Format(), data.End.Format()]);
    }

    protected override IListViewChildDialog<Exam> GetChildDialog(Exam data = null)
    {
        return new ExamDialog(data);
    }
}
