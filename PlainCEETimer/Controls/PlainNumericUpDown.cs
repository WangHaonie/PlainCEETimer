using System;
using System.Windows.Forms;
using PlainCEETimer.Interop;

namespace PlainCEETimer.Controls
{
    public class PlainNumericUpDown : NumericUpDown
    {
        public PlainNumericUpDown()
        {
            if (ThemeManager.ShouldUseDarkMode)
            {
                ForeColor = ThemeManager.DarkFore;
                BackColor = ThemeManager.DarkBack;
            }
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            if (ThemeManager.ShouldUseDarkMode)
            {
                for (int i = 0; i < Controls.Count; i++)
                {
                    ThemeManager.FlushDarkControl(Controls[i], Modules.NativeStyle.Explorer);
                }
            }

            base.OnHandleCreated(e);
        }
    }
}
