using System;
using System.Windows.Forms;
using PlainCEETimer.Interop;

namespace PlainCEETimer.Modules
{
    public class PlainButtonBase
    {
        private readonly ButtonBase button;

        public PlainButtonBase(ButtonBase target)
        {
            button = target;
            button.EnabledChanged += Button_EnabledChanged;
            UpdateStyle();
        }

        private void Button_EnabledChanged(object sender, EventArgs e)
        {
            UpdateStyle();
        }

        private void UpdateStyle()
        {
            if (ThemeManager.ShouldUseDarkMode)
            {
                button.FlatStyle = button.Enabled ? FlatStyle.Standard : FlatStyle.System;
                ThemeManager.FlushDarkControl(button, NativeStyle.Explorer);
            }
        }

        ~PlainButtonBase() => button.EnabledChanged -= Button_EnabledChanged;
    }
}
