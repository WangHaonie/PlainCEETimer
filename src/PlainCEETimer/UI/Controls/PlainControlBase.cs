using System.Drawing;
using System.Windows.Forms;

namespace PlainCEETimer.UI.Controls;

public abstract class PlainControlBase : Control
{
    private readonly ControlDpiScaleFix dpiScaleFix;

    public PlainControlBase()
    {
        dpiScaleFix = new();
    }

    protected override void ScaleControl(SizeF factor, BoundsSpecified specified)
    {
        if (dpiScaleFix.CanScale(factor))
        {
            base.ScaleControl(factor, specified);
        }
    }
}
