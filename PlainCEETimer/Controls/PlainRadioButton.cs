using System;
using System.Windows.Forms;
using PlainCEETimer.Interop;

namespace PlainCEETimer.Controls
{
    public sealed class PlainRadioButton : RadioButton
    {
        protected override void OnHandleCreated(EventArgs e)
        {
            if (ThemeManager.ShouldUseDarkMode)
            {
                ThemeManager.FlushDarkControl(this, Modules.NativeStyle.Explorer);
            }

            base.OnHandleCreated(e);
        }
    }
}
