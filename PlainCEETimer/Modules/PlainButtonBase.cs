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
            target.UseVisualStyleBackColor = true;

            if (ThemeManager.ShouldUseDarkMode)
            {
                button = target;
                button.EnabledChanged += Button_EnabledChanged;
                UpdateStyle();
            }
            else
            {
                target.FlatStyle = FlatStyle.System;
            }
        }

        private void Button_EnabledChanged(object sender, EventArgs e)
        {
            UpdateStyle();
        }

        private void UpdateStyle()
        {
            button.FlatStyle = button.Enabled ? FlatStyle.Standard : FlatStyle.System;
            ThemeManager.FlushControl(button, NativeStyle.Explorer);
        }

        ~PlainButtonBase()
        {
            if (button != null)
            {
                button.EnabledChanged -= Button_EnabledChanged;
            }
        }
    }
}
