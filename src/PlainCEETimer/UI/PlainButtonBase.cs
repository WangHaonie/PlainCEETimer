using System;
using System.Windows.Forms;
using PlainCEETimer.Interop;
using PlainCEETimer.Modules;

namespace PlainCEETimer.UI
{
    public class PlainButtonBase
    {
        private readonly ButtonBase Target;

        public PlainButtonBase(ButtonBase target)
        {
            target.UseVisualStyleBackColor = true;

            if (ThemeManager.ShouldUseDarkMode)
            {
                Target = target;
                Target.EnabledChanged += Button_EnabledChanged;
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
            Target.FlatStyle = Target.Enabled ? FlatStyle.Standard : FlatStyle.System;
            ThemeManager.FlushControl(Target, NativeStyle.ExplorerDark);
        }

        ~PlainButtonBase()
        {
            if (Target != null)
            {
                Target.EnabledChanged -= Button_EnabledChanged;
            }
        }
    }
}
