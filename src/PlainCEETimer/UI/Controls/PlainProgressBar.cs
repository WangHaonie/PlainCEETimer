using System;
using System.Windows.Forms;
using PlainCEETimer.Interop;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Extensions;
using PlainCEETimer.UI.Extensions;

namespace PlainCEETimer.UI.Controls;

public sealed class PlainProgressBar : ProgressBar, IThemeAware
{
    public new int Value
    {
        get => base.Value;
        set
        {
            m_value = value;
            UpdateValue();
            base.Value = value;
        }
    }

    public new ProgressStyle Style
    {
        get => m_style;
        set
        {
            if (m_style != value)
            {
                m_style = value;
                UpdateStyle();
            }
        }
    }

    public ProgressBarStyle RealStyle
    {
        get => base.Style;
        set => base.Style = value;
    }

    private bool init;
    private bool UseDark;
    private int m_value;
    private ProgressStyle m_style;
    private TaskbarProgress tbp;
    private ThemeHelper themeHelper;

    protected override void OnHandleCreated(EventArgs e)
    {
        themeHelper ??= new(this);

        if (!init)
        {
            tbp = new(this.FindParentForm().Handle);
            init = true;
        }

        UpdateStyle();
        UpdateValue();
        base.OnHandleCreated(e);
    }

    protected override void Dispose(bool disposing)
    {
        themeHelper.Destroy();
        base.Dispose(disposing);
    }

    protected override void OnHandleDestroyed(EventArgs e)
    {
        init = false;
        base.OnHandleDestroyed(e);
    }

    protected override void WndProc(ref Message m)
    {
        if (m.Msg == WinUser.WM_PAINT && UseDark)
        {
            Win32UI.ComctlHookThemeBackground();
            base.WndProc(ref m);
            Win32UI.ComctlUnhookThemeBackground();
        }

        base.WndProc(ref m);
    }

    private void UpdateStyle()
    {
        if (init)
        {
            tbp.SetState(m_style);

            var pbs = m_style switch
            {
                ProgressStyle.Error => CommCtrl.PBST_ERROR,
                ProgressStyle.Paused => CommCtrl.PBST_PAUSED,
                _ => CommCtrl.PBST_NORMAL
            };

            Win32UI.SendMessage(Handle, CommCtrl.PBM_SETSTATE, pbs, 0);
        }
    }

    private void UpdateValue()
    {
        if (init)
        {
            tbp.SetValue(m_value, 100);
        }
    }

    void IThemeAware.UpdateTheme(bool useDark, bool init)
    {
        if (ThemeManager.NewThemeAvailable)
        {
            ThemeManager.EnableDarkModeForControl(Handle, useDark ? SystemStyle.DarkTheme : SystemStyle.Progress);
        }
        else
        {
            UseDark = useDark;
        }
    }
}
