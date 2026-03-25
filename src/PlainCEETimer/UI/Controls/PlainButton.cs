using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using PlainCEETimer.Interop;
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
            if (m.Msg == WM.NOTIFY)
            {
                if (Marshal.ReadInt32(m.LParam, NMHDR.code) == NativeConstants.BCN_DROPDOWN
                    && Marshal.ReadIntPtr(m.LParam, NMHDR.hwndFrom) == instance.Handle)
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
                cp.Style |= NativeConstants.BS_SPLITBUTTON;
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
            ThemeManager.EnableDarkModeForControl(this, SystemStyle.ExplorerDark);
        }

        if (ContextMenu != null)
        {
            _ = new ParentNativeWindow(this);
        }

        base.OnHandleCreated(e);
    }
}
