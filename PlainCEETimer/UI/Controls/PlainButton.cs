using System;
using System.Windows.Forms;
using PlainCEETimer.Interop;
using PlainCEETimer.Modules;

namespace PlainCEETimer.UI.Controls
{
    public sealed class PlainButton : Button
    {
        public PlainButton()
        {
            FlatStyle = FlatStyle.System;
            UseVisualStyleBackColor = true;
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            if (ThemeManager.ShouldUseDarkMode)
            {
                ThemeManager.FlushControl(this, NativeStyle.Explorer);
            }

            base.OnHandleCreated(e);
        }
    }
}
