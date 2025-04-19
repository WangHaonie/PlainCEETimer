using System.Windows.Forms;
using PlainCEETimer.Interop;

namespace PlainCEETimer.Controls
{
    public class PlainRadioButton : RadioButton
    {
        public PlainRadioButton()
        {
            if (ThemeManager.ShouldUseDarkMode)
            {
                ThemeManager.FlushDarkControl(this, Modules.NativeStyle.Explorer);
            }
        }
    }
}
