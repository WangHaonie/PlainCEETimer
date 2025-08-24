using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using PlainCEETimer.Interop;
using PlainCEETimer.Modules;

namespace PlainCEETimer.UI.Controls;

public sealed class PlainColorDialog : ColorDialog
{
    private CommonDialogHelper Helper;

    public PlainColorDialog()
    {
        FullOpen = true;
        CustomColors = App.AppConfig.CustomColors;
    }

    public DialogResult ShowDialog(Color Default, AppForm owner)
    {
        Color = Default;
        Helper = new(this, owner, "选取颜色 - 高考倒计时", base.HookProc);
        var previous = CustomColors;
        var result = ShowDialog();

        if (result == DialogResult.OK)
        {
            var tmp = CustomColors;

            if (!tmp.SequenceEqual(previous))
            {
                App.AppConfig.CustomColors = tmp;
            }
        }

        return result;
    }

    protected override IntPtr HookProc(IntPtr hWnd, int msg, IntPtr wparam, IntPtr lparam)
    {
        return Helper.HookProc(hWnd, msg, wparam, lparam);
    }
}
