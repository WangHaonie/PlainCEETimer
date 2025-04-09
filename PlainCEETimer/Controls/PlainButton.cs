using PlainCEETimer.Interop;
using PlainCEETimer.Modules;
using System.Windows.Forms;

namespace PlainCEETimer.Controls
{
    public class PlainButton : Button
    {
        public PlainButton()
        {
            if (ThemeManager.ShouldUseDarkMode)
            {
                FlatStyle = FlatStyle.System;
                ThemeManager.FlushDarkControl(this, DarkControlType.Explorer);
            }
        }
    }
}
