using PlainCEETimer.Interop;
using System.Windows.Forms;

namespace PlainCEETimer.Controls
{
    public class PlainRadioButton : RadioButton
    {
        public PlainRadioButton()
        {
            if (ThemeManager.ShouldUseDarkMode)
            {
                ThemeManager.FlushDarkControl(this, Modules.DarkControlType.Explorer);
            }
        }
    }
}
