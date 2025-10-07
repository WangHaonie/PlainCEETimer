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

    public TaskbarProgressState TaskbarProgressState
    {
        get;
        set
        {
            if (init)
            {
                tbp.SetState(value);
            }

            field = value;
        }
    }

    private TaskbarProgress tbp;
    private bool init;

    protected override void OnHandleCreated(EventArgs e)
    {
        if (ThemeManager.ShouldUseDarkMode)
        {
            ThemeManager.FlushControl(Handle, NativeStyle.DarkTheme);
        }

        tbp = new(this.FindParentForm().Handle);
        init = true;
        base.OnHandleCreated(e);
    }
}
