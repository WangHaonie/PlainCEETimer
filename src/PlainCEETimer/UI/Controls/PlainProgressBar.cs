using System;
using System.Windows.Forms;
using PlainCEETimer.Interop;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Extensions;
using PlainCEETimer.UI.Extensions;

namespace PlainCEETimer.UI.Controls;

public sealed class PlainProgressBar : ProgressBar, IThemeAwareEx
{
    public new int Value
    {
        get => base.Value;
        set
        {
            base.Value = value;
            UpdateValue();
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
                base.Style = value == ProgressStyle.Indeterminate ? ProgressBarStyle.Marquee : ProgressBarStyle.Blocks;
                UpdateStyle();
            }
        }
    }

    private bool UseDark;
    private ProgressStyle m_style;
    private TaskbarProgress tbp;

    public PlainProgressBar()
    {
        MarqueeAnimationSpeed = 50;
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        ThemeHelper.Attach(this);
        tbp = new(this.FindParentForm().Handle);
        UpdateStyle();
        UpdateValue();
        base.OnHandleCreated(e);
    }

    protected override void OnHandleDestroyed(EventArgs e)
    {
        tbp.Destroy();
        tbp = null;
        base.OnHandleDestroyed(e);
    }

    protected override void WndProc(ref Message m)
    {
        if (m.Msg == WinUser.WM_PAINT && UseDark)
        {
            Win32UI.PnHookThemedPaint();
            base.WndProc(ref m);
            Win32UI.PnUnhookThemedPaint();
            return;
        }

        base.WndProc(ref m);
    }

    private void UpdateStyle()
    {
        if (IsHandleCreated && tbp != null)
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
        if (IsHandleCreated && tbp != null)
        {
            tbp.SetValue(base.Value - Minimum, Maximum - Minimum);
        }
    }

    void IThemeAware.UpdateTheme(bool useDark, bool init)
    {
        if (ThemeManager.NewThemeAvailable)
        {
            ThemeManager.ApplyControlTheme(Handle, useDark ? SystemStyle.DarkTheme : SystemStyle.Progress);
        }
        else
        {
            UseDark = useDark;
        }
    }
}
