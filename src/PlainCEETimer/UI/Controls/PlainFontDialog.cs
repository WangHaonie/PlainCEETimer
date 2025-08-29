using System.Drawing;
using PlainCEETimer.Interop;
using PlainCEETimer.Interop.Extensions;
using PlainCEETimer.Modules.Configuration;

namespace PlainCEETimer.UI.Controls;

public sealed class PlainFontDialog : PlainCommonDialog
{
    public Font Font => font;

    private Font font;

    public PlainFontDialog(AppForm owner, Font existing)
    {
        Parent = owner;
        font = existing;
        Text = "选择字体 - 高考倒计时";
    }

    protected override BOOL RunDialog(HWND hWndOwner)
    {
        var lf = LOGFONT.FromFont(font);
        var result = CommonDialogs.RunFontDialog(hWndOwner, HookProc, ref lf, int.MakeLong(Validator.MinFontSize, Validator.MaxFontSize));

        if (result)
        {
            font = lf.ToFont();
        }

        return result;
    }
}
