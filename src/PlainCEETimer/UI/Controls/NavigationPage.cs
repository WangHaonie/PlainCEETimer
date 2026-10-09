using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using PlainCEETimer.Interop;

namespace PlainCEETimer.UI.Controls;

[DebuggerDisplay("Text={Text}, Index={Header.Index}")]
public sealed class NavigationPage : Panel, IThemeAwareEx
{
    public new string Text { get; set; }

    internal TreeNode Header { get; set; }

    public NavigationPage()
    {
        Visible = false;
        Dock = DockStyle.Fill;
        AutoScroll = true;
        HScroll = false;
        ThemeHelper.Attach(this);
    }

    void IThemeAware.UpdateTheme(bool useDark, bool init)
    {
        BackColor = useDark ? Colors.DarkBackText : SystemColors.Window;

        if (!init && VScroll)
        {
            Win32UI.SetWindowTheme(Handle, null, null);
        }
    }
}