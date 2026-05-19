using System.Diagnostics;
using System.Windows.Forms;
using PlainCEETimer.Modules.Extensions;

namespace PlainCEETimer.UI.Controls;

public class PlainLinkLabel : LinkLabel, IThemeAware
{
    private readonly ThemeHelper themeHelper;

    public PlainLinkLabel()
    {
        AutoSize = true;
        LinkBehavior = LinkBehavior.HoverUnderline;
        themeHelper = new(this);
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

    protected override void Dispose(bool disposing)
    {
        themeHelper.Destroy();
        base.Dispose(disposing);
    }

    void IThemeAware.UpdateTheme(bool useDark, bool init)
    {
        LinkColor = useDark ? Colors.DarkForeLinkNormal : Colors.LightForeLinkNormal;
        ActiveLinkColor = useDark ? Colors.DarkForeLinkOnClick : Colors.LightForeLinkOnClick;
        DisabledLinkColor = useDark ? Colors.DarkForeLinkDisabled : Colors.LightForeLinkDisabled;
    }
}
