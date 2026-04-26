using PlainCEETimer.WPF.Controls;

namespace PlainCEETimer.UI.Core;

public class WPFWindowStyles(AppWindow window) : IWindowStyles
{
    public bool TopMost
    {
        get => window.Topmost;
        set => window.Topmost = value;
    }

    public bool ShowInTaskbar
    {
        get => window.ShowInTaskbar;
        set => window.ShowInTaskbar = value;
    }

    public bool Visible
    {
        get => window.IsVisible;
        set
        {
            if (value)
            {
                window.Show();
            }
            else
            {
                window.Hide();
            }
        }
    }

    public double Opacity
    {
        get => window.Opacity;
        set => window.Opacity = value;
    }
}
