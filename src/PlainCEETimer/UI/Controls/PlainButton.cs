using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using PlainCEETimer.Interop;

namespace PlainCEETimer.UI.Controls;

public sealed class PlainButton : Button
{
    private sealed class ParentNativeWindow : NativeWindow
    {
        internal PlainButton m_owner;

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WinUser.WM_NOTIFY)
            {
                if (Marshal.ReadInt32(m.LParam, NMHDR.code) == CommCtrl.BCN_DROPDOWN
                    && Marshal.ReadIntPtr(m.LParam, NMHDR.hwndFrom) == m_owner.Handle)
                {
                    m_owner.ContextMenu.Show(m_owner, new(0, m_owner.Height));
                }
            }

            base.WndProc(ref m);
        }
    }

    protected override CreateParams CreateParams
    {
        get
        {
            var cp = base.CreateParams;

            if (ContextMenu != null)
            {
                cp.Style |= CommCtrl.BS_SPLITBUTTON;
            }

            return cp;
        }
    }

    private ParentNativeWindow pnw;
    private readonly PlainButtonBase bb;

    public PlainButton()
    {
        bb = new(this) { AutoDarkTheme = false };
        SetStyle(ControlStyles.UserPaint, false);
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        bb.Attach();

        if (ContextMenu != null)
        {
            NativeWindowHelper.Attach(Parent.Handle, ref pnw);
            pnw.m_owner = this;
        }

        base.OnHandleCreated(e);
    }

    protected override void Dispose(bool disposing)
    {
        bb.Detach();
        base.Dispose(disposing);
    }
}
