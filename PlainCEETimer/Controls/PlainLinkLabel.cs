using PlainCEETimer.Interop;
using System.Drawing;
using System.Windows.Forms;

namespace PlainCEETimer.Controls
{
    public class PlainLinkLabel : LinkLabel
    {
        public PlainLinkLabel()
        {
            LinkColor = ThemeManager.ShouldUseDarkMode ? Color.FromArgb(95, 197, 255) : LinkColor;
        }
    }
}
