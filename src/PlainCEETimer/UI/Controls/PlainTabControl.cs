using System;
using System.Drawing;
using System.Windows.Forms;
using PlainCEETimer.Interop;
using PlainCEETimer.Interop.Extensions;
using PlainCEETimer.Modules;
using PlainCEETimer.UI.Extensions;

namespace PlainCEETimer.UI.Controls;

public class PlainTabControl : TabControl, IThemeAwareEx
{
    private bool UseDark;
    private static readonly bool NewTheme = ThemeManager.NewThemeAvailable;

    public PlainTabControl()
    {
        SetStyle(ControlStyles.UserPaint, false);
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        ThemeHelper.Attach(this);
        this.HideFocusIndicator();
        base.OnHandleCreated(e);
    }

    protected override void WndProc(ref Message m)
    {
        if (UseDark)
        {
            switch (m.Msg)
            {
                case WinUser.WM_PAINT when !NewTheme:
                    Win32UI.PnHookThemedPaint();
                    base.WndProc(ref m);
                    Win32UI.PnUnhookThemedPaint();
                    return;
                case WinUser.WM_PARENTNOTIFY when m.WParam.LoWord == WinUser.WM_CREATE:
                    UpdateUpDownTheme(m.LParam, true);
                    break;
            }
        }

        base.WndProc(ref m);
    }

    private static void UpdateUpDownTheme(IntPtr hud, bool useDark)
    {
        ThemeManager.ApplyControlTheme(hud, useDark ? SystemStyle.ExplorerDark : SystemStyle.Explorer);
    }

    void IThemeAware.UpdateTheme(bool useDark, bool init)
    {
        UseDark = useDark;
        var hwnd = Handle;

        if (useDark)
        {
            if (NewTheme)
            {
                ThemeManager.ApplyControlTheme(hwnd, SystemStyle.DarkTheme);
            }
        }
        else
        {
            ThemeManager.ApplyControlTheme(hwnd, SystemStyle.Explorer);
        }

        TabPage tp;
        var tabs = TabPages;
        var length = tabs.Count;
        var fore = useDark ? Colors.DarkForeText : SystemColors.WindowText;
        var back = useDark ? Colors.DarkBackText : SystemColors.Window;

        for (int i = 0; i < length; i++)
        {
            tp = tabs[i];
            tp.ForeColor = fore;
            tp.BackColor = back;
        }

        if (Win32UI.PnTryGetTabUpDown(hwnd, out var hud))
        {
            UpdateUpDownTheme(hud, useDark);
        }
    }
}