using PlainCEETimer.Interop;
using PlainCEETimer.Modules;
using System;
using System.Windows.Forms;

namespace PlainCEETimer.Controls
{
    public sealed class PlainRadioButton : RadioButton
    {
        protected override void OnHandleCreated(EventArgs e)
        {
            if (ThemeManager.ShouldUseDarkMode)
            {
                ThemeManager.FlushDarkControl(this, NativeStyle.Explorer);
            }

            base.OnHandleCreated(e);
        }
    }
}
