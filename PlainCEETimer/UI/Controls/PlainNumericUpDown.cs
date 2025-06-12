using System;
using System.Windows.Forms;
using PlainCEETimer.Interop;
using PlainCEETimer.Modules;

namespace PlainCEETimer.UI.Controls
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

            TextAlign = HorizontalAlignment.Right;
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            if (ThemeManager.ShouldUseDarkMode)
            {
                foreach (Control udbutton in Controls)
                {
                    ThemeManager.FlushControl(udbutton, NativeStyle.Explorer);
                }
            }

            base.OnHandleCreated(e);
        }
    }
}
