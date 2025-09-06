using System;
using System.Windows.Forms;
using PlainCEETimer.Modules.Configuration;
using PlainCEETimer.Modules.Extensions;
using PlainCEETimer.UI.Controls;

namespace PlainCEETimer.UI.Dialogs;

public sealed class ExamInfoManager : ListViewDialog<ExamInfoObject, ExamInfoDialog>
{
    public ExamInfoManager()
        : base(450, ["考试名称", "开始日期和时间", "结束日期和时间"], ["已过去的", "正在进行", "未开始的"])
    {
        Text = "管理考试信息 - 高考倒计时";
        ItemDescription = "考试信息";
    }

    protected override int GetGroupIndex(ExamInfoObject data)
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

    protected override ListViewItem GetListViewItem(ExamInfoObject data)
    {
        return new([data.Name, data.Start.Format(), data.End.Format()]);
    }

    protected override IListViewSubDialog<ExamInfoObject> GetSubDialog(ExamInfoObject data = null)
    {
        return new ExamInfoDialog(data);
    }
}
