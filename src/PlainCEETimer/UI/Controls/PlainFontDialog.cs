using System;
using System.Drawing;
using PlainCEETimer.Interop;
using PlainCEETimer.Modules.Configuration;

namespace PlainCEETimer.UI.Controls;

public sealed class PlainFontDialog(AppForm owner, Font font) : PlainCommonDialog(owner, "选择字体 - 高考倒计时")
{
    public Font Font => font;

    protected override bool StartDialog(IntPtr hWndOwner)
    {
        var lf = LOGFONT.FromFont(font);
        var result = Win32UI.RunFontDialog(hWndOwner, HookProc, ref lf, int.MakeLong(ConfigValidator.MaxFontSize, ConfigValidator.MinFontSize));

        if (result)
        {
            font = lf.ToFont();
        }

        return result;
    }
}
