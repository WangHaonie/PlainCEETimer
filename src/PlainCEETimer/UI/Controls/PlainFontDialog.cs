using System.Drawing;
using System.Runtime.InteropServices;
using PlainCEETimer.Interop;
using PlainCEETimer.Interop.Extensions;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Configuration;

namespace PlainCEETimer.UI.Controls;

public sealed class PlainFontDialog(AppForm owner, Font font) : PlainCommonDialog(owner, "选择字体 - 高考倒计时")
{
    public Font Font => font;

    protected override BOOL RunDialog(HWND hWndOwner)
    {
        var lf = LOGFONT.FromFont(font);
        var result = RunFontDialog(hWndOwner, HookProc, ref lf, int.MakeLong(Validator.MinFontSize, Validator.MaxFontSize));

        if (result)
        {
            font = lf.ToFont();
        }

        return result;
    }

    [DllImport(App.NativesDll, EntryPoint = "#25")]
    private static extern BOOL RunFontDialog(HWND hWndOwner, WNDPROC lpfnHookProc, ref LOGFONT lpLogFont, int nSizeLimit);
}
