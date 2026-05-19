using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using PlainCEETimer.Interop;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Extensions;

namespace PlainCEETimer.UI.Controls;

public sealed class PlainButton : Button, IThemeAware
{
    private sealed class ParentNativeWindow(PlainButton b) : NativeWindow
    {
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM.NOTIFY)
            {
                if (Marshal.ReadInt32(m.LParam, NMHDR.code) == NativeConstants.BCN_DROPDOWN
                    && Marshal.ReadIntPtr(m.LParam, NMHDR.hwndFrom) == b.Handle)
                {
                    b.ContextMenu.Show(b, new(0, b.Height));
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

    private ParentNativeWindow pnw;
    private ThemeHelper themeHelper;

    public PlainButton()
    {
        FlatStyle = FlatStyle.System;
        UseVisualStyleBackColor = true;
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        themeHelper ??= new(this);

        if (ContextMenu != null)
        {
            pnw?.ReleaseHandle();
            pnw ??= new ParentNativeWindow(this);
            pnw.AssignHandle(Parent.Handle);
        }

        base.OnHandleCreated(e);
    }

    protected override void Dispose(bool disposing)
    {
        themeHelper.Destroy();
        base.Dispose(disposing);
    }

    void IThemeAware.UpdateTheme(bool useDark, bool init)
    {
        ThemeManager.EnableDarkModeForControl(this, useDark ? SystemStyle.ExplorerDark : SystemStyle.Explorer);
    }
}
