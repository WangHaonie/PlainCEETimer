using System.Windows.Forms;
using PlainCEETimer.Interop;
using PlainCEETimer.Modules;

namespace PlainCEETimer.Controls
{
    public class PlainButton : Button
    {
        public PlainButton()
        {
            if (ThemeManager.ShouldUseDarkMode)
            {
                FlatStyle = FlatStyle.System;
                ThemeManager.FlushDarkControl(this, NativeStyle.Explorer);
            }
        }
    }
}
