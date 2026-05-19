using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using PlainCEETimer.Modules.Extensions;

namespace PlainCEETimer.UI.Controls;

[DebuggerDisplay("Text={Text}, Index={Header.Index}")]
public sealed class NavigationPage : Panel, IThemeAware
{
    public new string Text { get; set; }

    internal TreeNode Header { get; set; }

    private readonly ThemeHelper themeHelper;

    public NavigationPage()
    {
        themeHelper = new(this);
        Dock = DockStyle.Fill;
        Visible = false;
    }

    protected override void Dispose(bool disposing)
    {
        themeHelper.Destroy();
        base.Dispose(disposing);
    }

    void IThemeAware.UpdateTheme(bool useDark, bool init)
    {
        BackColor = useDark ? Colors.DarkBackText : SystemColors.Window;
    }
}