using System;
using System.Windows.Forms;
using PlainCEETimer.Interop;
using PlainCEETimer.Modules;

namespace PlainCEETimer.UI.Controls
{
    public sealed class PlainButton : Button
    {
        private readonly ContextMenu Menu;

        public PlainButton(ContextMenu menu)
        {
            Menu = menu;
            FlatStyle = FlatStyle.System;
            UseVisualStyleBackColor = true;
            DoubleBuffered = true;
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            if (ThemeManager.ShouldUseDarkMode)
            {
                ThemeManager.FlushControl(this, NativeStyle.Explorer);
            }

            base.OnHandleCreated(e);
        }

        protected override void OnClick(EventArgs e)
        {
            Menu?.Show(this, new(0, Height));
            base.OnClick(e);
        }
    }
}
