using System;
using System.Windows.Forms;
using PlainCEETimer.Interop;
using PlainCEETimer.Modules;

namespace PlainCEETimer.Controls
{
    public sealed class PlainButton : Button
    {
        public PlainButton()
        {
            FlatStyle = FlatStyle.System;
        }

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
