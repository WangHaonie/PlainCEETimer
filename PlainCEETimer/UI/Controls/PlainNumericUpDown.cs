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
                ForeColor = Colors.DarkForeText;
                BackColor = Colors.DarkBackText;
            }

            TextAlign = HorizontalAlignment.Right;
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            if (ThemeManager.ShouldUseDarkMode)
            {
                for (int i = 0; i < 2; i++)
                {
                    ThemeManager.FlushControl(Controls[i], NativeStyle.ExplorerDark);
                }
            }

            base.OnHandleCreated(e);
        }
    }
}
