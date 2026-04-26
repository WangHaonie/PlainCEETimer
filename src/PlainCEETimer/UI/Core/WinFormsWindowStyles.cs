using PlainCEETimer.UI.Controls;

namespace PlainCEETimer.UI.Core;

public class WinFormsWindowStyles(AppForm form) : IWindowStyles
{
    public bool TopMost
    {
        get => form.TopMost;
        set => form.TopMost = value;
    }

    public bool ShowInTaskbar
    {
        get => form.ShowInTaskbar;
        set => form.ShowInTaskbar = value;
    }

    public bool Visible
    {
        get => form.Visible;
        set => form.Visible = value;
    }

    public double Opacity
    {
        get => form.Opacity;
        set => form.Opacity = value;
    }
}