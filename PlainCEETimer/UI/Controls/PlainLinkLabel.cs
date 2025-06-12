using System.Drawing;
using System.Windows.Forms;
using PlainCEETimer.Interop;

namespace PlainCEETimer.UI.Controls
{
    public class PlainLinkLabel : LinkLabel
    {
        public PlainLinkLabel()
        {
            AutoSize = true;
            LinkColor = ThemeManager.ShouldUseDarkMode ? ThemeManager.DarkForeLink : LinkColor;
            ActiveLinkColor = Color.Blue;
            VisitedLinkColor = Color.Blue;
        }
    }
}
