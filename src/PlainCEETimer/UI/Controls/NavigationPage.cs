using System.Drawing;
using System.Windows.Forms;

namespace PlainCEETimer.UI.Controls;

public class NavigationPage : Panel
{
    public int Index { get; internal set; }

    public NavigationPage()
    {
        BackColor = ThemeManager.ShouldUseDarkMode ? Colors.DarkBackText : SystemColors.Window;
        Dock = DockStyle.Fill;
        Visible = false;
    }
}
