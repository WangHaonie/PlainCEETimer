using System;
using System.Windows.Forms;
using PlainCEETimer.Interop;
using PlainCEETimer.Modules;

namespace PlainCEETimer.Controls
{
    public sealed class PlainCheckBox : CheckBox
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
