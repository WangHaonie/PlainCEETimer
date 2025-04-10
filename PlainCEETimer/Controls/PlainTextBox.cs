using PlainCEETimer.Interop;
using PlainCEETimer.Modules;
using System.Windows.Forms;

namespace PlainCEETimer.Controls
{
    public class PlainTextBox : TextBox
    {
        public PlainTextBox()
        {
            if (ThemeManager.ShouldUseDarkMode)
            {
                ForeColor = ThemeManager.DarkFore;
                BackColor = ThemeManager.DarkBack;
                ThemeManager.FlushDarkControl(this, NativeStyle.CFD);
            }

            MaxLength = Validator.MaxCustomTextLength;
        }
    }
}
