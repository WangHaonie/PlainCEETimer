using System.Drawing;
using System.Windows.Forms;

namespace PlainCEETimer.UI.Controls;

public sealed class NavigationPage : Panel
{
    public new string Text { get; set; }

    internal TreeNode Header { get; set; }

    public NavigationPage()
    {
        BackColor = ThemeManager.ShouldUseDarkMode ? Colors.DarkBackText : SystemColors.Window;
        Dock = DockStyle.Fill;
        Visible = false;
    }
}