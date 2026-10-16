using System;
using System.Windows.Forms;
using PlainCEETimer.Interop;

namespace PlainCEETimer.UI.Controls;

public sealed class PlainTrackBar : TrackBar, IThemeAwareEx
{
    public bool ShowToolTip
    {
        get;
        set
        {
            if (field != value)
            {
                field = value;
                RecreateHandle();
            }
        }
    } = true;

    protected override CreateParams CreateParams
    {
        get
        {
            var cp = base.CreateParams;

            if (ShowToolTip)
            {
                cp.Style |= CommCtrl.TBS_TOOLTIPS;
            }

            return cp;
        }
    }

    private bool UseDark;

    protected override void OnHandleCreated(EventArgs e)
    {
        ThemeHelper.Attach(this);
        base.OnHandleCreated(e);
    }

    protected override void WndProc(ref Message m)
    {
        switch (m.Msg)
        {
            case WinUser.WM_PAINT when UseDark:
                Win32UI.PnHookThemedPaint();
                Win32UI.PnHookThemeColor();
                base.WndProc(ref m);
                Win32UI.PnUnhookThemedPaint();
                Win32UI.PnUnhookThemeColor();
                return;
        }

        base.WndProc(ref m);
    }

    void IThemeAware.UpdateTheme(bool useDark, bool init)
    {
        UseDark = useDark;
        ToolTipStyles.ApplyStyle(Win32UI.SendMessage(Handle, CommCtrl.TBM_GETTOOLTIPS, 0, 0), useDark, init);
    }
}
