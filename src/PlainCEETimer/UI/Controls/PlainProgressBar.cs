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
            if (init)
            {
                tbp.SetValue(value, 100);
            }

            base.Value = value;
        }
    }

    public new ProgressStyle Style
    {
        get;
        set
        {
            if (init && field != value)
            {
                tbp.SetState(value);
            }

            const int PBM_SETSTATE = 0x0410;
            const int PBST_NORMAL = 1;
            const int PBST_ERROR = 2;
            const int PBST_PAUSED = 3;

            var pbs = value switch
            {
                ProgressStyle.Error => PBST_ERROR,
                ProgressStyle.Paused => PBST_PAUSED,
                _ => PBST_NORMAL
            };

            Win32UI.SendMessage(Handle, PBM_SETSTATE, pbs, 0);
            field = value;
        }
    }

    public ProgressBarStyle RealStyle
    {
        get => base.Style;
        set => base.Style = value;
    }

    private TaskbarProgress tbp;
    private bool init;

    protected override void OnHandleCreated(EventArgs e)
    {
        if (ThemeManager.ShouldUseDarkMode)
        {
            ThemeManager.EnableDarkModeForControl(Handle, NativeStyle.DarkTheme);
        }

        if (!init)
        {
            tbp = new(this.FindParentForm().Handle);
            init = true;
        }

        base.OnHandleCreated(e);
    }
}
