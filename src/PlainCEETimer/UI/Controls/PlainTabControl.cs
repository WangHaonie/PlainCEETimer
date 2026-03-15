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

        Win32UI.SendMessage(Handle, WM.CHANGEUISTATE, int.MakeLong(NativeConstants.UIS_SET, NativeConstants.UISF_HIDEFOCUS), 0);

        base.OnHandleCreated(e);
    }

    protected override void WndProc(ref Message m)
    {
        if (UseDark && m.Msg == WM.PARENTNOTIFY
            && m.WParam.ToInt32().LoWord == WM.CREATE)
        {
            ThemeManager.EnableDarkModeForControl(m.LParam, NativeStyle.ExplorerDark);
        }

        base.WndProc(ref m);
    }
}