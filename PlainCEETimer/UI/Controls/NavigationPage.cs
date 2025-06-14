using System.Drawing;
using System.Windows.Forms;
using PlainCEETimer.Interop;

namespace PlainCEETimer.UI.Controls
{
    public class NavigationPage : Panel
    {
        public int Index { get; internal set; }

        public NavigationPage()
        {
            BackColor = ThemeManager.ShouldUseDarkMode ? ThemeManager.DarkBack : SystemColors.Window;
            Dock = DockStyle.Fill;
            Visible = false;
        }
    }
}
