using System;
using System.Windows.Forms;
using PlainCEETimer.Interop;

namespace PlainCEETimer.Controls
{
    public sealed class PlainNumericUpDown : NumericUpDown
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
                foreach (Control udbutton in Controls)
                {
                    ThemeManager.FlushDarkControl(udbutton, Modules.NativeStyle.Explorer);
                }
            }

            base.OnHandleCreated(e);
        }
    }
}
