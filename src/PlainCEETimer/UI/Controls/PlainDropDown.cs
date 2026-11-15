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

            if (!isW11)
            {
                cp.Style |= WinUser.WS_BORDER | WinUser.WS_POPUP;
            }

            return cp;
        }
    }

    private static readonly bool isW11 = SystemVersion.IsWindows11;

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);

        if (isW11)
        {
            Win32UI.PnSetRoundCornerEx(Handle, true);
        }
    }
}
