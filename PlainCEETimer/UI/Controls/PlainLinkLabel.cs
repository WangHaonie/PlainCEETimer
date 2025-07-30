using System.Drawing;
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
            LinkBehavior = LinkBehavior.HoverUnderline;

            Color normal;
            Color click;
            Color disabled;

            if (ThemeManager.ShouldUseDarkMode)
            {
                normal = Colors.DarkForeLinkNormal;
                click = Colors.DarkForeLinkOnClick;
                disabled = Colors.DarkForeLinkDisabled;
            }
            else
            {
                normal = Colors.LightForeLinkNormal;
                click = Colors.LightForeLinkOnClick;
                disabled = Colors.LightForeLinkDisabled;
            }

            LinkColor = normal;
            ActiveLinkColor = click;
            DisabledLinkColor = disabled;
        }
    }
}
