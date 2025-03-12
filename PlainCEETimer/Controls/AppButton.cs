using PlainCEETimer.Interop;
using PlainCEETimer.Modules;
using System;
using System.Windows.Forms;

namespace PlainCEETimer.Controls
{
    public sealed class AppButton : Button
    {
        public AppButton()
        {
            CheckForIllegalCrossThreadCalls = false;
            ThemeManager.SystemThemeChanged += ThemeManager_SystemThemeChanged;
            FlatStyle = FlatStyle.System;

            if (ThemeManager.AllowDarkMode && ThemeManager.IsDarkMode)
            {
                ThemeManager.FlushControl(this, DarkControlType.Button);
            }
        }

        private void ThemeManager_SystemThemeChanged(object sender, EventArgs e)
        {
            ThemeManager.FlushControl(this, DarkControlType.Button);
        }
    }
}
