using PlainCEETimer.Interop;
using PlainCEETimer.Modules;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace PlainCEETimer.Controls
{
    public sealed class AppCheckBox : CheckBox
    {
        public AppCheckBox()
        {
            ThemeManager.SystemThemeChanged += ThemeManager_SystemThemeChanged;

            if (ThemeManager.AllowDarkMode && ThemeManager.IsDarkMode)
            {
                ThemeManager.FlushControl(this, DarkControlType.CheckBox);
            }
        }

        private void ThemeManager_SystemThemeChanged(object sender, EventArgs e)
        {
            ThemeManager.FlushControl(this, DarkControlType.CheckBox);
        }
    }
}
