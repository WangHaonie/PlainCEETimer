using System;
using System.Windows.Forms;
using PlainCEETimer.Interop;
using PlainCEETimer.Interop.Extensions;
using PlainCEETimer.Modules;
using PlainCEETimer.UI.Extensions;

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
                ThemeManager.ApplyControlTheme(this, SystemStyle.DarkTheme);
            }
            else
            {
                Win32UI.SetWindowTheme(Handle, "DarkMode", "ExplorerNavPane");
            }
        }

        this.HideFocusIndicator();
        base.OnHandleCreated(e);
    }

    protected override void WndProc(ref Message m)
    {
        if (UseDark && m.Msg == WinUser.WM_PARENTNOTIFY
            && m.WParam.LoWord == WinUser.WM_CREATE)
        {
            ThemeManager.ApplyControlTheme(m.LParam, SystemStyle.ExplorerDark);
        }

        base.WndProc(ref m);
    }
}