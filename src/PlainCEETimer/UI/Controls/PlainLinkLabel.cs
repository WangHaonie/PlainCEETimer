using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace PlainCEETimer.UI.Controls;

public class PlainLinkLabel : LinkLabel, IThemeAwareEx
{
    public PlainLinkLabel()
    {
        AutoSize = true;
        LinkBehavior = LinkBehavior.HoverUnderline;
        ThemeHelper.Attach(this);
    }

    internal void AdjustLine(bool flag)
    {
        TextAlign = flag
            ? DeviceDpi > 96F ? ContentAlignment.MiddleLeft : ContentAlignment.BottomLeft
            : ContentAlignment.TopLeft;
    }

    protected override void OnLinkClicked(LinkLabelLinkClickedEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {
            if (e.Link.LinkData is string link && !string.IsNullOrEmpty(link))
            {
                Process.Start(link);
            }

            base.OnLinkClicked(e);
        }
    }

    void IThemeAware.UpdateTheme(bool useDark, bool init)
    {
        LinkColor = useDark ? Colors.DarkForeLinkNormal : Colors.LightForeLinkNormal;
        ActiveLinkColor = useDark ? Colors.DarkForeLinkOnClick : Colors.LightForeLinkOnClick;
        DisabledLinkColor = useDark ? Colors.DarkForeLinkDisabled : Colors.LightForeLinkDisabled;
    }
}
