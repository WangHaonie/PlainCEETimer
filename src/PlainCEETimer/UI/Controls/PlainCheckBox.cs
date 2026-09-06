using System;
using System.Windows.Forms;
using PlainCEETimer.Interop;

namespace PlainCEETimer.UI.Controls;

public sealed class PlainCheckBox : CheckBox
{
    private readonly PlainButtonBase bb;

    public PlainCheckBox()
    {
        bb = new(this);
        SetStyle(ControlStyles.UserPaint, false);
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        bb.Attach();
        base.OnHandleCreated(e);
    }

    protected override void Dispose(bool disposing)
    {
        bb.Detach();
        base.Dispose(disposing);
    }

    protected override void WndProc(ref Message m)
    {
        if (bb.ShouldHookPaint && m.Msg == WinUser.WM_PAINT)
        {
            Win32UI.PnHookThemedPaint();
            base.WndProc(ref m);
            Win32UI.PnUnhookThemedPaint();
            return;
        }

        base.WndProc(ref m);
    }
}
