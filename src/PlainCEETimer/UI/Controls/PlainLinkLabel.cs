using System.Diagnostics;
using System.Windows.Forms;

namespace PlainCEETimer.UI.Controls;

public class PlainLinkLabel : LinkLabel
{
    public new bool Enabled
    {
        get => Links[0].Enabled;
        set => Links[0].Enabled = value;
    }

    public string Hyperlink { get; set; }

    public PlainLinkLabel()
    {
        AutoSize = true;
        LinkBehavior = LinkBehavior.HoverUnderline;
        var normal = Colors.LightForeLinkNormal;
        var click = Colors.LightForeLinkOnClick;
        var disabled = Colors.LightForeLinkDisabled;

        if (ThemeManager.ShouldUseDarkMode)
        {
            normal = Colors.DarkForeLinkNormal;
            click = Colors.DarkForeLinkOnClick;
            disabled = Colors.DarkForeLinkDisabled;
        }

        LinkColor = normal;
        ActiveLinkColor = click;
        DisabledLinkColor = disabled;
    }

    protected override void OnLinkClicked(LinkLabelLinkClickedEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {
            if (!string.IsNullOrEmpty(Hyperlink))
            {
                Process.Start(Hyperlink);
            }

            base.OnLinkClicked(e);
        }
    }
}
