using System.Windows.Forms;
using PlainCEETimer.Interop;

namespace PlainCEETimer.Controls
{
    public class PlainLinkLabel : LinkLabel
    {
        public PlainLinkLabel()
        {
            LinkColor = ThemeManager.ShouldUseDarkMode ? ThemeManager.DarkForeLink : LinkColor;
        }
    }
}
