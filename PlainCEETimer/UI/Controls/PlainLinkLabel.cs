using System.Windows.Forms;
using PlainCEETimer.Interop;

namespace PlainCEETimer.UI.Controls
{
    public class PlainLinkLabel : LinkLabel
    {
        public new bool Enabled
        {
            get => Links[0].Enabled;
            set => Links[0].Enabled = value;
        }

        public PlainLinkLabel()
        {
            AutoSize = true;

            if (ThemeManager.ShouldUseDarkMode)
            {
                LinkColor = Colors.DarkForeLinkNormal;
                ActiveLinkColor = Colors.DarkForeLinkOnClick;
                DisabledLinkColor = Colors.DarkForeLinkDisabled;
            }
        }
    }
}
