using System;
using System.Windows.Forms;
using PlainCEETimer.Interop;
using PlainCEETimer.Modules;

namespace PlainCEETimer.UI.Controls;

public class PlainTabControl : TabControl
{
    private readonly bool UseDark = ThemeManager.ShouldUseDarkMode;

    protected override void OnHandleCreated(EventArgs e)
    {
        if (UseDark)
        {
            var tabs = TabPages;
            var length = tabs.Count;
            TabPage current;

            for (int i = 0; i < length; i++)
            {
                current = tabs[i];
                current.ForeColor = Colors.DarkForeText;
                current.BackColor = Colors.DarkBackText;
            }

            if (ThemeManager.NewThemeAvailable)
            {
                ThemeManager.EnableDarkModeForControl(this, NativeStyle.DarkTheme);
            }
            else
            {
                Win32UI.SetWindowTheme(Handle, "DarkMode", "ExplorerNavPane");
            }
        }

        const int WM_CHANGEUISTATE = 0x0127;
        const int UIS_SET = 1;
        const int UISF_HIDEFOCUS = 0x1;
        Win32UI.SendMessage(Handle, WM_CHANGEUISTATE, int.MakeLong(UIS_SET, UISF_HIDEFOCUS), 0);

        base.OnHandleCreated(e);
    }

    protected override void WndProc(ref Message m)
    {
        const int WM_PARENTNOTIFY = 0x0210;
        const int WM_CREATE = 0x0001;

        if (UseDark && m.Msg == WM_PARENTNOTIFY
            && m.WParam.ToInt32().LoWord == WM_CREATE)
        {
            ThemeManager.EnableDarkModeForControl(m.LParam, NativeStyle.ExplorerDark);
        }

        base.WndProc(ref m);
    }
}