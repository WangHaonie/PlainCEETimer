using PlainCEETimer.Interop;
using System.Windows.Forms;

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
