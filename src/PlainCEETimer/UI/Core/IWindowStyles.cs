namespace PlainCEETimer.UI.Core;

public interface IWindowStyles
{
    bool TopMost { get; set; }

    bool ShowInTaskbar { get; set; }

    bool Visible { get; set; }

    double Opacity { get; set; }
}