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
    }
}
