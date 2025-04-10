using PlainCEETimer.Interop;
using PlainCEETimer.Modules;
using System;
using System.Windows.Forms;

namespace PlainCEETimer.Controls
{
    public class PlainCheckBox : CheckBox
    {
        public PlainCheckBox()
        {
            UpdateStyle();
        }

        protected override void OnEnabledChanged(EventArgs e)
        {
            UpdateStyle();
            base.OnEnabledChanged(e);
        }

        private void UpdateStyle()
        {
            if (ThemeManager.ShouldUseDarkMode)
            {
                FlatStyle = Enabled ? FlatStyle.Standard : FlatStyle.System;
                ThemeManager.FlushDarkControl(this, NativeStyle.Explorer);
            }
        }
    }
}
