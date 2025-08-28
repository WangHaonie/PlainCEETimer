using System.Drawing;
using PlainCEETimer.Interop;
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
        var result = CommonDialogs.RunFontDialog(hWndOwner, DialogHook, ref lf, Validator.MinFontSize, Validator.MaxFontSize);

        if (result)
        {
            var tmp = lf.ToFont();
            font = new(tmp.FontFamily, tmp.SizeInPoints, tmp.Style, GraphicsUnit.Point, tmp.GdiCharSet, tmp.GdiVerticalFont);
        }

        return result;
    }
}
