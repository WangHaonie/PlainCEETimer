using System;
using System.Windows.Forms;
using PlainCEETimer.Interop;
using PlainCEETimer.Modules;

namespace PlainCEETimer.UI.Controls;

public class PlainDropDown : AppForm
{
    protected override AppWindowStyle Params => AppWindowStyle.OnEscClosing | AppWindowStyle.NoBorder;

    protected override CreateParams CreateParams
    {
        get
        {
            var cp = base.CreateParams;
            var s = cp.Style;
            s |= WinUser.WS_POPUP;

            if (UseStyles)
                s |= WinUser.WS_BORDER;
            else
                s &= ~WinUser.WS_BORDER;

            cp.Style = s;
            cp.ExStyle |= WinUser.WS_EX_TOOLWINDOW;
            return cp;
        }
    }

    protected virtual bool UseStyles => true;

    private static readonly bool isW11 = SystemVersion.IsWindows11;

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);

        if (isW11 && UseStyles)
        {
            Win32UI.PnSetRoundCornerEx(Handle, true);
        }
    }
}
