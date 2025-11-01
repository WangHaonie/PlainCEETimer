using System;
using System.Windows.Forms;
using PlainCEETimer.Modules;

namespace PlainCEETimer.UI.Controls;

public sealed class PlainNumericUpDown : NumericUpDown
{
    public PlainNumericUpDown()
    {
        if (ThemeManager.ShouldUseDarkMode)
        {
            ForeColor = Colors.DarkForeText;
            BackColor = Colors.DarkBackText;
        }

        TextAlign = HorizontalAlignment.Right;
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        if (ThemeManager.ShouldUseDarkMode)
        {
            var ctrls = Controls;
            var count = ctrls.Count;

            for (int i = 0; i < count; i++)
            {
                ThemeManager.EnableDarkModeForControl(ctrls[i], NativeStyle.ExplorerDark);
            }
        }

        base.OnHandleCreated(e);
    }
}
