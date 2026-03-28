using System;
using System.Windows.Forms;
using PlainCEETimer.Interop;
using PlainCEETimer.Modules;
using PlainCEETimer.UI.Extensions;

namespace PlainCEETimer.UI.Controls;

public sealed class PlainProgressBar : ProgressBar
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
    private int m_value;
    private ProgressStyle m_style;
    private TaskbarProgress tbp;

    protected override void OnHandleCreated(EventArgs e)
    {
        if (ThemeManager.ShouldUseDarkMode)
        {
            ThemeManager.EnableDarkModeForControl(Handle, SystemStyle.DarkTheme);
        }

        if (!init)
        {
            tbp = new(this.FindParentForm().Handle);
            init = true;
        }

        UpdateStyle();
        UpdateValue();
        base.OnHandleCreated(e);
    }

    protected override void OnHandleDestroyed(EventArgs e)
    {
        init = false;
        base.OnHandleDestroyed(e);
    }

    private void UpdateStyle()
    {
        if (init)
        {
            tbp.SetState(m_style);

            var pbs = m_style switch
            {
                ProgressStyle.Error => NativeConstants.PBST_ERROR,
                ProgressStyle.Paused => NativeConstants.PBST_PAUSED,
                _ => NativeConstants.PBST_NORMAL
            };

            Win32UI.SendMessage(Handle, NativeConstants.PBM_SETSTATE, pbs, 0);
        }
    }

    private void UpdateValue()
    {
        if (init)
        {
            tbp.SetValue(m_value, 100);
        }
    }
}
