using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using PlainCEETimer.Modules;

namespace PlainCEETimer.UI.Controls;

public sealed class PlainButton : Button
{
    private sealed class ParentNativeWindow : NativeWindow
    {
        private readonly PlainButton instance;

        public ParentNativeWindow(PlainButton pb)
        {
            instance = pb;
            AssignHandle(pb.Parent.Handle);
        }

        protected override void WndProc(ref Message m)
        {
            const int WM_NOTIFY = 0x004E;

            if (m.Msg == WM_NOTIFY)
            {
                const int NMHDR_hwndFrom = 0;
                const int NMHDR_code = 16;
                const int BCN_FIRST = unchecked((int)(0U - 1250U));
                const int BCN_DROPDOWN = BCN_FIRST + 0x0002;

                if (Marshal.ReadInt32(m.LParam, NMHDR_code) == BCN_DROPDOWN
                    && Marshal.ReadIntPtr(m.LParam, NMHDR_hwndFrom) == instance.Handle)
                {
                    instance.ContextMenu.Show(instance, new(0, instance.Height));
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
                const int BS_SPLITBUTTON = 0x0000000C;
                cp.Style |= BS_SPLITBUTTON;
            }

            return cp;
        }
    }

    public PlainButton()
    {
        FlatStyle = FlatStyle.System;
        UseVisualStyleBackColor = true;
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        if (ThemeManager.ShouldUseDarkMode)
        {
            ThemeManager.EnableDarkModeForControl(this, NativeStyle.ExplorerDark);
        }

        if (ContextMenu != null)
        {
            new ParentNativeWindow(this);
        }

        base.OnHandleCreated(e);
    }
}
