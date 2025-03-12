using PlainCEETimer.Interop;
using System;
using System.Windows.Forms;

namespace PlainCEETimer.Controls
{
    public sealed class AppGroupBox : GroupBox
    {


        public AppGroupBox()
        {
            ThemeManager.SystemThemeChanged += ThemeManager_SystemThemeChanged;

            if (ThemeManager.AllowDarkMode && ThemeManager.IsDarkMode)
            {
                ForeColor = ThemeManager.DarkFore;
            }
        }

        private void ThemeManager_SystemThemeChanged(object sender, EventArgs e)
        {
            if (ThemeManager.IsDarkMode)
            {
                ForeColor = ThemeManager.DarkFore;
            }
            else
            {
                ForeColor = ThemeManager.LightFore;
            }
        }
    }
}
