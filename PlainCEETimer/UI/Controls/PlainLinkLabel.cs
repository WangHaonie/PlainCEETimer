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
            var normal = Colors.LightForeLinkNormal;
            var click = Colors.LightForeLinkOnClick;
            var disabled = Colors.LightForeLinkDisabled;

            if (ThemeManager.ShouldUseDarkMode)
            {
                normal = Colors.DarkForeLinkNormal;
                click = Colors.DarkForeLinkOnClick;
                disabled = Colors.DarkForeLinkDisabled;
            }

            LinkColor = normal;
            ActiveLinkColor = click;
            DisabledLinkColor = disabled;
        }
    }
}
