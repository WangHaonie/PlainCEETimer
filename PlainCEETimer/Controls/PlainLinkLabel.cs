using PlainCEETimer.Interop;
using System.Windows.Forms;

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
